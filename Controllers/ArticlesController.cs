using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Lista10.Data;
using Lista10.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Authorization;
using System.Data;

namespace Lista10.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ArticlesController : Controller
    {
        private readonly Lista10Context _context;
        private readonly IHostingEnvironment _hostingEnvironment;

        public ArticlesController(Lista10Context context, IHostingEnvironment hostingEnvironment)
        {
            _context = context;
            _hostingEnvironment = hostingEnvironment;
        }

        // GET: Articles
        public async Task<IActionResult> Index()
        {
            ViewBag.Categories = _context.Category;
            return View(await _context.Article.ToListAsync());
        }

        // GET: Articles/Details/5
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

        // GET: Articles/Create
        public IActionResult Create()
        {
            ViewBag.Category = new SelectList(_context.Category, "Id", "Name");
            return View();
        }

        // POST: Articles/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Price,PictureFile,Category")] Article article)
        {
            if (ModelState.IsValid)
            {
                var webRootPath = _hostingEnvironment.WebRootPath;
                var uploadFolder = "uploads";

                if (article.PictureFile != null)
                {
                    var fileName = Path.GetFileNameWithoutExtension(article.PictureFile.FileName);
                    fileName += DateTime.Now.ToString("yyyMMddhhmmss");
                    fileName += Path.GetExtension(article.PictureFile.FileName);

                    article.Picture = Path.Combine("", uploadFolder, fileName);

                    using (var stream = new FileStream(Path.Combine(webRootPath, uploadFolder, fileName), FileMode.Create))
                    {
                        article.PictureFile.CopyTo(stream);
                    }
                }
                else
                {
                    article.Picture = "images\\no_photo.jpg";
                }

                

                _context.Add(article);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(article);
        }

        // GET: Articles/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            ViewBag.Category = new SelectList(_context.Category, "Id", "Name");


            var article = await _context.Article.FindAsync(id);
            if (article == null)
            {
                return NotFound();
            }

            TempData["oldPicture"] = article.Picture;
            return View(article);
        }

        // POST: Articles/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,PictureFile,Price,Category")] Article article)
        {
            if (id != article.Id)
            {
                return NotFound();
            }

            string oldPicture = (string)TempData["oldPicture"];
            var webRootPath = _hostingEnvironment.WebRootPath;
            var uploadFolder = "uploads";

            if (ModelState.IsValid)
            {
                try
                {
                    if (article.PictureFile != null)
                    {
                        var fileName = Path.GetFileNameWithoutExtension(article.PictureFile.FileName);
                        fileName += DateTime.Now.ToString("yyyMMddhhmmss");
                        fileName += Path.GetExtension(article.PictureFile.FileName);

                        article.Picture = Path.Combine("", uploadFolder, fileName);

                        using (var stream = new FileStream(Path.Combine(webRootPath, uploadFolder, fileName), FileMode.Create))
                        {
                            article.PictureFile.CopyTo(stream);
                        }
                    }
                    else
                    {
                        article.Picture = "\\images\\no_photo.jpg";
                    }

                    if (oldPicture != null && oldPicture.Contains("uploads"))
                    {
                        if (article.Picture.Contains("no_photo")) article.Picture = oldPicture;
                        else
                        {
                            var path = Path.Combine(webRootPath, oldPicture);

                            if (System.IO.File.Exists(path))
                            {
                                System.IO.File.Delete(path);
                            }
                        }
                    }

                    _context.Update(article);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ArticleExists(article.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(article);
        }

        // GET: Articles/Delete/5
        public async Task<IActionResult> Delete(int? id)
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

        // POST: Articles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var article = await _context.Article.FindAsync(id);

            if (article.Picture != null && article.Picture.Contains("uploads"))
            {

                var webRootPath = _hostingEnvironment.WebRootPath;
                var path = Path.Combine(_hostingEnvironment.WebRootPath, article.Picture);

                if (System.IO.File.Exists(path))
                {
                    System.IO.File.Delete(path);
                }
            }

            _context.Article.Remove(article);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ArticleExists(int id)
        {
            return _context.Article.Any(e => e.Id == id);
        }
    }
}
