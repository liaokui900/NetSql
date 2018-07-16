using System.Text;
using NetSql.Pagination;

namespace NetSql.SqlAdapter
{
    public class SqlServerAdapter : SqlAdapterAbstract
    {
        /// <summary>
        /// 左引号
        /// </summary>
        public override char LeftQuote => '[';

        /// <summary>
        /// 右引号
        /// </summary>
        public override char RightQuote => ']';

        /// <summary>
        /// 获取最后新增ID语句
        /// </summary>
        public override string IdentitySql => "SELECT SCOPE_IDENTITY() ID;";

        /// <summary>
        /// 获取分页语句
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <param name="queryWhere">查询条件</param>
        /// <param name="paging">分页类</param>
        /// <returns></returns>
        public override string GeneratePagingSql(string tableName, string queryWhere, Paging paging)
        {
            var sql = new StringBuilder($"SELECT * FROM {tableName}");
            AppendQueryWhere(sql, queryWhere);
            AppendOrderBy(sql, paging);
            sql.AppendFormat(" OFFSET {0} ROW FETCH NEXT {1} ROWS ONLY;", paging.Skip, paging.Size);

            //查询总数量语句
            sql.Append("SELECT CAST(COUNT(0) AS BIGINT) AS TotalCount FROM ");
            sql.Append(tableName);
            AppendQueryWhere(sql, queryWhere);

            return sql.ToString();
        }
    }
}
