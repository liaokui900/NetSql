using NetSql.Entities;
using NetSql.SqlAdapter;

namespace NetSql
{
    internal class DbSet<TEntity> : IDbSet<TEntity> where TEntity : Entity, new()
    {
        private readonly ISqlAdapter _sqlAdapter;

        public DbSet(ISqlAdapter sqlAdapter)
        {
            _sqlAdapter = sqlAdapter;
        }

        public bool Insert(TEntity entity)
        {
            return true;
        }
    }
}
