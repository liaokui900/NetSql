using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using NetSql.Entities;
using NetSql.Enums;
using NetSql.Internal;
using NetSql.SqlAdapter;
using DbType = NetSql.Enums.DbType;

namespace NetSql
{
    internal class DbSet<TEntity> : IDbSet<TEntity> where TEntity : Entity, new()
    {
        #region ==属性==

        private readonly NetSqlDbContext _context;

        private readonly EntityDescriptor<TEntity> _descriptor;

        private readonly EntitySqlStatement<TEntity> _sqlStatement;

        private readonly ISqlAdapter _sqlAdapter;

        #endregion

        #region ==构造函数==

        public DbSet(ISqlAdapter sqlAdapter, NetSqlDbContext context)
        {
            _sqlAdapter = sqlAdapter;
            _context = context;
            _descriptor = new EntityDescriptor<TEntity>();
            _sqlStatement = new EntitySqlStatement<TEntity>(_descriptor, sqlAdapter);
        }

        #endregion

        #region ==操作方法==

        public async Task<bool> AddAsync(TEntity entity, IDbTransaction transaction = null)
        {
            Check.NotNull(entity, nameof(entity));

            var con = GetCon(transaction);
            if (_descriptor.PrimaryKeyType == PrimaryKeyType.Int)
            {
                var sql = _sqlStatement.Insert + _sqlAdapter.IdentitySql;
                var id = await con.ExecuteScalarAsync<int>(sql, entity, transaction);
                if (id > 0)
                {
                    _descriptor.PrimaryKey.SetValue(entity, id);
                    return true;
                }
            }
            else if (_descriptor.PrimaryKeyType == PrimaryKeyType.Long)
            {
                var sql = _sqlStatement.Insert + _sqlAdapter.IdentitySql;
                var id = await con.ExecuteScalarAsync<long>(sql, entity, transaction);
                if (id > 0)
                {
                    _descriptor.PrimaryKey.SetValue(entity, id);
                    return true;
                }
            }
            else
            {
                return await con.ExecuteAsync(_sqlStatement.Insert, entity, transaction) > 0;
            }

            return false;
        }

        public Task<bool> BatchAddtAsync(IList<TEntity> entityList, IDbTransaction transaction = null)
        {
            if (entityList == null || !entityList.Any())
                return Task.FromResult(false);

            var con = GetCon(transaction);
            var commit = false;//标注是否提交事务
            if (transaction == null)
            {
                transaction = con.BeginTransaction();
                commit = true;
            }

            try
            {
                if (DbType.SqlServer == _context.Options.DbType)
                {
                    SqlServerBatchInsert(entityList, transaction);
                }
                else if (DbType.MySql == _context.Options.DbType)
                {
                    MySqlBatchInsert(entityList, transaction);
                }
                else if (DbType.SQLite == _context.Options.DbType)
                {
                    SQLiteBatchInsert(entityList, transaction);
                }

                if (commit)
                    transaction.Commit();

                return Task.FromResult(true);
            }
            catch
            {
                if (commit)
                    transaction?.Rollback();

                throw;
            }
        }

        public async Task<bool> RemoveAsync(dynamic id, IDbTransaction transaction = null)
        {
            PrimaryKeyValidate(id);

            var dynParms = new DynamicParameters();
            dynParms.Add(_sqlAdapter.AppendParameter("Id"), id);

            return await GetCon(transaction).ExecuteAsync(_sqlStatement.DeleteSingle, dynParms, transaction) > 0;
        }

        public Task<bool> BatchRemoveAsync<T>(IList<T> idList, IDbTransaction transaction = null)
        {
            //没有主键的表无法批量删除
            if (_descriptor.PrimaryKeyType == PrimaryKeyType.NoPrimaryKey)
                throw new ArgumentException("没有主键的表无法删除单条记录", nameof(idList));

            if (idList == null || !idList.Any())
                return Task.FromResult(false);

            var queryWhere = $" Id IN ({string.Join(",", idList)})";

            return DeleteWhereAsync(queryWhere, null, transaction);
        }

        public async Task<bool> UpdateAsync(TEntity entity, IDbTransaction transaction = null)
        {
            Check.NotNull(entity, nameof(entity));

            if (_descriptor.PrimaryKeyType == PrimaryKeyType.NoPrimaryKey)
                throw new ArgumentException("没有主键的实体对象无法使用该方法", nameof(entity));

            return await GetCon(transaction).ExecuteAsync(_sqlStatement.UpdateSingle, entity, transaction) > 0;
        }

        public Task<bool> BatchUpdateAsync(IList<TEntity> entityList, IDbTransaction transaction = null)
        {
            Check.Collection(entityList, nameof(entityList));

            var con = GetCon(transaction);
            var commit = false;//标注是否提交事务
            if (transaction == null)
            {
                transaction = con.BeginTransaction();
                commit = true;
            }

            try
            {
                if (DbType.SqlServer == _context.Options.DbType)
                {
                    SqlServerBatchUpdate(entityList, transaction);
                }
                else if (DbType.MySql == _context.Options.DbType)
                {
                    MySqlBatchUpdate(entityList, transaction);
                }
                else if (DbType.SQLite == _context.Options.DbType)
                {
                    SQLiteBatchUpdate(entityList, transaction);
                }

                if (commit)
                    transaction.Commit();

                return Task.FromResult(true);
            }
            catch
            {
                if (commit)
                    transaction?.Rollback();

                throw;
            }
        }

        public Task<TEntity> GetAsync(dynamic id, IDbTransaction transaction = null)
        {
            PrimaryKeyValidate(id);

            var dynParms = new DynamicParameters();
            dynParms.Add(_sqlAdapter.AppendParameter("Id"), id);

            return GetCon(transaction).QuerySingleOrDefaultAsync<TEntity>(_sqlStatement.Get, dynParms, transaction);
        }

        #endregion

        #region ==私有方法==

        /// <summary>
        /// 获取数据库连接
        /// </summary>
        /// <param name="transaction"></param>
        /// <returns></returns>
        private IDbConnection GetCon(IDbTransaction transaction)
        {
            return transaction == null ? _context.OpenConnection() : transaction.Connection;
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

        /// <summary>
        /// 根据条件删除数据
        /// </summary>
        /// <param name="queryWhere"></param>
        /// <param name="param"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        private async Task<bool> DeleteWhereAsync(string queryWhere, object param = null, IDbTransaction transaction = null)
        {
            if (string.IsNullOrWhiteSpace(queryWhere))
                return false;

            var sql = _sqlStatement.Delete + _sqlAdapter.AppendQueryWhere(queryWhere);

            return await GetCon(transaction).ExecuteAsync(sql, param, transaction) > 0;
        }

        #region ==批量插入==

        /// <summary>
        /// SqlServer批量插入
        /// </summary>
        /// <param name="entityList"></param>
        /// <param name="transaction"></param>
        private void SqlServerBatchInsert(IList<TEntity> entityList, IDbTransaction transaction)
        {
            var tasks = new List<Task>();

            var sqlSb = new StringBuilder();
            for (var i = 0; i < entityList.Count; i++)
            {
                sqlSb.Append(GenerateInsertSql(entityList[i]));

                if (i == entityList.Count - 1 || sqlSb.Length > 1048576)//1MB
                {
                    tasks.Add(transaction.Connection.ExecuteAsync(sqlSb.ToString(), null, transaction));

                    sqlSb.Clear();
                }
            }

            Task.WaitAll(tasks.ToArray());
        }

        /// <summary>
        /// MySql批量插入
        /// </summary>
        /// <param name="entityList"></param>
        /// <param name="transaction"></param>
        private void MySqlBatchInsert(IList<TEntity> entityList, IDbTransaction transaction)
        {
            var tasks = new List<Task>();

            var sqlSb = new StringBuilder();
            for (var i = 0; i < entityList.Count; i++)
            {
                sqlSb.Append(GenerateInsertSql(entityList[i]));

                if (i == entityList.Count - 1 || sqlSb.Length > 524288)//512KB
                {
                    tasks.Add(transaction.Connection.ExecuteAsync(sqlSb.ToString(), null, transaction));

                    sqlSb.Clear();
                }
            }

            Task.WaitAll(tasks.ToArray());
        }

        /// <summary>
        /// SQLite批量插入
        /// </summary>
        /// <param name="entityList"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        private void SQLiteBatchInsert(IList<TEntity> entityList, IDbTransaction transaction)
        {
            var tasks = new Task[entityList.Count];
            for (var i = 0; i < entityList.Count; i++)
            {
                var entity = entityList[i];
                tasks[i] = transaction.Connection.ExecuteAsync(GenerateInsertSql(entity), null, transaction);
            }

            Task.WaitAll(tasks);
        }

        /// <summary>
        /// 根据实体对象生成插入语句
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        private string GenerateInsertSql(TEntity entity)
        {
            var sb = new StringBuilder(_sqlStatement.Insert);

            foreach (var p in _descriptor.Properties)
            {
                if (p.PropertyType == typeof(string) || p.PropertyType == typeof(DateTime))
                    sb.Replace(_sqlAdapter.AppendParameter(p.Name), $"'{p.GetValue(entity)}'");
                else if (p.PropertyType == typeof(Enum))
                    sb.Replace(_sqlAdapter.AppendParameter(p.Name), ((int)p.GetValue(entity)).ToString());
                else if (p.PropertyType == typeof(bool))
                    sb.Replace(_sqlAdapter.AppendParameter(p.Name), ((bool)p.GetValue(entity)) ? "1" : "0");
                else
                    sb.Replace(_sqlAdapter.AppendParameter(p.Name), p.GetValue(entity).ToString());
            }

            return sb.ToString();
        }

        #endregion

        #region ==批量更新==

        /// <summary>
        /// SqlServer批量插入
        /// </summary>
        /// <param name="entityList"></param>
        /// <param name="transaction"></param>
        private void SqlServerBatchUpdate(IList<TEntity> entityList, IDbTransaction transaction)
        {
            var tasks = new List<Task>();

            var sqlSb = new StringBuilder();
            for (var i = 0; i < entityList.Count; i++)
            {
                sqlSb.Append(GenerateUpdateSql(entityList[i]));

                if (i == entityList.Count - 1 || sqlSb.Length > 1048576)//1MB
                {
                    tasks.Add(transaction.Connection.ExecuteAsync(sqlSb.ToString(), null, transaction));

                    sqlSb.Clear();
                }
            }

            Task.WaitAll(tasks.ToArray());
        }

        /// <summary>
        /// MySql批量插入
        /// </summary>
        /// <param name="entityList"></param>
        /// <param name="transaction"></param>
        private void MySqlBatchUpdate(IList<TEntity> entityList, IDbTransaction transaction)
        {
            var tasks = new List<Task>();

            var sqlSb = new StringBuilder();
            for (var i = 0; i < entityList.Count; i++)
            {
                sqlSb.Append(GenerateUpdateSql(entityList[i]));

                if (i == entityList.Count - 1 || sqlSb.Length > 524288)//512KB
                {
                    tasks.Add(transaction.Connection.ExecuteAsync(sqlSb.ToString(), null, transaction));

                    sqlSb.Clear();
                }
            }

            Task.WaitAll(tasks.ToArray());
        }

        /// <summary>
        /// SQLite批量插入
        /// </summary>
        /// <param name="entityList"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        private void SQLiteBatchUpdate(IList<TEntity> entityList, IDbTransaction transaction)
        {
            var tasks = new Task[entityList.Count];
            for (var i = 0; i < entityList.Count; i++)
            {
                var entity = entityList[i];
                tasks[i] = transaction.Connection.ExecuteAsync(GenerateUpdateSql(entity), null, transaction);
            }

            Task.WaitAll(tasks);
        }

        /// <summary>
        /// 根据实体对象生成更新语句
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        private string GenerateUpdateSql(TEntity entity)
        {
            var sb = new StringBuilder(_sqlStatement.UpdateSingle);

            foreach (var p in _descriptor.Properties)
            {
                if (p.PropertyType == typeof(string) || p.PropertyType == typeof(DateTime))
                    sb.Replace(_sqlAdapter.AppendParameter(p.Name), $"'{p.GetValue(entity)}'");
                else if (p.PropertyType == typeof(Enum))
                    sb.Replace(_sqlAdapter.AppendParameter(p.Name), ((int)p.GetValue(entity)).ToString());
                else if (p.PropertyType == typeof(bool))
                    sb.Replace(_sqlAdapter.AppendParameter(p.Name), ((bool)p.GetValue(entity)) ? "1" : "0");
                else
                    sb.Replace(_sqlAdapter.AppendParameter(p.Name), p.GetValue(entity).ToString());
            }

            return sb.ToString();
        }

        #endregion

        #endregion
    }
}
