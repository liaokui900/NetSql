using System;
using System.Collections.Generic;
using System.Data;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Dapper;
using NetSql.Entities;
using NetSql.Enums;
using NetSql.Expressions;
using NetSql.Internal;
using NetSql.SqlAdapter;
using NetSql.SqlQueryable;

namespace NetSql
{
    internal class DbSet<TEntity> : IDbSet<TEntity> where TEntity : Entity, new()
    {
        #region ==属性==

        private readonly DbContext _context;

        private readonly IEntityDescriptor _descriptor;

        private readonly EntitySqlStatement _sqlStatement;

        private readonly ISqlAdapter _sqlAdapter;

        #endregion

        #region ==构造函数==

        public DbSet(ISqlAdapter sqlAdapter, DbContext context)
        {
            _sqlAdapter = sqlAdapter;
            _context = context;
            _descriptor = EntityCollection.TryGet<TEntity>();
            _sqlStatement = new EntitySqlStatement(_descriptor, sqlAdapter);
        }

        #endregion

        public IDbTransaction BeginTransaction()
        {
            return _context.BeginTransaction();
        }

        public async Task<bool> InsertAsync(TEntity entity, IDbTransaction transaction = null)
        {
            Check.NotNull(entity, nameof(entity));

            if (_descriptor.PrimaryKeyType == PrimaryKeyType.Int)
            {
                var sql = _sqlStatement.Insert + _sqlAdapter.IdentitySql;
                var id = await ExecuteScalarAsync<int>(sql, entity, transaction);
                if (id > 0)
                {
                    _descriptor.PrimaryKey.SetValue(entity, id);
                    return true;
                }
            }
            else if (_descriptor.PrimaryKeyType == PrimaryKeyType.Long)
            {
                var sql = _sqlStatement.Insert + _sqlAdapter.IdentitySql;
                var id = await ExecuteScalarAsync<long>(sql, entity, transaction);
                if (id > 0)
                {
                    _descriptor.PrimaryKey.SetValue(entity, id);
                    return true;
                }
            }
            else
            {
                return await ExecuteAsync(_sqlStatement.Insert, entity, transaction) > 0;
            }

            return false;
        }

        public async Task<bool> DeleteAsync(dynamic id, IDbTransaction transaction = null)
        {
            PrimaryKeyValidate(id);

            var dynParms = new DynamicParameters();
            dynParms.Add(_sqlAdapter.AppendParameter("Id"), id);

            return await ExecuteAsync(_sqlStatement.DeleteSingle, dynParms, transaction) > 0;
        }

        public async Task<bool> UpdateAsync(TEntity entity, IDbTransaction transaction = null)
        {
            Check.NotNull(entity, nameof(entity));

            if (_descriptor.PrimaryKeyType == PrimaryKeyType.NoPrimaryKey)
                throw new ArgumentException("没有主键的实体对象无法使用该方法", nameof(entity));

            return await ExecuteAsync(_sqlStatement.UpdateSingle, entity, transaction) > 0;
        }

        public Task<TEntity> GetAsync(dynamic id, IDbTransaction transaction = null)
        {
            PrimaryKeyValidate(id);

            var dynParms = new DynamicParameters();
            dynParms.Add(_sqlAdapter.AppendParameter("Id"), id);

            return QuerySingleOrDefaultAsync<TEntity>(_sqlStatement.Get, dynParms, transaction);
        }

        public INetSqlQueryable<TEntity> Find(Expression<Func<TEntity, bool>> expression = null, IDbTransaction transaction = null)
        {
            return new NetSqlQueryable<TEntity>(expression, this, _descriptor, _sqlStatement, _sqlAdapter, transaction);
        }

        public Task<int> ExecuteAsync(string sql, object param = null, IDbTransaction transaction = null, CommandType? commandType = null)
        {
            return GetCon(transaction).ExecuteAsync(sql, param, transaction, commandType: commandType);
        }

        public Task<T> ExecuteScalarAsync<T>(string sql, object param = null, IDbTransaction transaction = null, CommandType? commandType = null)
        {
            return GetCon(transaction).ExecuteScalarAsync<T>(sql, param, transaction, commandType: commandType);
        }

        public Task<T> QueryFirstOrDefaultAsync<T>(string sql, object param = null, IDbTransaction transaction = null, CommandType? commandType = null)
        {
            return GetCon(transaction).QueryFirstOrDefaultAsync<T>(sql, param, transaction, commandType: commandType);
        }

        public Task<T> QuerySingleOrDefaultAsync<T>(string sql, object param = null, IDbTransaction transaction = null, CommandType? commandType = null)
        {
            return GetCon(transaction).QuerySingleOrDefaultAsync<T>(sql, param, transaction, commandType: commandType);
        }

        public Task<IEnumerable<T>> QueryAsync<T>(string sql, object param = null, IDbTransaction transaction = null, CommandType? commandType = null)
        {
            return GetCon(transaction).QueryAsync<T>(sql, param, transaction, commandType: commandType);
        }

        #region ==私有方法==

        /// <summary>
        /// 获取数据库连接
        /// </summary>
        /// <param name="transaction"></param>
        /// <returns></returns>
        private IDbConnection GetCon(IDbTransaction transaction)
        {
            return transaction == null ? _context.DbConnection : transaction.Connection;
        }

        /// <summary>
        /// 主键验证
        /// </summary>
        /// <param name="id"></param>
        private void PrimaryKeyValidate(dynamic id)
        {
            //没有主键的表无法删除单条记录
            if (_descriptor.PrimaryKeyType == PrimaryKeyType.NoPrimaryKey)
                throw new ArgumentException("该实体没有主键，无法使用该方法~");

            //验证id有效性
            if (_descriptor.PrimaryKeyType == PrimaryKeyType.Int || _descriptor.PrimaryKeyType == PrimaryKeyType.Long)
            {
                if (id < 1)
                    throw new ArgumentException("主键不能小于1~");
            }
            else
            {
                if (string.IsNullOrWhiteSpace(id))
                    throw new ArgumentException("主键不能为空~");
            }
        }
        #endregion
    }
}
