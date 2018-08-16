using System;
using System.Text;
using NetSql.Pagination;

namespace NetSql.SqlAdapter
{
    public abstract class SqlAdapterAbstract : ISqlAdapter
    {
        public virtual char LeftQuote => '"';

        /// <summary>
        /// 右引号
        /// </summary>
        public virtual char RightQuote => '"';

        /// <summary>
        /// 参数前缀符号
        /// </summary>
        public virtual char ParameterPrefix => '@';

        /// <summary>
        /// 获取最后新增ID语句
        /// </summary>
        public abstract string IdentitySql { get; }

        /// <summary>
        /// 附加引号
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public string AppendQuote(string value)
        {
            return $"{LeftQuote}{value.Trim()}{RightQuote}";
        }

        /// <summary>
        /// 附加引号
        /// </summary>
        /// <param name="sb"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public void AppendQuote(StringBuilder sb, string value)
        {
            sb.Append(AppendQuote(value));
        }

        /// <summary>
        /// 附加参数
        /// </summary>
        /// <param name="parameterName">参数名</param>
        /// <returns></returns>
        public string AppendParameter(string parameterName)
        {
            return $"{ParameterPrefix}{parameterName}";
        }

        /// <summary>
        /// 附加参数
        /// </summary>
        /// <param name="sb"></param>
        /// <param name="parameterName">参数名</param>
        /// <returns></returns>
        public void AppendParameter(StringBuilder sb, string parameterName)
        {
            sb.Append(AppendParameter(parameterName));
        }

        public string AppendParameterWithValue(string parameterName)
        {
            return $"{AppendQuote(parameterName)}={AppendParameter(parameterName)}";
        }

        public void AppendParameterWithValue(StringBuilder sb, string parameterName)
        {
            sb.Append(AppendParameterWithValue(parameterName));
        }

        /// <summary>
        /// 附加查询条件
        /// </summary>
        /// <param name="queryWhere">查询条件</param>
        public string AppendQueryWhere(string queryWhere)
        {
            if (!string.IsNullOrWhiteSpace(queryWhere))
            {
                if (queryWhere.Trim().StartsWith("where", StringComparison.OrdinalIgnoreCase))
                    return queryWhere;

                if (queryWhere.Trim().StartsWith("and", StringComparison.OrdinalIgnoreCase))
                    queryWhere = "1=1 AND " + queryWhere;

                return $"WHERE {queryWhere}";
            }

            return "";
        }
        public void AppendQueryWhere(StringBuilder sb, string queryWhere)
        {
            if (!string.IsNullOrWhiteSpace(queryWhere))
            {
                if (!queryWhere.Trim().StartsWith("where", StringComparison.OrdinalIgnoreCase))
                    sb.Append(" WHERE ");

                sb.Append(queryWhere);
            }
        }
        /// <summary>
        /// 生成分页语句
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <param name="queryWhere">查询条件</param>
        /// <param name="skip">跳过数量</param>
        /// <param name="size">查询数量</param>
        /// <param name="sort">排序</param>
        /// <param name="columns">查询指定列</param>
        /// <returns></returns>
        public abstract string GeneratePagingSql(string tableName, string queryWhere, int skip, int size, string sort = null, string columns = null);
    }
}
