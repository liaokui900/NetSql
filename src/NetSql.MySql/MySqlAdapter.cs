using System;
using System.Collections.Generic;
using System.Text;
using NetSql.Pagination;
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

        public override string GeneratePagingSql(string tableName, string queryWhere, Paging paging, ISort sort = null, string columns = null)
        {
            if (string.IsNullOrWhiteSpace(columns))
                columns = "*";

            var sb = new StringBuilder($"SELECT SQL_CALC_FOUND_ROWS {columns} FROM {tableName}");
            AppendQueryWhere(sb, queryWhere);
            if (sort != null)
            {
                sb.Append(sort.Builder());
            }
            sb.AppendFormat(" LIMIT {0},{1};SELECT CAST(FOUND_ROWS() as SIGNED) AS TotalCount;", paging.Skip, paging.Size);
            return sb.ToString();
        }
    }
}
