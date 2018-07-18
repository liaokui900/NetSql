# NetSql
基于Dapper轻量级的ORM框架~
# 使用方法
## 1、定义实体
``` C#
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
```
## 2、定义DbContext
``` C#
using NetSql.Test.Common.Model;

namespace NetSql.MySql.Test
{
    public class BlogDbContext : NetSqlDbContext
    {
        public IDbSet<Article> Articles { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //optionsBuilder.UseSqlServer("");
            //optionsBuilder.UseMySql("");
            optionsBuilder.UseSQLite("Filename=./Database/Test.db");
        }
    }
}
```
### 3、创建DbContext实例
``` C#
private readonly BlogDbContext _dbContext = new BlogDbContext();
```
### 4、添加
``` C#
var article = new Article
{
    Title1 = "test",
    Category = Category.Blog,
    Summary = "这是一篇测试文章",
    Body = "这是一篇测试文章这是一篇测试文章这是一篇测试文章这是一篇测试文章这是一篇测试文章这是一篇测试文章这是一篇测试文章这是一篇测试文章",
    ReadCount = 10,
    IsDeleted = true,
    CreatedTime = DateTime.Now
};

var b = _dbContext.Articles.AddAsync(article).Result;
```
### 5、批量添加
``` C#
var list = new List<Article>();
for (var i = 0; i < 10000; i++)
{
    list.Add(new Article
    {
        Title1 = "test" + i,
        Category = i % 3 == 1 ? Category.Blog : Category.Movie,
        Summary = "这是一篇测试文章",
        Body = "这是一篇测试文章这是一篇测试文章这是一篇测试文章这是一篇测试文章这是一篇测试文章这是一篇测试文章这是一篇测试文章这是一篇测试文章",
        ReadCount = 10,
        IsDeleted = i % 2 == 0,
        CreatedTime = DateTime.Now
    });
}
var b = _dbContext.Articles.BatchAddtAsync(list).Result;
```
### 6、单条删除
``` C#
var b = _dbContext.Articles.RemoveAsync(2).Result;
```
### 7、批量删除
``` C#
var idList = new List<int>();
for (var i = 0; i < 10000; i++)
{
    idList.Add(i);
}
var b = _dbContext.Articles.BatchRemoveAsync(idList).Result;
```
### 8、使用Lambda表达式删除
``` C#
var b = _dbContext.Articles.RemoveAsync(m => m.Title1 == "hahaha").Result;
```
### 9、修改单个实体
``` C#
var entity = _dbContext.Articles.GetAsync(10).Result;
entity.Title1 = "更新测试";
entity.IsDeleted = true;

var b = _dbContext.Articles.UpdateAsync(entity).Result;
```
### 10、批量修改
``` C#
var list = new List<Article>();
for (var i = 1; i < 10000; i++)
{
    list.Add(new Article
    {
        Id = i,
        Title1 = "更新测试" + i,
        Summary = "更新测试这是一篇测试文章",
        Body = "更新测试这一篇测试文章这是一篇测试文章这是一篇测试文章这是一篇测试文章这是一篇测试文章这是一篇测试文章这是一篇测试文章这是一篇测试",
        ReadCount = 20,
        IsDeleted = i % 2 == 1
    });
}

var b = _dbContext.Articles.BatchUpdateAsync(list).Result;
```
### 11、根据Lambda表达式修改
``` C#
var b = _dbContext.Articles.UpdateAsync(m => m.Id > 10, n => new Article
{
    Title1 = "哈哈",
    ReadCount = 100
}).Result;
```
### 12、根据主键查询
``` C#
var entity = _dbContext.Articles.GetAsync(2).Result;
```
# 其他用法
## 1、指定表名称
使用``` TableAttribute ``` 特性可以为实体指定表名称
``` C#
[Table("Article")]
public class Article : EntityBase
{
}
```
## 2、忽略属性
使用``` IgnoreAttribute ``` 特性可以忽略实体的属性
``` C#
public class Article : EntityBase
{
     [Ignore]
     public string Title { get; set; }
}
```
## 2、列别名属性
使用``` ColumnAttribute ``` 特性可以为实体属性指定表中的列名
``` C#
public class Article : EntityBase
{
     [Column("Title")]
     public string Title { get; set; }
}
```
# 未完成
分页查询
