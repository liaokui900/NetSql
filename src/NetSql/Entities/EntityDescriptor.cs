using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NetSql.Enums;
using NetSql.Mapper;

namespace NetSql.Entities
{
    /// <summary>
    /// 实体描述
    /// </summary>
    internal class EntityDescriptor<TEntity> : IEntityDescriptor where TEntity : Entity, new()
    {
        #region ==属性==

        /// <summary>
        /// 实体类型
        /// </summary>
        public Type EntityType { get; }

        /// <summary>
        /// 表名称
        /// </summary>
        public string TableName { get; private set; }

        /// <summary>
        /// 属性列表
        /// </summary>
        public List<ColumnDescriptor> Columns { get; private set; }

        /// <summary>
        /// 主键类型
        /// </summary>
        public PrimaryKeyType PrimaryKeyType { get; private set; }

        /// <summary>
        /// 主键属性
        /// </summary>
        public ColumnDescriptor PrimaryKey { get; private set; }

        #endregion

        #region ==构造器==

        public EntityDescriptor()
        {
            EntityType = typeof(TEntity);

            SetTableName();

            SetColumns();
        }

        #endregion

        #region ==私有方法==


        /// <summary>
        /// 设置表名
        /// </summary>
        private void SetTableName()
        {
            var tableArr = EntityType.GetCustomAttribute<TableAttribute>(false);
            TableName = tableArr != null ? tableArr.Name : EntityType.Name;
        }

        /// <summary>
        /// 设置属性列表
        /// </summary>
        private void SetColumns()
        {
            Columns = new List<ColumnDescriptor>();

            //加载属性列表
            var properties = EntityType.GetProperties().Where(p =>
                !p.PropertyType.IsGenericType
                && Type.GetTypeCode(p.PropertyType) != TypeCode.Object
                && p.GetCustomAttributes().All(attr => attr.GetType() != typeof(IgnoreAttribute))).ToList();

            foreach (var p in properties)
            {
                Columns.Add(Property2Column(p));
            }
        }

        /// <summary>
        /// 设置主键类型
        /// </summary>
        private ColumnDescriptor Property2Column(PropertyInfo property)
        {
            var columnAttribute = property.GetCustomAttribute<ColumnAttribute>();

            var col = new ColumnDescriptor
            {
                Name = columnAttribute != null ? columnAttribute.Name : property.Name,
                PropertyInfo = property,
                PropertyType = property.PropertyType
            };

            var isPrimaryKey = property.GetCustomAttributes().Any(attr => attr.GetType() == typeof(KeyAttribute));

            if (!isPrimaryKey)
            {
                isPrimaryKey = property.Name.Equals("Id", StringComparison.OrdinalIgnoreCase);
            }

            if (isPrimaryKey)
            {
                if (property.PropertyType == typeof(int))
                {
                    PrimaryKeyType = PrimaryKeyType.Int;
                }
                else if (property.PropertyType == typeof(long))
                {
                    PrimaryKeyType = PrimaryKeyType.Long;
                }
                else if (property.PropertyType == typeof(string))
                {
                    PrimaryKeyType = PrimaryKeyType.String;
                }
                else
                {
                    throw new ArgumentException("无效的主键类型", nameof(PrimaryKey.PropertyType));
                }

                col.IsPrimaryKey = true;

                PrimaryKey = col;
            }

            return col;
        }

        #endregion
    }
}
