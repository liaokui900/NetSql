using System;

namespace NetSql.Mapper
{
    /// <summary>
    /// 忽略属性，标识该实体属性不在表中映射列
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class IgnoreAttribute : Attribute
    {
    }
}
