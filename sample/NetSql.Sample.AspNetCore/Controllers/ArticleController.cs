using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NetSql.Test.Common.Model;
using NetSql.Test.Common.Repository;

namespace NetSql.Sample.AspNetCore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ArticleController : ControllerBase
    {
        private readonly ArticleRepository _repository;

        public ArticleController(ArticleRepository repository)
        {
            _repository = repository;
        }

        [HttpGet("{id:int:min(0)}")]
        public Task<Article> Get(int id)
        {
            return _repository.GetAsync(id);
        }

        [HttpPost("create")]
        public async Task<Article> Create([FromBody] Article article)
        {
            await _repository.AddAsync(article);

            return article;
        }
    }
}