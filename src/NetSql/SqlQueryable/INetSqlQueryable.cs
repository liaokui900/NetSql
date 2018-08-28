using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using NetSql.Entities;
using NetSql.Enums;
using NetSql.Pagination;

namespace NetSql.SqlQueryable
{
    /// <summary>
    /// Sql构造器
    /// </summary>
    public interface INetSqlQueryable<TEntity> where TEntity : Entity, new()
    {
        /// <summary>
        /// 过滤
        /// </summary>
        /// <param name="expression">过滤条件</param>
        /// <returns></returns>
        INetSqlQueryable<TEntity> Where(Expression<Func<TEntity, bool>> expression);

        /// <summary>
        /// 过滤
        /// </summary>
        /// <param name="isAdd">是否添加</param>
        /// <param name="expression">条件</param>
        /// <returns></returns>
        INetSqlQueryable<TEntity> WhereIf(bool isAdd, Expression<Func<TEntity, bool>> expression);

        /// <summary>
        /// 排序
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="expression">列</param>
        /// <param name="sortType">排序规则</param>
        /// <returns></returns>
        INetSqlQueryable<TEntity> OrderBy<TKey>(Expression<Func<TEntity, TKey>> expression, SortType sortType = SortType.Asc);

        /// <summary>
        /// 排序
        /// </summary>
        /// <param name="sort"></param>
        /// <returns></returns>
        INetSqlQueryable<TEntity> OrderBy(Sort sort);

        /// <summary>
        /// 限制
        /// </summary>
        /// <param name="skip">跳过前几条数据</param>
        /// <param name="take">取前几条数据</param>
        /// <returns></returns>
        INetSqlQueryable<TEntity> Limit(int skip, int take);

        /// <summary>
        /// 查询指定列
        /// </summary>
        /// <returns></returns>
        INetSqlQueryable<TEntity> Select<TResult>(Expression<Func<TEntity, TResult>> expression);

        /// <summary>
        /// 获取最大值
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="expression"></param>
        /// <returns></returns>
        Task<TResult> Max<TResult>(Expression<Func<TEntity, TResult>> expression);

        /// <summary>
        /// 获取最小值
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="expression"></param>
        /// <returns></returns>
        Task<TResult> Min<TResult>(Expression<Func<TEntity, TResult>> expression);

        /// <summary>
        /// 查询数量
        /// </summary>
        /// <returns></returns>
        Task<long> Count();

        /// <summary>
        /// 判断是否存在
        /// </summary>
        /// <returns></returns>
        Task<bool> Exists();

        /// <summary>
        /// 查询第一条数据
        /// </summary>
        /// <returns></returns>
        Task<TEntity> First();

        /// <summary>
        /// 删除
        /// </summary>
        /// <returns></returns>
        Task<bool> Delete();

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        Task<bool> Update(Expression<Func<TEntity, TEntity>> expression);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        Task<List<TEntity>> ToList();

        /// <summary>
        /// 输出Sql语句
        /// </summary>
        /// <returns></returns>
        string ToSql();
    }
}
