using System;
using System.Data;
using System.Linq;
using System.Reflection;
using NetSql.Internal;
using NetSql.SqlAdapter;

namespace NetSql
{
    public abstract class NetSqlContextAbstract : INetSqlContext
    {
        public ISqlAdapter SqlAdapter { get; }
        public string ConnectionString { get; }

        #region ==构造函数==

        protected NetSqlContextAbstract(string connectionString, ISqlAdapter sqlAdapter)
        {
            Check.NotNull(connectionString, nameof(connectionString));
            Check.NotNull(sqlAdapter, nameof(sqlAdapter));

            ConnectionString = connectionString;
            SqlAdapter = sqlAdapter;

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
                            && (p.PropertyType == typeof(IDbSet<>) || p.PropertyType == typeof(DbSet<>)));

            foreach (var propertyInfo in properties)
            {
                var entityType = propertyInfo.PropertyType.GenericTypeArguments.Single();
                var dbSetType = typeof(DbSet<>).MakeGenericType(entityType);
                var dbSet = Activator.CreateInstance(dbSetType);
                propertyInfo.SetValue(this, dbSet);
                Console.WriteLine(propertyInfo.Name);
            }
        }

        #endregion
    }
}