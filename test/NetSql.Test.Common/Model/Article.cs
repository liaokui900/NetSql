using System;
using System.Collections.Generic;
using System.Text;
using NetSql.Entities;

namespace NetSql.Test.Common.Model
{
    public class Article : EntityBase
    {
        public string Title { get; set; }

        public string Summary { get; set; }

        public string Body { get; set; }

        public string Category { get; set; }

        public int ReadCount { get; set; }

        public bool IsDeleted { get; set; }
    }
}
