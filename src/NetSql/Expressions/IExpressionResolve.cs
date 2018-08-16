using System.Linq.Expressions;

namespace NetSql.Expressions
{
    internal interface IExpressionResolve
    {

        /// <summary>
        /// 转换成Sql
        /// </summary>
        /// <param name="exp"></param>
        /// <returns></returns>
        string ToSql(Expression exp);

        /// <summary>
        /// 解析查询指定列的SQL
        /// </summary>
        /// <param name="exp"></param>
        /// <returns></returns>
        string ToSelectSql(Expression exp);
    }
}
