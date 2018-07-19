using System;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using NetSql.Entities;
using NetSql.Internal;
using NetSql.SqlAdapter;

namespace NetSql.Expressions
{
    internal class ExpressionContext : IExpressionContext
    {
        private readonly IEntityDescriptor _entityDescriptor;
        private readonly ISqlAdapter _sqlAdapter;

        public ExpressionContext(ISqlAdapter sqlAdapter, IEntityDescriptor entityDescriptor)
        {
            _sqlAdapter = sqlAdapter;
            _entityDescriptor = entityDescriptor;
        }

        /// <summary>
        /// 转换成Sql
        /// </summary>
        /// <param name="exp"></param>
        /// <returns></returns>
        public string ToSql(Expression exp)
        {
            if (exp == null)
                return string.Empty;

            var sqlBuilder = new StringBuilder();

            Resolve(exp, sqlBuilder);

            if (sqlBuilder[0] == '(' && sqlBuilder[sqlBuilder.Length - 1] == ')')
            {
                sqlBuilder.Remove(0, 1);
                sqlBuilder.Remove(sqlBuilder.Length - 1, 1);
            }

            return sqlBuilder.ToString();
        }

        public string ToSelectSql(Expression exp)
        {
            if (exp == null)
                return string.Empty;

            var sqlBuilder = new StringBuilder();

            if (exp is LambdaExpression lambdaExpression && lambdaExpression.Body is NewExpression newExpression && newExpression.Members.Any())
            {
                foreach (var member in newExpression.Members)
                {
                    sqlBuilder.AppendFormat("{0},", member.Name);
                }

                sqlBuilder.Remove(sqlBuilder.Length - 1, 1);
            }

            return sqlBuilder.ToString();
        }

        #region ==表达式解析==

        private void Resolve(Expression exp, StringBuilder sqlBuilder)
        {
            switch (exp.NodeType)
            {
                case ExpressionType.Lambda:
                    LambdaResolve(exp, sqlBuilder);
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
                    BinaryResolve(exp, sqlBuilder);
                    break;
                case ExpressionType.Constant:
                    ConstantResolve(exp, sqlBuilder);
                    break;
                case ExpressionType.MemberAccess:
                    MemberAccessResolve(exp, sqlBuilder);
                    break;
                case ExpressionType.Convert:
                case ExpressionType.ConvertChecked:
                    UnaryResolve(exp, sqlBuilder);
                    break;
                case ExpressionType.Call:
                case ExpressionType.New:
                    DynamicInvokeResolve(exp, sqlBuilder);
                    break;
                case ExpressionType.Not:
                    NotResolve(exp, sqlBuilder);
                    break;
                case ExpressionType.MemberInit:
                    MemberInitResolve(exp, sqlBuilder);
                    break;
            }
        }

        private void LambdaResolve(Expression exp, StringBuilder sqlBuilder)
        {
            if (exp == null || !(exp is LambdaExpression lambdaExp))
                return;

            Resolve(lambdaExp.Body, sqlBuilder);
        }

        private void BinaryResolve(Expression exp, StringBuilder sqlBuilder)
        {
            if (exp == null || !(exp is BinaryExpression binaryExp))
                return;

            sqlBuilder.Append("(");

            Resolve(binaryExp.Left, sqlBuilder);

            switch (binaryExp.NodeType)
            {
                case ExpressionType.And:
                case ExpressionType.AndAlso:
                    sqlBuilder.Append(" AND ");
                    break;
                case ExpressionType.GreaterThan:
                    sqlBuilder.Append(" > ");
                    break;
                case ExpressionType.GreaterThanOrEqual:
                    sqlBuilder.Append(" >= ");
                    break;
                case ExpressionType.LessThan:
                    sqlBuilder.Append(" < ");
                    break;
                case ExpressionType.LessThanOrEqual:
                    sqlBuilder.Append(" <= ");
                    break;
                case ExpressionType.Equal:
                    sqlBuilder.Append(" = ");
                    break;
                case ExpressionType.OrElse:
                case ExpressionType.Or:
                    sqlBuilder.Append(" OR ");
                    break;
            }

            Resolve(binaryExp.Right, sqlBuilder);

            sqlBuilder.Append(")");
        }

        private void ConstantResolve(Expression exp, StringBuilder sqlBuilder)
        {
            if (exp == null || !(exp is ConstantExpression constantExp))
                return;

            if (exp.Type == typeof(string))
                sqlBuilder.AppendFormat("'{0}'", constantExp.Value);
            else if (exp.Type == typeof(bool))
                sqlBuilder.AppendFormat("{0}", constantExp.Value.ToBool().ToIntString());
            else
                sqlBuilder.Append(constantExp.Value);
        }

        private void MemberAccessResolve(Expression exp, StringBuilder sqlBuilder)
        {
            if (exp == null || !(exp is MemberExpression memberExp))
                return;

            if (memberExp.Expression != null && memberExp.Expression.NodeType == ExpressionType.Parameter)
            {
                var col = _entityDescriptor.Columns.First(c => c.PropertyInfo.Name.Equals(memberExp.Member.Name));

                _sqlAdapter.AppendQuote(sqlBuilder, col.Name);
            }
            else
            {
                //对于非实体属性的成员，如外部变量等
                DynamicInvokeResolve(exp, sqlBuilder);
            }
        }

        private void UnaryResolve(Expression exp, StringBuilder sqlBuilder)
        {
            if (exp == null || !(exp is UnaryExpression unaryExp))
                return;

            Resolve(unaryExp.Operand, sqlBuilder);
        }

        private void DynamicInvokeResolve(Expression exp, StringBuilder sqlBuilder)
        {
            if (exp == null)
                return;

            var f = Expression.Lambda(exp).Compile();
            var value = f.DynamicInvoke();

            if (exp.Type == typeof(DateTime) || exp.Type == typeof(string) || exp.Type == typeof(char))
                sqlBuilder.AppendFormat("'{0}'", value);
            else if (exp.Type.IsEnum)
                sqlBuilder.AppendFormat("{0}", value.ToInt());
            else
                sqlBuilder.AppendFormat("{0}", value);
        }

        private void NotResolve(Expression exp, StringBuilder sqlBuilder)
        {
            if (exp == null)
                return;

            sqlBuilder.Append("(");

            UnaryResolve(exp, sqlBuilder);

            sqlBuilder.Append(" = 0)");
        }

        private void MemberInitResolve(Expression exp, StringBuilder sqlBuilder)
        {
            if (exp == null || !(exp is MemberInitExpression initExp) || !initExp.Bindings.Any())
                return;

            foreach (var binding in initExp.Bindings)
            {
                if (binding is MemberAssignment assignment)
                {
                    var col = _entityDescriptor.Columns.FirstOrDefault(c => c.PropertyInfo.Name.Equals(assignment.Member.Name));

                    if (col != null)
                    {
                        sqlBuilder.AppendFormat("{0}=", col.Name);
                        Resolve(assignment.Expression, sqlBuilder);
                        sqlBuilder.Append(",");
                    }
                }
            }

            if (sqlBuilder.Length > 0)
            {
                sqlBuilder.Remove(sqlBuilder.Length - 1, 1);
            }
        }

        #endregion
    }
}
