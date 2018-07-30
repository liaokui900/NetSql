using System.Data;
using NetSql.SqlAdapter;
using DbType = NetSql.Enums.DbType;

namespace NetSql
{
    /// <summary>
    /// 数据库上下文配置项
    /// </summary>
    public interface IDbContextOptions
    {
        /// <summary>
        /// 数据库连接字符串
        /// </summary>
        string ConnectionString { get; }

        /// <summary>
        /// 数据库适配器
        /// </summary>
        ISqlAdapter SqlAdapter { get; }

        /// <summary>
        /// 打开一个连接
        /// </summary>
        /// <returns></returns>
        IDbConnection DbConnection { get; }

        /// <summary>
        /// 数据库类型
        /// </summary>
        DbType DbType { get; }
    }
}
