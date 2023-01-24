using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Lista10.Data;
using Lista10.Models;

namespace Lista10.Controllers
{
    [Route("api/article")]
    [ApiController]
    public class ArticlesAPIController : ControllerBase
    {
        private readonly Lista10Context _context;
        private int which_n = 0;

        public ArticlesAPIController(Lista10Context context)
        {
            _context = context;
        }

        // GET: api/ArticlesAPI
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Article>>> GetArticle()
        {
            return await _context.Article.ToListAsync();
        }

        // GET: api/ArticlesAPI/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Article>> GetArticle(int id)
        {
            var article = await _context.Article.FindAsync(id);

            if (article == null)
            {
                return NotFound();
            }

            return article;
        }

        // PUT: api/ArticlesAPI/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutArticle(int id, [FromBody] Article article)
        {
            if (id != article.Id)
            {
                return BadRequest();
            }

            _context.Entry(article).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ArticleExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/ArticlesAPI
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Article>> PostArticle([FromBody] Article article)
        {
            article.Picture = "images\\no_photo.jpg";
            _context.Article.Add(article);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetArticle", new { id = article.Id }, article);
        }

        // DELETE: api/ArticlesAPI/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteArticle(int id)
        {
            var article = await _context.Article.FindAsync(id);
            if (article == null)
            {
                return NotFound();
            }

            _context.Article.Remove(article);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpGet("range/{first}/{last}")]
        public async Task<ActionResult<IEnumerable<Article>>> GetArticleRange(int first, int last)
        {
            return await _context.Article
                .OrderBy(a => a.Id)
                .Skip(first)
                .Take(last - first)
                .ToListAsync();
        }

        private bool ArticleExists(int id)
        {
            return _context.Article.Any(e => e.Id == id);
        }
    }
}
