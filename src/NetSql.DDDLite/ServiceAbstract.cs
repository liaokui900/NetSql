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

        public Task<int> RemoveAsync(dynamic id)
        {
            return _repository.RemoveAsync(id);
        }

        public Task<int> UpdateAsync(TEntity entity)
        {
            return _repository.UpdateAsync(entity);
        }

        public Task<TEntity> GetAsync(dynamic id)
        {
            return _repository.GetAsync(id);
        }
    }
}
