using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using NetSql.Entities;
using NetSql.Enums;
using NetSql.Expressions;
using NetSql.Internal;
using NetSql.Pagination;
using NetSql.SqlAdapter;
using NetSql.SqlQueryable;

namespace NetSql.SqlQueryable
{
    internal class NetSqlQueryable<TEntity> : INetSqlQueryable<TEntity> where TEntity : Entity, new()
    {
        private readonly IDbTransaction _transaction;
        private Expression<Func<TEntity, bool>> _whereExpression;
        private Expression _selectExpression;
        private readonly List<Sort> _sort = new List<Sort>();
        private int _skip;
        private int _take;
        private readonly ParameterExpression _parameterExpression;
        private readonly IExpressionResolve _expressionResolve;
        private readonly IDbSet<TEntity> _dbSet;
        private readonly IEntityDescriptor _descriptor;
        private readonly EntitySqlStatement _sqlStatement;
        private readonly ISqlAdapter _sqlAdapter;

        public NetSqlQueryable(Expression<Func<TEntity, bool>> whereExpression, IDbSet<TEntity> dbSet, IEntityDescriptor descriptor, EntitySqlStatement sqlStatement, ISqlAdapter sqlAdapter, IDbTransaction transaction)
        {
            _whereExpression = whereExpression;
            _dbSet = dbSet;
            _descriptor = descriptor;
            _sqlStatement = sqlStatement;
            _sqlAdapter = sqlAdapter;
            _transaction = transaction;
            _expressionResolve = new ExpressionResolve(sqlAdapter, _descriptor);

            _parameterExpression = Expression.Parameter(typeof(TEntity), "m");
        }

        public INetSqlQueryable<TEntity> Where(Expression<Func<TEntity, bool>> expression)
        {
            if (expression == null)
                return this;

            if (_whereExpression == null)
                _whereExpression = expression;
            else
            {
                var exp = Expression.AndAlso(_whereExpression.Body, expression.Body);
                _whereExpression = Expression.Lambda<Func<TEntity, bool>>(exp, _parameterExpression);
            }

            return this;
        }

        public INetSqlQueryable<TEntity> WhereIf(bool isAdd, Expression<Func<TEntity, bool>> expression)
        {
            if (isAdd)
                Where(expression);

            return this;
        }

        public INetSqlQueryable<TEntity> OrderBy<TKey>(Expression<Func<TEntity, TKey>> expression, SortType sortType = SortType.Asc)
        {
            if (expression == null)
                return this;

            if (expression == null || !(expression.Body is MemberExpression memberExpression) || memberExpression.Expression.NodeType != ExpressionType.Parameter)
                throw new ArgumentException("排序列无效");

            OrderBy(new Sort(memberExpression.Member.Name, sortType));

            return this;
        }

        public INetSqlQueryable<TEntity> OrderBy(Sort sort)
        {
            if (sort != null)
            {
                var col = _descriptor.Columns.FirstOrDefault(m => m.Name.Equals(sort.OrderBy) || m.PropertyInfo.Name.Equals(sort.OrderBy));
                if (col == null)
                    throw new ArgumentException("排序列无效");

                var oldSort = _sort.FirstOrDefault(m => m.OrderBy.Equals(col.Name, StringComparison.OrdinalIgnoreCase));
                if (oldSort != null)
                {
                    _sort.Remove(oldSort);
                }
                _sort.Add(new Sort(col.Name, sort.Type));
            }

            return this;
        }

        public INetSqlQueryable<TEntity> Limit(int skip, int take)
        {
            _skip = skip;
            _take = take;
            return this;
        }

        public INetSqlQueryable<TEntity> Select<TResult>(Expression<Func<TEntity, TResult>> expression)
        {
            _selectExpression = expression;
            return this;
        }

        public Task<TResult> Max<TResult>(Expression<Func<TEntity, TResult>> expression)
        {
            Check.NotNull(expression, nameof(expression), "未指定求最大值的列");

            var memberExpression = expression.Body as MemberExpression;
            if (memberExpression == null)
                throw new ArgumentException("无法解析表达式", nameof(expression));

            var col = _descriptor.Columns.FirstOrDefault(c => c.PropertyInfo.Name.Equals(memberExpression.Member.Name));
            if (col == null)
                throw new ArgumentException("指定的求最大值的列不存在", nameof(TResult));

            var sql = $"SELECT MAX({_sqlAdapter.AppendQuote(col.Name)}) FROM {_sqlAdapter.AppendQuote(_descriptor.TableName)} {WhereSql};";
            return _dbSet.ExecuteScalarAsync<TResult>(sql, null, _transaction);
        }

        public Task<TResult> Min<TResult>(Expression<Func<TEntity, TResult>> expression)
        {
            Check.NotNull(expression, nameof(expression), "未指定求最小值的列");

            var memberExpression = expression.Body as MemberExpression;
            if (memberExpression == null)
                throw new ArgumentException("无法解析表达式", nameof(expression));

            var col = _descriptor.Columns.FirstOrDefault(c => c.PropertyInfo.Name.Equals(memberExpression.Member.Name));
            if (col == null)
                throw new ArgumentException("指定的求最小值的列不存在", nameof(TResult));

            var sql = $"SELECT MIN({_sqlAdapter.AppendQuote(col.Name)}) FROM {_sqlAdapter.AppendQuote(_descriptor.TableName)} {WhereSql};";
            return _dbSet.ExecuteScalarAsync<TResult>(sql, null, _transaction);
        }

        public Task<long> Count()
        {
            var sql = $"SELECT COUNT(*) FROM {_sqlAdapter.AppendQuote(_descriptor.TableName)} {WhereSql};";
            return _dbSet.ExecuteScalarAsync<long>(sql, _transaction);
        }

        public async Task<TEntity> FirstAsync()
        {
            _take = 1;
            _skip = 0;
            return (await ToListAsync()).FirstOrDefault();
        }

        public async Task<bool> DeleteAsync()
        {
            Check.NotNull(_whereExpression, nameof(_whereExpression), "删除条件不能为空");
            Check.NotNull(WhereSql, nameof(WhereSql), "删除条件不能为空");

            var sql = $"{_sqlStatement.Delete} {WhereSql};";

            return await _dbSet.ExecuteAsync(sql, null, _transaction) > 0;
        }

        public async Task<bool> Update(Expression<Func<TEntity, TEntity>> expression)
        {
            Check.NotNull(_whereExpression, nameof(_whereExpression), "未指定过滤条件");
            Check.NotNull(expression, nameof(expression));

            var updateSql = _expressionResolve.ToSql(expression);
            Check.NotNull(updateSql, nameof(updateSql), "生成更新sql异常");
            var whereSql = WhereSql;
            Check.NotNull(whereSql, nameof(whereSql), "生成过滤sql异常");

            var sql = $"{_sqlStatement.Update} {updateSql} {whereSql}";

            return await _dbSet.ExecuteAsync(sql, null, _transaction) > 0;
        }

        public async Task<List<TEntity>> ToListAsync()
        {
            var list = await _dbSet.QueryAsync<TEntity>(ToSql());
            return list.ToList();
        }

        public string ToSql()
        {
            var selectSql = _expressionResolve.ToSelectSql(_selectExpression);

            //取前几条表示分页查询
            return _take > 0 ? _sqlAdapter.GeneratePagingSql(_descriptor.TableName, WhereSql, _skip, _take, OrderBySql(), selectSql) : $"{_sqlStatement.Query} {WhereSql} {OrderBySql()}";
        }

        /// <summary>
        /// 解析排序语句
        /// </summary>
        /// <returns></returns>
        private string OrderBySql()
        {
            if (_sort.Any())
            {
                var sb = new StringBuilder(" ORDER BY ");
                foreach (var sort in _sort)
                {
                    sb.AppendFormat("{0} {1},", sort.OrderBy, sort.Type == SortType.Asc ? "ASC" : "DESC");
                }

                sb.Remove(sb.Length - 1, 1);

                return sb.ToString();
            }
            return string.Empty;
        }

        /// <summary>
        /// 解析条件语句
        /// </summary>
        private string WhereSql => _sqlAdapter.AppendQueryWhere(_expressionResolve.ToSql(_whereExpression));
    }
}
