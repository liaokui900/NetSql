using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using NetSql.Entities;
using NetSql.Internal;
using NetSql.SqlAdapter;

namespace NetSql.Expressions
{
    internal class ExpressionResolve : IExpressionResolve
    {
        private readonly IEntityDescriptor _descriptor;
        private readonly ISqlAdapter _sqlAdapter;
        private StringBuilder _sqlBuilder;

        public ExpressionResolve(ISqlAdapter sqlAdapter, IEntityDescriptor descriptor)
        {
            _sqlAdapter = sqlAdapter;
            _descriptor = descriptor;
        }

        public string ToSql(Expression exp)
        {
            if (exp == null)
                return string.Empty;

            _sqlBuilder = new StringBuilder();

            Resolve(exp);

            //删除多余的括号
            if (_sqlBuilder.Length > 1 && _sqlBuilder[0] == '(' && _sqlBuilder[_sqlBuilder.Length - 1] == ')')
            {
                _sqlBuilder.Remove(0, 1);
                _sqlBuilder.Remove(_sqlBuilder.Length - 1, 1);
            }

            var sql = _sqlBuilder.ToString();
            _sqlBuilder = null;
            return sql;
        }

        public string ToSelectSql(Expression exp)
        {
            if (exp == null)
                return string.Empty;

            var sqlBuilder = new StringBuilder();

            if (exp is LambdaExpression lambdaExpression && lambdaExpression.Body is NewExpression newExpression && newExpression.Members.Any())
            {
                for (var i = 0; i < newExpression.Members.Count; i++)
                {
                    var col = GetColumn(newExpression.Members[i]);
                    if (col != null)
                    {
                        if (col.Name.Equals(col.PropertyInfo.Name))
                            sqlBuilder.Append(_sqlAdapter.AppendQuote(col.Name));
                        else
                            sqlBuilder.AppendFormat("{0} AS '{1}'", _sqlAdapter.AppendQuote(col.Name), col.PropertyInfo.Name);
                        if (i != newExpression.Members.Count - 1)
                        {
                            sqlBuilder.Append(",");
                        }
                    }
                }
            }

            return sqlBuilder.ToString();
        }

        #region ==表达式解析==

        private void Resolve(Expression exp)
        {
            switch (exp.NodeType)
            {
                case ExpressionType.Lambda:
                    LambdaResolve(exp);
                    break;
                case ExpressionType.Add:
                case ExpressionType.AddChecked:
                case ExpressionType.Subtract:
                case ExpressionType.SubtractChecked:
                case ExpressionType.Multiply:
                case ExpressionType.MultiplyChecked:
                case ExpressionType.Divide:
                case ExpressionType.Modulo:
                case ExpressionType.And:
                case ExpressionType.AndAlso:
                case ExpressionType.Or:
                case ExpressionType.OrElse:
                case ExpressionType.LessThan:
                case ExpressionType.LessThanOrEqual:
                case ExpressionType.GreaterThan:
                case ExpressionType.GreaterThanOrEqual:
                case ExpressionType.Equal:
                case ExpressionType.NotEqual:
                case ExpressionType.Coalesce:
                case ExpressionType.ArrayIndex:
                case ExpressionType.RightShift:
                case ExpressionType.LeftShift:
                case ExpressionType.ExclusiveOr:
                    BinaryResolve(exp);
                    break;
                case ExpressionType.Constant:
                    ConstantResolve(exp);
                    break;
                case ExpressionType.MemberAccess:
                    MemberAccessResolve(exp);
                    break;
                case ExpressionType.Convert:
                case ExpressionType.ConvertChecked:
                    UnaryResolve(exp);
                    break;
                case ExpressionType.Call:
                    CallResolve(exp);
                    break;
                case ExpressionType.Not:
                    NotResolve(exp);
                    break;
                case ExpressionType.MemberInit:
                    MemberInitResolve(exp);
                    break;
            }
        }

        private void LambdaResolve(Expression exp)
        {
            if (exp == null || !(exp is LambdaExpression lambdaExp))
                return;

            Resolve(lambdaExp.Body);
        }

        private void BinaryResolve(Expression exp)
        {
            if (exp == null || !(exp is BinaryExpression binaryExp))
                return;

            _sqlBuilder.Append("(");

            Resolve(binaryExp.Left);

            switch (binaryExp.NodeType)
            {
                case ExpressionType.And:
                case ExpressionType.AndAlso:
                    _sqlBuilder.Append(" AND ");
                    break;
                case ExpressionType.GreaterThan:
                    _sqlBuilder.Append(" > ");
                    break;
                case ExpressionType.GreaterThanOrEqual:
                    _sqlBuilder.Append(" >= ");
                    break;
                case ExpressionType.LessThan:
                    _sqlBuilder.Append(" < ");
                    break;
                case ExpressionType.LessThanOrEqual:
                    _sqlBuilder.Append(" <= ");
                    break;
                case ExpressionType.Equal:
                    _sqlBuilder.Append(" = ");
                    break;
                case ExpressionType.OrElse:
                case ExpressionType.Or:
                    _sqlBuilder.Append(" OR ");
                    break;
            }

            Resolve(binaryExp.Right);

            _sqlBuilder.Append(")");
        }

        private void ConstantResolve(Expression exp)
        {
            if (exp == null || !(exp is ConstantExpression constantExp))
                return;

            if (exp.Type == typeof(string))
                _sqlBuilder.AppendFormat("'{0}'", constantExp.Value);
            else if (exp.Type == typeof(bool))
                _sqlBuilder.AppendFormat("{0}", constantExp.Value.ToBool().ToIntString());
            else
                _sqlBuilder.Append(constantExp.Value);
        }

        private void MemberAccessResolve(Expression exp)
        {
            if (exp == null || !(exp is MemberExpression memberExp))
                return;

            if (memberExp.Expression != null && memberExp.Expression.NodeType == ExpressionType.Parameter)
            {
                var col = _descriptor.Columns.First(c => c.PropertyInfo.Name.Equals(memberExp.Member.Name));

                _sqlAdapter.AppendQuote(_sqlBuilder, col.Name);
            }
            else
            {
                //对于非实体属性的成员，如外部变量等
                DynamicInvokeResolve(exp);
            }
        }

        private void UnaryResolve(Expression exp)
        {
            if (exp == null || !(exp is UnaryExpression unaryExp))
                return;

            Resolve(unaryExp.Operand);
        }

        private void DynamicInvokeResolve(Expression exp)
        {
            var value = DynamicInvoke(exp);

            if (exp.Type == typeof(DateTime) || exp.Type == typeof(string) || exp.Type == typeof(char))
                _sqlBuilder.AppendFormat("'{0}'", value);
            else if (exp.Type.IsEnum)
                _sqlBuilder.AppendFormat("{0}", value.ToInt());
            else
                _sqlBuilder.AppendFormat("{0}", value);
        }

        private void CallResolve(Expression exp)
        {
            if (exp == null || !(exp is MethodCallExpression callExp))
                return;

            var methodName = callExp.Method.Name;
            if (methodName.Equals("StartsWith"))
            {
                StartsWithResolve(callExp);
            }
            else if (methodName.Equals("EndsWith"))
            {
                EndsWithResolve(callExp);
            }
            else if (methodName.Equals("Contains"))
            {
                ContainsResolve(callExp);
            }
            else if (methodName.Equals("Equals"))
            {
                EqualsResolve(callExp);
            }
            else
            {
                DynamicInvokeResolve(exp);
            }
        }

        private void StartsWithResolve(MethodCallExpression exp)
        {
            if (exp.Object is MemberExpression objExp && objExp.Expression.NodeType == ExpressionType.Parameter)
            {
                var col = GetColumn(objExp.Member);
                if (col == null)
                    return;

                string value;
                if (exp.Arguments[0] is ConstantExpression c)
                {
                    value = c.Value.ToString();
                }
                else
                {
                    value = DynamicInvoke(exp.Arguments[0]).ToString();
                }

                _sqlBuilder.AppendFormat("{0} LIKE '{1}%'", _sqlAdapter.AppendQuote(col.Name), value);
            }
        }

        private void EndsWithResolve(MethodCallExpression exp)
        {
            if (exp.Object is MemberExpression objExp && objExp.Expression.NodeType == ExpressionType.Parameter)
            {
                var col = GetColumn(objExp.Member);
                if (col == null)
                    return;

                string value;
                if (exp.Arguments[0] is ConstantExpression c)
                {
                    value = c.Value.ToString();
                }
                else
                {
                    value = DynamicInvoke(exp.Arguments[0]).ToString();
                }

                _sqlBuilder.AppendFormat("{0} LIKE '%{1}'", _sqlAdapter.AppendQuote(col.Name), value);
            }
        }

        private void ContainsResolve(MethodCallExpression exp)
        {
            if (exp.Object is MemberExpression objExp)
            {
                if (objExp.Expression.NodeType == ExpressionType.Parameter)
                {
                    var col = GetColumn(objExp.Member);
                    if (col == null)
                        return;

                    string value;
                    if (exp.Arguments[0] is ConstantExpression c)
                    {
                        value = c.Value.ToString();
                    }
                    else
                    {
                        value = DynamicInvoke(exp.Arguments[0]).ToString();
                    }

                    _sqlBuilder.AppendFormat("{0} LIKE '%{1}%'", _sqlAdapter.AppendQuote(col.Name), value);
                }
                else if (objExp.Type.IsGenericType && exp.Arguments[0] is MemberExpression argExp && argExp.Expression.NodeType == ExpressionType.Parameter)
                {
                    var col = GetColumn(argExp.Member);
                    if (col == null)
                        return;

                    _sqlBuilder.AppendFormat(" {0} IN (", _sqlAdapter.AppendQuote(col.Name));

                    #region ==解析集合==

                    var constant = objExp.Expression as ConstantExpression;
                    var value = ((FieldInfo)objExp.Member).GetValue(constant.Value);
                    var valueType = objExp.Type.GenericTypeArguments[0];
                    var isValueType = false;
                    var list = new List<string>();
                    if (valueType == typeof(string))
                    {
                        list = value as List<string>;
                    }
                    else if (valueType == typeof(char))
                    {
                        if (value is List<char> valueList)
                        {
                            foreach (var c in valueList)
                            {
                                list.Add(c.ToString());
                            }
                        }
                    }
                    else if (valueType == typeof(DateTime))
                    {
                        if (value is List<DateTime> valueList)
                        {
                            foreach (var c in valueList)
                            {
                                list.Add(c.ToString("yyyy-MM-dd HH:mm:ss"));
                            }
                        }
                    }
                    else if (valueType == typeof(int))
                    {
                        isValueType = true;
                        if (value is List<int> valueList)
                        {
                            foreach (var c in valueList)
                            {
                                list.Add(c.ToString());
                            }
                        }
                    }
                    else if (valueType == typeof(long))
                    {
                        isValueType = true;
                        if (value is List<long> valueList)
                        {
                            foreach (var c in valueList)
                            {
                                list.Add(c.ToString());
                            }
                        }
                    }
                    else if (valueType == typeof(double))
                    {
                        isValueType = true;
                        if (value is List<double> valueList)
                        {
                            foreach (var c in valueList)
                            {
                                list.Add(c.ToString());
                            }
                        }
                    }
                    else if (valueType == typeof(float))
                    {
                        isValueType = true;
                        if (value is List<float> valueList)
                        {
                            foreach (var c in valueList)
                            {
                                list.Add(c.ToString());
                            }
                        }
                    }
                    else if (valueType == typeof(decimal))
                    {
                        isValueType = true;
                        if (value is List<decimal> valueList)
                        {
                            foreach (var c in valueList)
                            {
                                list.Add(c.ToString());
                            }
                        }
                    }

                    if (list == null)
                        return;

                    #endregion

                    //值类型不带引号
                    if (isValueType)
                    {
                        for (var i = 0; i < list.Count; i++)
                        {
                            _sqlBuilder.AppendFormat("{0}", list[i]);
                            if (i != list.Count - 1)
                            {
                                _sqlBuilder.Append(",");
                            }
                        }
                    }
                    else
                    {
                        for (var i = 0; i < list.Count; i++)
                        {
                            _sqlBuilder.AppendFormat("'{0}'", list[i].Replace("'", "''"));
                            if (i != list.Count - 1)
                            {
                                _sqlBuilder.Append(",");
                            }
                        }
                    }
                    _sqlBuilder.Append(") ");
                }
            }
            else if (exp.Arguments[0].Type.IsArray && exp.Arguments[1] is MemberExpression argExp && argExp.Expression.NodeType == ExpressionType.Parameter)
            {
                var col = GetColumn(argExp.Member);
                if (col == null)
                    return;

                _sqlBuilder.AppendFormat(" {0} IN (", _sqlAdapter.AppendQuote(col.Name));

                #region ==解析数组==

                var member = exp.Arguments[0] as MemberExpression;
                var constant = member.Expression as ConstantExpression;
                var value = ((FieldInfo)member.Member).GetValue(constant.Value);
                var valueType = member.Type.FullName;
                var isValueType = false;
                string[] list = null;
                if (valueType == "System.String[]")
                {
                    list = value as string[];
                }
                else if (valueType == "System.Char[]")
                {
                    if (value is char[] valueList)
                    {
                        list = new string[valueList.Length];
                        for (var i = 0; i < valueList.Length; i++)
                        {
                            list[i] = valueList[i].ToString();
                        }
                    }
                }
                else if (valueType == "System.Datetime[]")
                {
                    if (value is DateTime[] valueList)
                    {
                        list = new string[valueList.Length];
                        for (var i = 0; i < valueList.Length; i++)
                        {
                            list[i] = valueList[i].ToString("yyyy-MM-dd HH:mm:ss");
                        }
                    }
                }
                else if (valueType == "System.Int32[]")
                {
                    isValueType = true;
                    if (value is int[] valueList)
                    {
                        list = new string[valueList.Length];
                        for (var i = 0; i < valueList.Length; i++)
                        {
                            list[i] = valueList[i].ToString();
                        }
                    }
                }
                else if (valueType == "System.Int64[]")
                {
                    isValueType = true;
                    if (value is long[] valueList)
                    {
                        list = new string[valueList.Length];
                        for (var i = 0; i < valueList.Length; i++)
                        {
                            list[i] = valueList[i].ToString();
                        }
                    }
                }
                else if (valueType == "System.Double[]")
                {
                    isValueType = true;
                    if (value is double[] valueList)
                    {
                        list = new string[valueList.Length];
                        for (var i = 0; i < valueList.Length; i++)
                        {
                            list[i] = valueList[i].ToString();
                        }
                    }
                }
                else if (valueType == "System.Single[]")
                {
                    isValueType = true;
                    if (value is float[] valueList)
                    {
                        list = new string[valueList.Length];
                        for (var i = 0; i < valueList.Length; i++)
                        {
                            list[i] = valueList[i].ToString();
                        }
                    }
                }
                else if (valueType == "System.Decimal[]")
                {
                    isValueType = true;
                    if (value is decimal[] valueList)
                    {
                        list = new string[valueList.Length];
                        for (var i = 0; i < valueList.Length; i++)
                        {
                            list[i] = valueList[i].ToString();
                        }
                    }
                }

                if (list == null)
                    return;

                #endregion

                //值类型不带引号
                if (isValueType)
                {
                    for (var i = 0; i < list.Length; i++)
                    {
                        _sqlBuilder.AppendFormat("{0}", list[i]);
                        if (i != list.Length - 1)
                        {
                            _sqlBuilder.Append(",");
                        }
                    }
                }
                else
                {
                    for (var i = 0; i < list.Length; i++)
                    {
                        _sqlBuilder.AppendFormat("'{0}'", list[i].Replace("'", "''"));
                        if (i != list.Length - 1)
                        {
                            _sqlBuilder.Append(",");
                        }
                    }
                }
                _sqlBuilder.Append(") ");
            }
        }

        private void EqualsResolve(MethodCallExpression exp)
        {
            if (exp.Object is MemberExpression objExp && objExp.Expression.NodeType == ExpressionType.Parameter)
            {
                var col = GetColumn(objExp.Member);
                if (col == null)
                    return;

                string value;
                if (exp.Arguments[0] is ConstantExpression c)
                {
                    value = c.Value.ToString();
                }
                else
                {
                    value = DynamicInvoke(exp.Arguments[0]).ToString();
                }

                _sqlBuilder.AppendFormat("{0} = '{1}'", _sqlAdapter.AppendQuote(col.Name), value);
            }
        }

        private void NotResolve(Expression exp)
        {
            if (exp == null)
                return;

            _sqlBuilder.Append("(");

            UnaryResolve(exp);

            _sqlBuilder.Append(" = 0)");
        }

        private void MemberInitResolve(Expression exp)
        {
            if (exp == null || !(exp is MemberInitExpression initExp) || !initExp.Bindings.Any())
                return;

            foreach (var binding in initExp.Bindings)
            {
                if (binding is MemberAssignment assignment)
                {
                    var col = GetColumn(assignment.Member);
                    if (col != null)
                    {
                        _sqlBuilder.AppendFormat("{0}=", _sqlAdapter.AppendQuote(col.Name));
                        Resolve(assignment.Expression);
                        _sqlBuilder.Append(",");
                    }
                }
            }

            if (_sqlBuilder.Length > 0)
            {
                _sqlBuilder.Remove(_sqlBuilder.Length - 1, 1);
            }
        }

        /// <summary>
        /// 根据成员信息获取对应的列信息
        /// </summary>
        /// <param name="member"></param>
        /// <returns></returns>
        private ColumnDescriptor GetColumn(MemberInfo member)
        {
            return _descriptor.Columns.FirstOrDefault(c => c.PropertyInfo.Name.Equals(member.Name));
        }

        /// <summary>
        /// 执行表达式
        /// </summary>
        /// <param name="exp"></param>
        /// <returns></returns>
        private object DynamicInvoke(Expression exp)
        {
            return Expression.Lambda(exp).Compile().DynamicInvoke();
        }

        #endregion
    }
}
