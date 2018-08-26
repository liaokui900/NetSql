using System.Data;

namespace NetSql.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly IDbContext _context;
        private IDbTransaction _transaction;

        public UnitOfWork(IDbContext context)
        {
            _context = context;
        }

        public IDbTransaction BeginTransaction()
        {
            _transaction = _context.BeginTransaction();
            return _transaction;
        }

        public void Commit()
        {
            if (_transaction != null)
            {
                _transaction.Commit();
                _transaction.Connection.Close();
            }
        }

        public void Rollback()
        {
            if (_transaction != null)
            {
                _transaction.Rollback();
                _transaction.Connection.Close();
            }
        }
    }
}
