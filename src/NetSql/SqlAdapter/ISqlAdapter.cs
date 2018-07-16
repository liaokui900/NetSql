using NetSql.Pagination;
using System.Text;

namespace NetSql.SqlAdapter
{
    /// <summary>
    /// 数据库适配器接口
    /// </summary>
    public interface ISqlAdapter
    {
        #region ==属性==

        /// <summary>
        /// 左引号
        /// </summary>
        char LeftQuote { get; }

        /// <summary>
        /// 右引号
        /// </summary>
        char RightQuote { get; }

        /// <summary>
        /// 参数前缀符号
        /// </summary>
        char ParameterPrefix { get; }

        /// <summary>
        /// 获取新增ID的SQL语句
        /// </summary>
        string IdentitySql { get; }

        #endregion

        #region ==方法==

        /// <summary>
        /// 给定的值附加引号
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        string AppendQuote(string value);

        /// <summary>
        /// 给定的值附加引号
        /// </summary>
        /// <param name="sb"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        void AppendQuote(StringBuilder sb, string value);

        /// <summary>
        /// 附加参数
        /// </summary>
        /// <param name="parameterName">参数名</param>
        /// <returns></returns>
        string AppendParameter(string parameterName);

        /// <summary>
        /// 附加参数
        /// </summary>
        /// <param name="sb"></param>
        /// <param name="parameterName">参数名</param>
        /// <returns></returns>
        void AppendParameter(StringBuilder sb, string parameterName);

        /// <summary>
        /// 附加含有值的参数
        /// </summary>
        /// <param name="parameterName">参数名</param>
        /// <returns></returns>
        string AppendParameterWithValue(string parameterName);

        /// <summary>
        /// 附加含有值的参数
        /// </summary>
        /// <param name="sb"></param>
        /// <param name="parameterName">参数名</param>
        /// <returns></returns>
        void AppendParameterWithValue(StringBuilder sb, string parameterName);

        /// <summary>
        /// 附加查询条件
        /// </summary>
        /// <param name="queryWhere"></param>
        /// <returns></returns>
        string AppendQueryWhere(string queryWhere);

        /// <summary>
        /// 附加查询条件
        /// </summary>
        /// <param name="sb"></param>
        /// <param name="queryWhere">查询条件</param>
        void AppendQueryWhere(StringBuilder sb, string queryWhere);

        /// <summary>
        /// 附加排序字段
        /// </summary>
        /// <param name="sb"></param>
        /// <param name="paging"></param>
        void AppendOrderBy(StringBuilder sb, Paging paging);

        /// <summary>
        /// 生成分页语句
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <param name="queryWhere">查询条件</param>
        /// <param name="paging">分页类</param>
        /// <returns></returns>
        string GeneratePagingSql(string tableName, string queryWhere, Paging paging);

        #endregion

    }
}
