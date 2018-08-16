using System;
using System.Data;
using System.Linq;
using System.Reflection;
using NetSql.Entities;
using NetSql.Internal;

namespace NetSql
{
    /// <summary>
    /// 数据库上下文
    /// </summary>
    public abstract class DbContext : IDbContext
    {
        #region ==属性==

        /// <summary>
        /// 上下文配置项
        /// </summary>
        public IDbContextOptions Options { get; }

        /// <summary>
        /// 获取一个数据库连接
        /// </summary>
        public IDbConnection DbConnection => Options.DbConnection;

        #endregion

        #region ==构造函数==

        /// <summary>
        /// 
        /// </summary>
        protected DbContext(IDbContextOptions options)
        {
            Check.NotNull(options, nameof(options), "数据库配置项为空");
            Check.NotNull(options.ConnectionString, nameof(options.ConnectionString), "数据库连接字符串为空");

            Options = options;

            InitializeSets();
        }

        #endregion

        #region ==公共方法==

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IDbTransaction BeginTransaction()
        {
            var con = DbConnection;
            con.Open();
            return con.BeginTransaction();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <returns></returns>
        public IDbSet<TEntity> Set<TEntity>() where TEntity : Entity, new()
        {
            var properties = GetType().GetRuntimeProperties()
                .Where(p => !p.IsStatic()
                            && !p.GetIndexParameters().Any()
                            && p.PropertyType.GetTypeInfo().IsGenericType
                            && (p.PropertyType.GetGenericTypeDefinition() == typeof(IDbSet<>) || p.PropertyType.GetGenericTypeDefinition() == typeof(DbSet<>)));

            var entityType = typeof(TEntity);
            foreach (var propertyInfo in properties)
            {
                if (entityType == propertyInfo.PropertyType.GenericTypeArguments.Single())
                {
                    return (IDbSet<TEntity>)propertyInfo.GetValue(this);
                }
            }

            throw new NullReferenceException("未找到指定的实体数据集");
        }

        #endregion

        #region ==私有方法==

        /// <summary>
        /// 初始化IDbSet
        /// </summary>
        private void InitializeSets()
        {
            var properties = GetType().GetRuntimeProperties()
                .Where(p => !p.IsStatic()
                            && !p.GetIndexParameters().Any()
                            && p.PropertyType.GetTypeInfo().IsGenericType
                            && (p.PropertyType.GetGenericTypeDefinition() == typeof(IDbSet<>) || p.PropertyType.GetGenericTypeDefinition() == typeof(DbSet<>)));

            foreach (var propertyInfo in properties)
            {
                var entityType = propertyInfo.PropertyType.GenericTypeArguments.Single();
                var dbSetType = typeof(DbSet<>).MakeGenericType(entityType);
                var dbSet = Activator.CreateInstance(dbSetType, Options.SqlAdapter, this);
                propertyInfo.SetValue(this, dbSet);
            }
        }

        #endregion
    }
}