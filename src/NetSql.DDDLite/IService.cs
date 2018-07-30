using System;
using System.Collections.Generic;
using System.Data;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using NetSql.Entities;
using NetSql.Pagination;

namespace NetSql.DDDLite
{
    public interface IService<TEntity> where TEntity : Entity, new()
    {
        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="entity">实体</param>
        /// <returns></returns>
        Task<bool> AddAsync(TEntity entity);

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns></returns>
        Task<int> RemoveAsync(dynamic id);

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="entity">实体</param>
        /// <returns></returns>
        Task<int> UpdateAsync(TEntity entity);

        /// <summary>
        /// 查询单个实体
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns></returns>
        Task<TEntity> GetAsync(dynamic id);
    }
}
