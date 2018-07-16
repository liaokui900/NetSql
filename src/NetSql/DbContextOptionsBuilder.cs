using NetSql.Internal;

namespace NetSql
{
    /// <summary>
    /// 数据库上下文配置项生成器
    /// </summary>
    public class DbContextOptionsBuilder
    {
        private DbContextOptionsAbstract _options;

        internal DbContextOptionsAbstract Builder()
        {
            Validate();

            return _options;
        }

        public void Init(DbContextOptionsAbstract options)
        {
            _options = options;
        }

        private void Validate()
        {
            Check.NotNull(_options, nameof(_options));
            Check.NotNull(_options.ConnectionString, nameof(_options.ConnectionString));
            Check.NotNull(_options.SqlAdapter, nameof(_options.SqlAdapter));
        }
    }
}
