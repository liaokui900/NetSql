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
    public abstract class NetSqlDbContext : IDbContext
    {
        #region ==属性==

        /// <summary>
        /// 上下文配置项
        /// </summary>
        public DbContextOptionsAbstract Options { get; private set; }

        #endregion

        #region ==公共方法==

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IDbConnection OpenConnection()
        {
            //使用反射创建
            var con = Options.DbConnection;
            con.Open();
            return con;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IDbTransaction BeginTransaction()
        {
            return OpenConnection().BeginTransaction();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <returns></returns>
        public IDbSet<TEntity> DbSet<TEntity>() where TEntity : Entity, new()
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

        #region ==抽象方法==

        /// <summary>
        /// 
        /// </summary>
        /// <param name="builder"></param>
        protected abstract void OnConfiguring(DbContextOptionsBuilder builder);

        #endregion

        #region ==构造函数==

        /// <summary>
        /// 
        /// </summary>
        protected NetSqlDbContext()
        {
            Config();

            InitializeSets();
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

        /// <summary>
        /// 加载配置
        /// </summary>
        private void Config()
        {
            var optionsBuilder = new DbContextOptionsBuilder();

            OnConfiguring(optionsBuilder);

            Options = optionsBuilder.Builder();
        }

        #endregion
    }
}