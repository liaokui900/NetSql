using System.Linq;
using System.Text;
using NetSql.Enums;
using NetSql.SqlAdapter;

namespace NetSql.Entities
{
    internal class EntitySqlStatement
    {
        private readonly ISqlAdapter _sqlAdapter;
        private readonly IEntityDescriptor _descriptor;

        public EntitySqlStatement(IEntityDescriptor descriptor, ISqlAdapter sqlAdapter)
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

            foreach (var col in _descriptor.Columns)
            {
                //排除自增主键
                if (col.IsPrimaryKey && (_descriptor.PrimaryKeyType == PrimaryKeyType.Int || _descriptor.PrimaryKeyType == PrimaryKeyType.Long))
                    continue;

                _sqlAdapter.AppendQuote(sb, col.Name);
                sb.Append(",");

                _sqlAdapter.AppendParameter(valuesSql, col.PropertyInfo.Name);
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
            if (_descriptor.PrimaryKeyType != PrimaryKeyType.NoPrimaryKey)
                DeleteSingle = $"DELETE FROM {_descriptor.TableName} WHERE {_sqlAdapter.AppendQuote(_descriptor.PrimaryKey.Name)}={_sqlAdapter.AppendParameter(_descriptor.PrimaryKey.PropertyInfo.Name)};";
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

            Update = sb.ToString();

            if (_descriptor.PrimaryKeyType != PrimaryKeyType.NoPrimaryKey)
            {
                var columns = _descriptor.Columns.Where(m => !m.IsPrimaryKey);

                foreach (var col in columns)
                {
                    sb.AppendFormat("{0}={1}", _sqlAdapter.AppendQuote(col.Name),
                        _sqlAdapter.AppendParameter(col.PropertyInfo.Name));
                    sb.Append(",");
                }

                sb.Remove(sb.Length - 1, 1);

                sb.AppendFormat(" WHERE {0}={1};", _sqlAdapter.AppendQuote(_descriptor.PrimaryKey.Name), _sqlAdapter.AppendParameter(_descriptor.PrimaryKey.PropertyInfo.Name));

                UpdateSingle = sb.ToString();
            }
        }

        /// <summary>
        /// 设置查询语句
        /// </summary>
        private void SetQuerySql()
        {
            var sb = new StringBuilder("SELECT ");
            for (var i = 0; i < _descriptor.Columns.Count; i++)
            {
                var col = _descriptor.Columns[i];
                sb.AppendFormat("{0} AS '{1}'", _sqlAdapter.AppendQuote(col.Name), col.PropertyInfo.Name);
                if (i != _descriptor.Columns.Count - 1)
                {
                    sb.Append(",");
                }
            }
            sb.Append(" FROM ");
            _sqlAdapter.AppendQuote(sb, _descriptor.TableName);

            Query = sb.ToString();

            if (_descriptor.PrimaryKeyType != PrimaryKeyType.NoPrimaryKey)
            {
                sb.AppendFormat(" WHERE {0}={1};", _sqlAdapter.AppendQuote(_descriptor.PrimaryKey.Name), _sqlAdapter.AppendParameter(_descriptor.PrimaryKey.PropertyInfo.Name));
                Get = sb.ToString();
            }
        }

        #endregion
    }
}
