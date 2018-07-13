using System;
using System.Collections.Generic;
using System.Text;
using NetSql.Entities;

namespace NetSql.Test.Common.Model
{
    public class Article : EntityBase
    {
        public string Title { get; set; }

        public bool IsDeleted { get; set; }
    }
}
