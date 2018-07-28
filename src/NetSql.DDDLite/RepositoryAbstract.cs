using System;
using System.Collections.Generic;
using System.Data;
using System.Linq.Expressions;
using System.Threading.Tasks;
using NetSql.Entities;
using NetSql.Pagination;

namespace NetSql.DDDLite
{
    public abstract class RepositoryAbstract<TEntity> : IRepository<TEntity> where TEntity : Entity, new()
    {
        protected IDbSet<TEntity> Db { get; }

        protected RepositoryAbstract(IDbContext dbContext)
        {
            Db = dbContext.DbSet<TEntity>();
        }

        public Task<bool> AddAsync(TEntity entity, IDbTransaction transaction = null)
        {
            return Db.AddAsync(entity, transaction);
        }

        public Task<bool> BatchAddtAsync(IList<TEntity> entityList, IDbTransaction transaction = null)
        {
            return Db.BatchAddtAsync(entityList, transaction);
        }

        public Task<bool> BatchRemoveAsync<T>(IList<T> idList, IDbTransaction transaction = null)
        {
            return Db.BatchRemoveAsync(idList, transaction);
        }

        public Task<bool> BatchUpdateAsync(IList<TEntity> entityList, IDbTransaction transaction = null)
        {
            return Db.BatchUpdateAsync(entityList, transaction);
        }

        public Task<TEntity> GetAsync(dynamic id, IDbTransaction transaction = null)
        {
            return Db.GetAsync(id, transaction);
        }

        public Task<TEntity> GetAsync(Expression<Func<TEntity, bool>> where, ISort sort = null, IDbTransaction transaction = null)
        {
            return Db.GetAsync(where, sort, transaction);
        }

        public Task<IEnumerable<TEntity>> Query(Expression<Func<TEntity, bool>> whereExp, Paging paging, ISort sort = null, IDbTransaction transaction = null)
        {
            return Db.Query(whereExp, paging, sort, transaction);
        }

        public Task<IEnumerable<TEntity>> Query<TResult>(Expression<Func<TEntity, bool>> whereExp, Expression<Func<TEntity, TResult>> selectExp, Paging paging, ISort sort = null, IDbTransaction transaction = null)
        {
            return Db.Query(whereExp, selectExp, paging, sort, transaction);
        }

        public Task<int> RemoveAsync(dynamic id, IDbTransaction transaction = null)
        {
            return Db.RemoveAsync(id, transaction);
        }

        public Task<int> RemoveAsync(Expression<Func<TEntity, bool>> exp, IDbTransaction transaction = null)
        {
            return Db.RemoveAsync(exp, transaction);
        }

        public Task<int> UpdateAsync(TEntity entity, IDbTransaction transaction = null)
        {
            return Db.UpdateAsync(entity, transaction);
        }

        public Task<int> UpdateAsync(Expression<Func<TEntity, bool>> whereExp, Expression<Func<TEntity, TEntity>> updateEntity, IDbTransaction transaction = null)
        {
            return Db.UpdateAsync(whereExp, updateEntity, transaction);
        }
    }
}
