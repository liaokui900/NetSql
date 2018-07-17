using System;
using System.Collections.Generic;
using System.Data;
using System.Linq.Expressions;
using System.Threading.Tasks;
using NetSql.Pagination;

namespace NetSql
{
    public interface IDbSet<TEntity> where TEntity : new()
    {
        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="entity">实体</param>
        /// <param name="transaction">事务</param>
        /// <returns></returns>
        Task<bool> AddAsync(TEntity entity, IDbTransaction transaction = null);

        /// <summary>
        /// 批量添加
        /// </summary>
        /// <param name="entityList">实体列表</param>
        /// <param name="transaction">事务</param>
        /// <returns></returns>
        Task<bool> BatchAddtAsync(IList<TEntity> entityList, IDbTransaction transaction = null);

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="id">主键</param>
        /// <param name="transaction">事务</param>
        /// <returns></returns>
        Task<int> RemoveAsync(dynamic id, IDbTransaction transaction = null);

        /// <summary>
        /// 根据表达式删除
        /// </summary>
        /// <param name="exp">查询条件</param>
        /// <param name="transaction">事务</param>
        /// <returns></returns>
        Task<int> RemoveAsync(Expression<Func<TEntity, bool>> exp, IDbTransaction transaction = null);

        /// <summary>
        /// 批量删除
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="idList">主键列表</param>
        /// <param name="transaction">事务</param>
        /// <returns></returns>
        Task<bool> BatchRemoveAsync<T>(IList<T> idList, IDbTransaction transaction = null);


        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="entity">实体</param>
        /// <param name="transaction">事务</param>
        /// <returns></returns>
        Task<int> UpdateAsync(TEntity entity, IDbTransaction transaction = null);

        /// <summary>
        /// 根据表达式更新实体
        /// </summary>
        /// <param name="whereExp"></param>
        /// <param name="updateEntity"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        Task<int> UpdateAsync(Expression<Func<TEntity, bool>> whereExp, Expression<Func<TEntity, TEntity>> updateEntity, IDbTransaction transaction = null);

        /// <summary>
        /// 批量更新
        /// </summary>
        /// <param name="entityList"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        Task<bool> BatchUpdateAsync(IList<TEntity> entityList, IDbTransaction transaction = null);

        /// <summary>
        /// 查询单个实体
        /// </summary>
        /// <param name="id">主键</param>
        /// <param name="transaction">事务</param>
        /// <returns></returns>
        Task<TEntity> GetAsync(dynamic id, IDbTransaction transaction = null);

        Task<IEnumerable<TEntity>> Find(Expression<Func<TEntity, bool>> whereExp, Paging paging, IDbTransaction transaction = null);
    }
}
