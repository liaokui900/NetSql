using System.Text;
using NetSql.Internal;
using NetSql.SqlAdapter;

namespace NetSql.MySql
{
    internal class MySqlAdapter : SqlAdapterAbstract
    {
        /// <summary>
        /// 左引号
        /// </summary>
        public override char LeftQuote => '`';

        /// <summary>
        /// 右引号
        /// </summary>
        public override char RightQuote => '`';

        /// <summary>
        /// 获取最后新增ID语句
        /// </summary>
        public override string IdentitySql => "SELECT LAST_INSERT_ID() ID;";

        public override string GeneratePagingSql(string tableName, string queryWhere, int skip, int size, string sort = null, string columns = null)
        {
            if (columns.IsNull())
                columns = "*";

            var sb = new StringBuilder($"SELECT {columns} FROM {AppendQuote(tableName)} ");
            AppendQueryWhere(sb, queryWhere);
            if (sort.NotNull())
            {
                sb.AppendFormat(" {0} ", sort);
            }
            sb.AppendFormat(" LIMIT {0},{1};", skip, size);
            return sb.ToString();
        }
    }
}
