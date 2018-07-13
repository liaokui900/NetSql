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
    internal class EntityDescriptor<TEntity> where TEntity : Entity, new()
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
        public List<PropertyInfo> Properties { get; private set; }

        /// <summary>
        /// 主键类型
        /// </summary>
        public PrimaryKeyType PrimaryKeyType { get; set; }

        /// <summary>
        /// 主键属性
        /// </summary>
        public PropertyInfo PrimaryKey { get; set; }

        #endregion

        #region ==构造器==

        public EntityDescriptor()
        {
            EntityType = typeof(TEntity);

            SetTableName();

            SetProperties();

            SetPrimaryKeyType();
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
        private void SetProperties()
        {
            var ignoreName = typeof(IgnoreAttribute).Name;
            Properties = EntityType.GetProperties().Where(p => !p.PropertyType.IsGenericType && Type.GetTypeCode(p.PropertyType) != TypeCode.Object && !p.GetCustomAttributes().Any(attr => attr.GetType().Name.Equals(ignoreName))).ToList();
        }

        /// <summary>
        /// 设置主键类型
        /// </summary>
        private void SetPrimaryKeyType()
        {
            PrimaryKeyType = PrimaryKeyType.NoPrimaryKey;
            PrimaryKey = Properties.FirstOrDefault(m => m.Name.Equals("Id"));
            if (PrimaryKey != null)
            {
                if (PrimaryKey.PropertyType == typeof(int))
                {
                    PrimaryKeyType = PrimaryKeyType.Int;
                }
                else if (PrimaryKey.PropertyType == typeof(long))
                {
                    PrimaryKeyType = PrimaryKeyType.String;
                }
                else if (PrimaryKey.PropertyType == typeof(string))
                {
                    PrimaryKeyType = PrimaryKeyType.String;
                }
                else
                {
                    throw new ArgumentException("无效的主键类型", nameof(PrimaryKey.PropertyType));
                }
            }
        }

        #endregion
    }
}
