using System;
using System.Collections.Generic;
using System.Linq;
using Lista10.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Lista10.Models;
using Microsoft.AspNetCore.Http;

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

        public async Task<IActionResult> Index(int? Id)
        {
            List<Article> articles;
            if (Id == null || Id == -1)
            {
                articles = await _context.Article.ToListAsync();
            }
            else
            {
                articles = await _context.Article.Where(a => a.Category == Id).ToListAsync();
            }

            var categories = await _context.Category.ToListAsync();


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

        public async Task<IActionResult> AddToBasket(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var article = await _context.Article
                .FirstOrDefaultAsync(m => m.Id == id);

            AddArticleToBasket(article.Id);

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> AddInBasket(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var article = await _context.Article
                .FirstOrDefaultAsync(m => m.Id == id);

            AddArticleToBasket(article.Id);

            return RedirectToAction(nameof(Basket));
        }

        public async Task<IActionResult> RemoveFromBasket(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var article = await _context.Article
                .FirstOrDefaultAsync(m => m.Id == id);

            RemoveArticleFromBasket(article.Id);

            return RedirectToAction(nameof(Basket));
        }

        public async Task<IActionResult> RemoveEntirelyFromBasket(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var article = await _context.Article
                .FirstOrDefaultAsync(m => m.Id == id);

            Response.Cookies.Delete($"article{id}");

            return RedirectToAction(nameof(Basket));
        }

        public async Task<IActionResult> Basket()
        {
            List<(Article, int)> list = new List<(Article, int)>();
            double price = 0;

            foreach (var (articleId, value) in Request.Cookies.Where(c => c.Key.Contains("article")).ToList())
            {
                var id = int.Parse(articleId.Substring(7));
                var valueInt = int.Parse(value);

                var article = await _context.Article
                    .FirstOrDefaultAsync(m => m.Id == id);

                if (article == null)
                {
                    ViewBag.deletedMeassage = "Artykuł, który był w koszyku nie jest już dostęny!";
                    Response.Cookies.Delete(articleId);
                }
                else
                {
                    price += valueInt * article.Price;
                    list.Add((article, valueInt));
                }
            }

            ViewBag.articlesInBasket = list.OrderBy(t => t.Item1.Name);
            ViewBag.Categories = _context.Category;
            ViewBag.isEmpty = (list.Count == 0);
            ViewBag.Sum = price;

            return View();
        }

        public void AddArticleToBasket(int Id)
        {
            CookieOptions options = new CookieOptions();
            options.Expires = DateTimeOffset.Now.AddDays(7);
            if (Request.Cookies.ContainsKey($"article{Id}"))
            {
                string value;
                Request.Cookies.TryGetValue($"article{Id}", out value);
                Response.Cookies.Append($"article{Id}", (int.Parse(value) + 1).ToString(), options);
            }
            else
            {
                Response.Cookies.Append($"article{Id}", "1", options);
            }
        }

        public void RemoveArticleFromBasket(int Id)
        {
            if (Request.Cookies.ContainsKey($"article{Id}"))
            {
                CookieOptions options = new CookieOptions();
                options.Expires = DateTimeOffset.Now.AddDays(7);

                string value;
                Request.Cookies.TryGetValue($"article{Id}", out value);
                var res = int.Parse(value) - 1;

                if (res > 0)
                {
                    Response.Cookies.Append($"article{Id}", (int.Parse(value) - 1).ToString(), options);
                }
                else
                {
                    Response.Cookies.Delete($"article{Id}");
                }
            }
        }
    }
}
