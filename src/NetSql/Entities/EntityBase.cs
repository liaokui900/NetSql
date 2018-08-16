using NetSql.Mapper;

namespace NetSql.Entities
{
    /// <summary>
    /// 实体基类
    /// </summary>
    public class EntityBase<TKey> : Entity
    {
        public virtual TKey Id { get; set; }
    }

    /// <summary>
    /// 实体基类
    /// </summary>
    public class EntityBase : EntityBase<int>
    {

    }
}
