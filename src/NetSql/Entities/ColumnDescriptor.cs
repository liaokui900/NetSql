using System;
using System.Reflection;

namespace NetSql.Entities
{
    internal class ColumnDescriptor
    {
        /// <summary>
        /// 列名
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 属性类型
        /// </summary>
        public Type PropertyType { get; set; }

        /// <summary>
        /// 属性信息
        /// </summary>
        public PropertyInfo PropertyInfo { get; set; }

        /// <summary>
        /// 是否主键
        /// </summary>
        public bool IsPrimaryKey { get; set; }

        /// <summary>
        /// 获取值
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="entity">实体</param>
        /// <returns></returns>
        public object GetValue<TEntity>(TEntity entity) where TEntity : Entity, new()
        {
            return PropertyInfo.GetValue(entity);
        }

        /// <summary>
        /// 给实体属性设置值
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="entity"></param>
        /// <param name="value"></param>
        public void SetValue<TEntity>(TEntity entity, object value) where TEntity : Entity, new()
        {
            PropertyInfo.SetValue(entity, value);
        }
    }
}
