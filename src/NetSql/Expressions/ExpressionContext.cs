using System;
using System.Linq.Expressions;
using System.Text;
using NetSql.Entities;
using NetSql.Internal;
using NetSql.SqlAdapter;

namespace NetSql.Expressions
{
    internal class ExpressionContext<TEntity> : IExpressionContext<TEntity> where TEntity : Entity, new()
    {
        private readonly ISqlAdapter _sqlAdapter;
        private readonly StringBuilder _sqlBuilder;

        public ExpressionContext(ISqlAdapter sqlAdapter)
        {
            _sqlAdapter = sqlAdapter;
            _sqlBuilder = new StringBuilder();
        }

        /// <summary>
        /// 转换成Sql
        /// </summary>
        /// <param name="exp"></param>
        /// <returns></returns>
        public string ToSql(Expression<Func<TEntity, bool>> exp)
        {
            if (exp == null)
                return string.Empty;

            Resolve(exp);

            if (_sqlBuilder[0] == '(' && _sqlBuilder[_sqlBuilder.Length - 1] == ')')
            {
                _sqlBuilder.Remove(0, 1);
                _sqlBuilder.Remove(_sqlBuilder.Length - 2, 1);
            }

            return _sqlBuilder.ToString();
        }

        public string ToSql(Expression<Func<TEntity, TEntity>> exp)
        {
            if (exp == null)
                return string.Empty;

            Resolve(exp);

            if (_sqlBuilder[0] == '(' && _sqlBuilder[_sqlBuilder.Length - 1] == ')')
            {
                _sqlBuilder.Remove(0, 1);
                _sqlBuilder.Remove(_sqlBuilder.Length - 2, 1);
            }

            return _sqlBuilder.ToString();
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
                case ExpressionType.New:
                    DynamicInvokeResolve(exp);
                    break;
                case ExpressionType.Not:
                    NotResolve(exp);
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
                _sqlAdapter.AppendQuote(_sqlBuilder, constantExp.Value.ToString());
            else if (exp.Type == typeof(bool))
                _sqlAdapter.AppendQuote(_sqlBuilder, constantExp.Value.ToBool().ToIntString());
            else
                _sqlBuilder.Append(constantExp.Value);
        }

        private void MemberAccessResolve(Expression exp)
        {
            if (exp == null || !(exp is MemberExpression memberExp))
                return;

            if (memberExp.Expression != null && memberExp.Expression.NodeType == ExpressionType.Parameter)
            {
                _sqlAdapter.AppendQuote(_sqlBuilder, memberExp.Member.Name);
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
            if (exp == null)
                return;

            var f = Expression.Lambda(exp).Compile();
            var value = f.DynamicInvoke();

            if (exp.Type == typeof(DateTime) || exp.Type == typeof(string) || exp.Type == typeof(char))
                _sqlBuilder.AppendFormat("'{0}'", value);
            else if (exp.Type.IsEnum)
                _sqlBuilder.AppendFormat("{0}", value.ToInt());
            else
                _sqlBuilder.AppendFormat("{0}", value);
        }

        private void NotResolve(Expression exp)
        {
            if (exp == null)
                return;

            _sqlBuilder.Append("(");

            UnaryResolve(exp);

            _sqlBuilder.Append(" = 0)");
        }

        #endregion
    }
}
