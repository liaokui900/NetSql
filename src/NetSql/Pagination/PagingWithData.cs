using System.Collections.Generic;

namespace NetSql.Pagination
{
    /// <summary>
    /// 通用泛型分页类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Paging<T> : Paging
    {
        /// <summary>
        /// 查询的数据列表
        /// </summary>
        public IEnumerable<T> DataList { get; set; }
    }
}
