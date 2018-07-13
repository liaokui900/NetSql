using System.Data;
using NetSql.SqlAdapter;

namespace NetSql
{
    /// <summary>
    /// NetSql上下文接口
    /// </summary>
    public interface INetSqlContext
    {
        /// <summary>
        /// 数据库适配器
        /// </summary>
        ISqlAdapter SqlAdapter { get; }

        /// <summary>
        /// 数据库连接字符串
        /// </summary>
        string ConnectionString { get; }

    }
}
