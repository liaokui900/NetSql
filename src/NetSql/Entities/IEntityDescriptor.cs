using System;
using System.Collections.Generic;
using NetSql.Enums;

namespace NetSql.Entities
{
    internal interface IEntityDescriptor
    {
        #region ==属性==

        /// <summary>
        /// 实体类型
        /// </summary>
        Type EntityType { get; }

        /// <summary>
        /// 表名称
        /// </summary>
        string TableName { get; }

        /// <summary>
        /// 属性列表
        /// </summary>
        List<ColumnDescriptor> Columns { get; }

        /// <summary>
        /// 主键类型
        /// </summary>
        PrimaryKeyType PrimaryKeyType { get; }

        /// <summary>
        /// 主键属性
        /// </summary>
        ColumnDescriptor PrimaryKey { get; }

        #endregion
    }
}
