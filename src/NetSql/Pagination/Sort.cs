using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using NetSql.Entities;
using NetSql.Enums;

namespace NetSql.Pagination
{
    public class Sort<TEntity> : ISort where TEntity : Entity, new()
    {
        public SortType Type { get; set; }

        public IList<string> Columns { get; }

        private IEntityDescriptor _descriptor;

        public Sort(SortType sortType = SortType.Asc)
        {
            Type = sortType;
            Columns = new List<string>();
            _descriptor = EntityCollection.TryGet<TEntity>();
        }

        public Sort<TEntity> OrderBy<TKey>(Expression<Func<TEntity, TKey>> exp)
        {
            if (exp == null || !(exp.Body is MemberExpression memberExpression) || memberExpression.Expression.NodeType != ExpressionType.Parameter)
                throw new ArgumentException("排序列无效");

            var col = _descriptor.Columns.FirstOrDefault(m => m.PropertyInfo.Name.Equals(memberExpression.Member.Name));
            if (col == null)
                throw new ArgumentException("排序列无效");

            Columns.Add(col.Name);

            return this;
        }

        public string Builder()
        {
            if (Columns.Any())
                return $" ORDER BY {string.Join(",", Columns)} {(Type == SortType.Asc ? "ASC" : "DESC")}";
            return string.Empty;
        }
    }
}
