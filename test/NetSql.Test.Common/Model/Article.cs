using System;
using NetSql.Entities;
using NetSql.Mapper;

namespace NetSql.Test.Common.Model
{
    public class Article : EntityBase
    {
        [Column("Title")]
        public string Title1 { get; set; }

        public string Summary { get; set; }

        public string Body { get; set; }

        public Category Category { get; set; }

        public int ReadCount { get; set; }

        public bool IsDeleted { get; set; }

        public DateTime CreatedTime { get; set; }
    }

    public enum Category
    {
        Blog,
        Movie
    }
}
