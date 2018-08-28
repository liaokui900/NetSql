using System.Text;
using NetSql.Internal;
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
        /// <param name="skip"></param>
        /// <param name="size"></param>
        /// <param name="sort">排序</param>
        /// <returns></returns>
        public override string GeneratePagingSql(string tableName, string queryWhere, int skip, int size, string sort = null, string columns = null)
        {
            if (columns.IsNull())
                columns = "*";

            var sql = new StringBuilder($"SELECT {columns} FROM ");
            sql.AppendFormat(" {0} ", AppendQuote(tableName));
            AppendQueryWhere(sql, queryWhere);
            if (sort.NotNull())
            {
                sql.AppendFormat(" {0} ", sort);
            }

            sql.AppendFormat(" LIMIT {0} OFFSET {1};", size, skip);

            return sql.ToString();
        }
    }
}
