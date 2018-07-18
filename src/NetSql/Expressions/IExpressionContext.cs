using System.Linq.Expressions;

namespace NetSql.Expressions
{
    internal interface IExpressionContext
    {
        string ToSql(Expression exp);
    }
}
