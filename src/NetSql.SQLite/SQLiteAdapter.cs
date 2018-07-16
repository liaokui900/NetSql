using System.Text;
using NetSql.Pagination;
using NetSql.SqlAdapter;

namespace NetSql.SQLite
{
    internal class SQLiteAdapter : SqlAdapterAbstract
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
        public override string IdentitySql => "SELECT LAST_INSERT_ROWID() ID;";

        /// <summary>
        /// 获取分页语句
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <param name="queryWhere">查询条件</param>
        /// <param name="paging">分页类</param>
        /// <returns></returns>
        public override string GeneratePagingSql(string tableName, string queryWhere, Paging paging)
        {
            var sql = new StringBuilder("SELECT * FROM ");
            sql.Append(tableName);
            AppendQueryWhere(sql, queryWhere);
            AppendOrderBy(sql, paging);
            sql.AppendFormat(" LIMIT {0} OFFSET {1};", paging.Size, paging.Skip);

            //总数量
            sql.Append("SELECT COUNT(0) AS TotalCount FROM ");
            sql.Append(tableName);
            AppendQueryWhere(sql, queryWhere);

            return sql.ToString();
        }
    }
}
