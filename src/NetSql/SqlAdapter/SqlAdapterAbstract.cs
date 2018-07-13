using System;
using System.Collections.Generic;
using System.Text;
using NetSql.Pagination;

namespace NetSql.SqlAdapter
{
    public abstract class SqlAdapterAbstract : ISqlAdapter
    {
        public char LeftQuote { get; }
        public char RightQuote { get; }
        public char ParameterPrefix { get; }
        public string IdentitySql { get; }
        public string AppendQuote(string value)
        {
            throw new NotImplementedException();
        }

        public void AppendQuote(StringBuilder sb, string value)
        {
            throw new NotImplementedException();
        }

        public string AppendParameter(string parameterName)
        {
            throw new NotImplementedException();
        }

        public void AppendParameter(StringBuilder sb, string parameterName)
        {
            throw new NotImplementedException();
        }

        public string AppendParameterWithValue(string parameterName)
        {
            throw new NotImplementedException();
        }

        public void AppendParameterWithValue(StringBuilder sb, string parameterName)
        {
            throw new NotImplementedException();
        }

        public string GeneratePagingSql(string tableName, string queryWhere, Paging paging)
        {
            throw new NotImplementedException();
        }
    }
}
