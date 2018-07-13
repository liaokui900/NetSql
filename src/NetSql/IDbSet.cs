using NetSql.Entities;

namespace NetSql
{
    public interface IDbSet<TEntity> where TEntity : Entity, new()
    {
        bool Insert(TEntity entity);
    }
}
