using System;
using System.Linq.Expressions;
using NetSql.Entities;

namespace NetSql.Expressions
{
    internal interface IExpressionContext<TEntity> where TEntity : Entity, new()
    {
        string ToSql(Expression<Func<TEntity, bool>> exp);

        string ToSql(Expression<Func<TEntity, TEntity>> exp);
    }
}
