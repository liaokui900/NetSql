using System.Linq;
using System.Text;
using NetSql.Enums;
using NetSql.SqlAdapter;

namespace NetSql.Entities
{
    internal class EntitySqlStatement<TEntity> where TEntity : Entity, new()
    {
        private readonly ISqlAdapter _sqlAdapter;
        private readonly EntityDescriptor<TEntity> _descriptor;

        public EntitySqlStatement(EntityDescriptor<TEntity> descriptor, ISqlAdapter sqlAdapter)
        {
            _descriptor = descriptor;
            _sqlAdapter = sqlAdapter;

            SetInsertSql();
            SetDeleteSql();
            SetUpdateSql();
            SetQuerySql();
        }

        /// <summary>
        /// 插入语句
        /// </summary>
        public string Insert { get; private set; }

        /// <summary>
        /// 删除单条语句
        /// </summary>
        public string DeleteSingle { get; private set; }

        /// <summary>
        /// 删除语句
        /// </summary>
        public string Delete { get; private set; }

        /// <summary>
        /// 修改单条语句
        /// </summary>
        public string UpdateSingle { get; private set; }

        /// <summary>
        /// 修改语句
        /// </summary>
        public string Update { get; private set; }

        /// <summary>
        /// 查询单条语句
        /// </summary>
        public string Get { get; set; }

        /// <summary>
        /// 查询语句
        /// </summary>
        public string Query { get; set; }

        #region ==语句生成方法==

        /// <summary>
        /// 设置插入语句
        /// </summary>
        private void SetInsertSql()
        {
            var sb = new StringBuilder();
            sb.Append("INSERT INTO ");
            _sqlAdapter.AppendQuote(sb, _descriptor.TableName);
            sb.Append("(");

            var valuesSql = new StringBuilder(") VALUES(");

            foreach (var p in _descriptor.Properties)
            {
                //排除自增主键
                if ((_descriptor.PrimaryKeyType == PrimaryKeyType.Int ||
                     _descriptor.PrimaryKeyType == PrimaryKeyType.Long) && p == _descriptor.PrimaryKey)
                    continue;

                _sqlAdapter.AppendQuote(sb, p.Name);
                sb.Append(",");

                _sqlAdapter.AppendParameter(valuesSql, p.Name);
                valuesSql.Append(",");
            }

            //删除最后一个","
            sb.Remove(sb.Length - 1, 1);

            //删除最后一个","
            valuesSql.Remove(valuesSql.Length - 1, 1);
            valuesSql.Append(");");

            sb.Append(valuesSql);

            Insert = sb.ToString();
        }

        /// <summary>
        /// 设置删除语句
        /// </summary>
        private void SetDeleteSql()
        {
            Delete = $"DELETE FROM {_descriptor.TableName} ";
            DeleteSingle = $"DELETE FROM {_descriptor.TableName} WHERE {_sqlAdapter.AppendParameterWithValue("Id")};";
        }

        /// <summary>
        /// 设置更新语句
        /// </summary>
        private void SetUpdateSql()
        {
            var sb = new StringBuilder();
            sb.AppendFormat("UPDATE ");
            _sqlAdapter.AppendQuote(sb, _descriptor.TableName);
            sb.Append(" SET ");

            var properties = _descriptor.PrimaryKeyType == PrimaryKeyType.NoPrimaryKey
                ? _descriptor.Properties
                : _descriptor.Properties.Where(m => m != _descriptor.PrimaryKey);

            foreach (var p in properties)
            {
                _sqlAdapter.AppendParameterWithValue(sb, p.Name);
                sb.Append(",");
            }

            sb.Remove(sb.Length - 1, 1);

            Update = sb.ToString();

            if (_descriptor.PrimaryKeyType != PrimaryKeyType.NoPrimaryKey)
            {
                sb.Append(" WHERE ");

                _sqlAdapter.AppendParameterWithValue(sb, "Id");

                sb.Append(";");

                UpdateSingle = sb.ToString();
            }
        }

        /// <summary>
        /// 设置查询语句
        /// </summary>
        private void SetQuerySql()
        {
            var sb = new StringBuilder();
            sb.Append("SELECT * FROM ");
            _sqlAdapter.AppendQuote(sb, _descriptor.TableName);

            Query = sb.ToString();

            if (_descriptor.PrimaryKeyType != PrimaryKeyType.NoPrimaryKey)
            {
                sb.Append(" WHERE ");
                _sqlAdapter.AppendParameterWithValue(sb, "Id");
                Get = sb.ToString();
            }
        }

        #endregion
    }
}
