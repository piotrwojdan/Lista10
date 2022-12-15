using System.Collections.Generic;
using System.Linq;
using Lista10.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Lista10.Models;

namespace Lista10.Controllers
{
    public class ShopController : Controller
    {
        private readonly Lista10Context _context;
        public ShopController(Lista10Context context)
        {
            _context = context;
        }

        // public async Task<IActionResult> Index(List<Article> articles, List<Category> categories)
        // {
        //     ViewBag.articles = TempData["articles"];
        //     ViewBag.categories = TempData["categories"];
        //
        //     return View();
        // }

        public async Task<IActionResult> Index(int Id)
        {
            List<Article> articles;
            if (Id == -1)
            {
                articles = await _context.Article.ToListAsync();
            }
            else
            {
                articles = await _context.Article.Where(a => a.Category == Id).ToListAsync();
            }

            var categories = await _context.Category.ToListAsync();

            // TempData["articles"] = articles;
            // TempData["categories"] = categories;

            ViewBag.articles = articles;
            ViewBag.categories = categories;

            return View();
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var article = await _context.Article
                .FirstOrDefaultAsync(m => m.Id == id);
            if (article == null)
            {
                return NotFound();
            }

            ViewBag.Categories = _context.Category;

            return View(article);
        }
    }
}
