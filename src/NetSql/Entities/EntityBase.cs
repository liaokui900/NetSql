namespace NetSql.Entities
{
    /// <summary>
    /// 实体基类
    /// </summary>
    /// <typeparam name="T">主键类型</typeparam>
    public class EntityBase<T> : Entity
    {
        public virtual T Id { get; set; }
    }

    /// <summary>
    /// 实体基类
    /// </summary>
    public class EntityBase : EntityBase<int>
    {
    }
}
