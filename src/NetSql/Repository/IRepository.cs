using System;
using System.Collections.Generic;
using System.Data;
using System.Linq.Expressions;
using System.Threading.Tasks;
using NetSql.Entities;
using NetSql.Pagination;

namespace NetSql.Repository
{
    /// <summary>
    /// 泛型仓储接口
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public interface IRepository<TEntity> where TEntity : Entity, new()
    {
        /// <summary>
        /// 打开一个事务
        /// </summary>
        /// <returns></returns>
        IDbTransaction BeginTransaction();

        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="entity">实体</param>
        /// <param name="transaction">事务</param>
        /// <returns></returns>
        Task<bool> AddAsync(TEntity entity, IDbTransaction transaction = null);

        /// <summary>
        /// 批量新增
        /// </summary>
        /// <param name="list"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        Task<bool> AddAsync(List<TEntity> list, IDbTransaction transaction = null);

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="id"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        Task<bool> DeleteAsync(dynamic id, IDbTransaction transaction = null);

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="entity">实体</param>
        /// <param name="transaction">事务</param>
        /// <returns></returns>
        Task<bool> UpdateAsync(TEntity entity, IDbTransaction transaction = null);

        /// <summary>
        /// 根据主键查询
        /// </summary>
        /// <param name="id"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        Task<TEntity> GetAsync(dynamic id, IDbTransaction transaction = null);

        /// <summary>
        /// 分页查询
        /// </summary>
        /// <param name="paging">分页</param>
        /// <param name="where">过滤条件</param>
        /// <param name="transaction">事务</param>
        /// <returns></returns>
        Task<List<TEntity>> PaginationAsync(Paging paging, Expression<Func<TEntity, bool>> where = null, IDbTransaction transaction = null);
    }
}
