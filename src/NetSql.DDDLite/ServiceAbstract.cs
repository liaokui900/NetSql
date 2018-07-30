using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using NetSql.Entities;
using NetSql.Pagination;

namespace NetSql.DDDLite
{
    public abstract class ServiceAbstract<TEntity> : IService<TEntity> where TEntity : Entity, new()
    {
        private readonly RepositoryAbstract<TEntity> _repository;

        protected ServiceAbstract(RepositoryAbstract<TEntity> repository)
        {
            _repository = repository;
        }

        public Task<bool> AddAsync(TEntity entity)
        {
            return _repository.AddAsync(entity);
        }

        public Task<bool> BatchAddtAsync(IList<TEntity> entityList)
        {
            return _repository.BatchAddtAsync(entityList);
        }

        public Task<int> RemoveAsync(dynamic id)
        {
            return _repository.RemoveAsync(id);
        }

        public Task<int> RemoveAsync(Expression<Func<TEntity, bool>> exp)
        {
            return _repository.RemoveAsync(exp);
        }

        public Task<bool> BatchRemoveAsync<T>(IList<T> idList)
        {
            return _repository.BatchRemoveAsync(idList);
        }

        public Task<int> UpdateAsync(TEntity entity)
        {
            return _repository.UpdateAsync(entity);
        }

        public Task<int> UpdateAsync(Expression<Func<TEntity, bool>> whereExp, Expression<Func<TEntity, TEntity>> updateEntity)
        {
            return _repository.UpdateAsync(whereExp, updateEntity);
        }

        public Task<bool> BatchUpdateAsync(IList<TEntity> entityList)
        {
            return _repository.BatchUpdateAsync(entityList);
        }

        public Task<TEntity> GetAsync(dynamic id)
        {
            return _repository.GetAsync(id);
        }

        public Task<TEntity> GetAsync(Expression<Func<TEntity, bool>> @where, ISort sort = null)
        {
            return _repository.GetAsync(where, sort);
        }

        public Task<IEnumerable<TEntity>> Query(Expression<Func<TEntity, bool>> whereExp, Paging paging, ISort sort = null)
        {
            return _repository.Query(whereExp, paging, sort);
        }

        public Task<IEnumerable<TEntity>> Query<TResult>(Expression<Func<TEntity, bool>> whereExp, Expression<Func<TEntity, TResult>> selectExp, Paging paging, ISort sort = null)
        {
            return _repository.Query(whereExp, selectExp, paging, sort);
        }
    }
}
