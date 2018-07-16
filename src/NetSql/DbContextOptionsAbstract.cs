using System;
using System.Data;
using NetSql.SqlAdapter;

namespace NetSql
{
    /// <summary>
    /// 数据库上下文配置项
    /// </summary>
    public abstract class DbContextOptionsAbstract
    {
        /// <summary>
        /// 数据库连接字符串
        /// </summary>
        public string ConnectionString { get; }

        /// <summary>
        /// 数据库适配器
        /// </summary>
        public ISqlAdapter SqlAdapter { get; }

        /// <summary>
        /// 打开一个连接
        /// </summary>
        /// <returns></returns>
        public abstract IDbConnection DbConnection { get; }

        /// <summary>
        /// 数据库类型
        /// </summary>
        public Enums.DbType DbType { get; set; }

        protected DbContextOptionsAbstract(string connectionString, ISqlAdapter sqlAdapter, Enums.DbType dbType)
        {
            ConnectionString = connectionString;
            SqlAdapter = sqlAdapter;
            DbType = dbType;
        }
    }
}
