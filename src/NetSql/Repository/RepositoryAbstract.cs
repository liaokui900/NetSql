using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using NetSql.Entities;
using NetSql.Pagination;
using NetSql.SqlQueryable;

namespace NetSql.Repository
{
    /// <summary>
    /// 仓储抽象类
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public abstract class RepositoryAbstract<TEntity> : IRepository<TEntity> where TEntity : Entity, new()
    {
        protected readonly IDbSet<TEntity> Db;
        protected readonly IDbContext _dbContext;

        protected RepositoryAbstract(IDbContext dbContext)
        {
            _dbContext = dbContext;
            Db = dbContext.Set<TEntity>();
        }

        public async Task<bool> ExistsAsync(Expression<Func<TEntity, bool>> where, IDbTransaction transaction = null)
        {
            return await Db.Find(where).Count() > 0;
        }

        public virtual Task<bool> AddAsync(TEntity entity, IDbTransaction transaction = null)
        {
            if (entity == null)
                return Task.FromResult(false);

            return Db.InsertAsync(entity, transaction);
        }

        public virtual Task<bool> AddAsync(List<TEntity> list, IDbTransaction transaction = null)
        {
            if (list == null || !list.Any())
                return Task.FromResult(false);

            if (transaction == null)
                transaction = _dbContext.BeginTransaction();

            try
            {
                foreach (var enitty in list)
                {
                    AddAsync(enitty, transaction);
                }

                transaction.Commit();
                return Task.FromResult(true);
            }
            catch
            {
                transaction.Rollback();
                transaction.Connection.Close();
                throw;
            }
        }

        public virtual Task<bool> DeleteAsync(dynamic id, IDbTransaction transaction = null)
        {
            return Db.DeleteAsync(id, transaction);
        }

        public virtual Task<bool> UpdateAsync(TEntity entity, IDbTransaction transaction = null)
        {
            return Db.UpdateAsync(entity, transaction);
        }

        public virtual Task<TEntity> GetAsync(dynamic id, IDbTransaction transaction = null)
        {
            return Db.GetAsync(id, transaction);
        }

        public Task<TEntity> GetAsync(Expression<Func<TEntity, bool>> @where, IDbTransaction transaction = null)
        {
            return Db.Find(where).FirstAsync();
        }

        public virtual Task<List<TEntity>> PaginationAsync(Paging paging = null, Expression<Func<TEntity, bool>> where = null, IDbTransaction transaction = null)
        {
            var query = Db.Find(where);

            return PaginationAsync(paging, query, transaction);
        }

        protected async Task<List<TEntity>> PaginationAsync(Paging paging = null, INetSqlQueryable<TEntity> query = null, IDbTransaction transaction = null)
        {
            if (paging == null)
                paging = new Paging();

            if (query == null)
                query = Db.Find();

            query.Limit(paging.Skip, paging.Size);

            //排序
            foreach (var sort in paging.OrderBy)
            {
                query.OrderBy(sort);
            }

            var count = query.Count();
            var list = query.ToListAsync();

            paging.TotalCount = await count;
            return await list;
        }

    }
}
