using System.Text;
using NetSql.Internal;

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
        /// <param name="size"></param>
        /// <param name="sort">排序</param>
        /// <param name="columns"></param>
        /// <param name="skip"></param>
        /// <returns></returns>
        public override string GeneratePagingSql(string tableName, string queryWhere, int skip, int size, string sort = null, string columns = null)
        {
            if (columns.IsNull())
                columns = "*";

            var sql = new StringBuilder($"SELECT {columns} FROM {AppendQuote(tableName)} ");
            AppendQueryWhere(sql, queryWhere);
            if (sort.NotNull())
            {
                sql.AppendFormat(" {0} ", sort);
            }

            sql.AppendFormat(" OFFSET {0} ROW FETCH NEXT {1} ROWS ONLY;", skip, size);

            return sql.ToString();
        }
    }
}
