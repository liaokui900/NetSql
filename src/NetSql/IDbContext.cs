using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using NetSql.Entities;

namespace NetSql
{
    /// <summary>
    /// 数据库上下文
    /// </summary>
    public interface IDbContext
    {
        /// <summary>
        /// 打开一个数据库连接
        /// </summary>
        /// <returns></returns>
        IDbConnection OpenConnection();
        /// <summary>
        /// 打开一个事务
        /// </summary>
        /// <returns></returns>
        IDbTransaction BeginTransaction();

        /// <summary>
        /// 获取实体数据集
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <returns></returns>
        IDbSet<TEntity> DbSet<TEntity>() where TEntity : Entity, new();
    }
}
