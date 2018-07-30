using NetSql.Entities;
using NetSql.Pagination;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace NetSql
{
    /// <summary>
    /// 实体数据集
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public interface IDbSet<TEntity> where TEntity : Entity, new()
    {
        #region ==CRUD方法==

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

        /// <summary>
        /// 根据Lambda表达式查询单挑数据
        /// <para>Note：有多条时返回第一条</para>
        /// </summary>
        /// <param name="where"></param>
        /// <param name="sort">排序</param>
        /// <param name="transaction">事务</param>
        /// <returns></returns>
        Task<TEntity> GetAsync(Expression<Func<TEntity, bool>> where, ISort sort = null, IDbTransaction transaction = null);

        /// <summary>
        /// 查询实体列表
        /// </summary>
        /// <param name="whereExp"></param>
        /// <param name="sort"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        Task<IEnumerable<TEntity>> QueryAsync(Expression<Func<TEntity, bool>> whereExp = null, ISort sort = null, IDbTransaction transaction = null);

        /// <summary>
        /// 查询实体列表，返回部分字段
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="selectExp"></param>
        /// <param name="whereExp"></param>
        /// <param name="sort"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        Task<IEnumerable<TEntity>> QueryPartialFieldAsync<TResult>(Expression<Func<TEntity, TResult>> selectExp, Expression<Func<TEntity, bool>> whereExp = null, ISort sort = null, IDbTransaction transaction = null);

        /// <summary>
        /// 分页查询
        /// </summary>
        /// <param name="paging">分页</param>
        /// <param name="whereExp">查询条件</param>
        /// <param name="sort">排序</param>
        /// <param name="transaction">事务</param>
        /// <returns></returns>
        Task<IEnumerable<TEntity>> PaginationAsync(Paging paging, Expression<Func<TEntity, bool>> whereExp = null, ISort sort = null, IDbTransaction transaction = null);

        /// <summary>
        /// 分页查询，返回指定列
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="selectExp"></param>
        /// <param name="paging"></param>
        /// <param name="whereExp"></param>
        /// <param name="sort"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        Task<IEnumerable<TEntity>> PaginationAsync<TResult>(Expression<Func<TEntity, TResult>> selectExp, Paging paging, Expression<Func<TEntity, bool>> whereExp = null, ISort sort = null, IDbTransaction transaction = null);

        #endregion

        #region ==数据库操作方法==

        /// <summary>
        /// 执行一个命令并返回受影响的行数
        /// </summary>
        /// <param name="sql">sql语句</param>
        /// <param name="param">参数</param>
        /// <param name="transaction">事务</param>
        /// <param name="commandType">命令类型</param>
        /// <returns></returns>
        Task<int> ExecuteAsync(string sql, object param = null, IDbTransaction transaction = null, CommandType? commandType = null);

        /// <summary>
        /// 执行一个命令并返回单个值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql">sql语句</param>
        /// <param name="param">参数</param>
        /// <param name="transaction">事务</param>
        /// <param name="commandType">命令类型</param>
        /// <returns></returns>
        Task<T> ExecuteScalarAsync<T>(string sql, object param = null, IDbTransaction transaction = null, CommandType? commandType = null);

        /// <summary>
        /// 查询第一条数据，不存在返回默认值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql">sql语句</param>
        /// <param name="param">参数</param>
        /// <param name="transaction">事务</param>
        /// <param name="commandType">命令类型</param>
        /// <returns></returns>
        Task<T> QueryFirstOrDefaultAsync<T>(string sql, object param = null, IDbTransaction transaction = null, CommandType? commandType = null);

        /// <summary>
        /// 查询单条记录，不存在返回默认值，如果存在多条记录则抛出异常
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql">sql语句</param>
        /// <param name="param">参数</param>
        /// <param name="transaction">事务</param>
        /// <param name="commandType">命令类型</param>
        /// <returns></returns>
        Task<T> QuerySingleOrDefaultAsync<T>(string sql, object param = null, IDbTransaction transaction = null, CommandType? commandType = null);

        /// <summary>
        /// 查询数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql">sql语句</param>
        /// <param name="param">参数</param>
        /// <param name="transaction">事务</param>
        /// <param name="commandType">命令类型</param>
        /// <returns></returns>
        Task<IEnumerable<T>> QueryAsync<T>(string sql, object param = null, IDbTransaction transaction = null, CommandType? commandType = null);


        #endregion
    }
}