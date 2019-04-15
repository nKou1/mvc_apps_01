using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using mvc_apps_01.Data;
using mvc_apps_01.Models;

namespace mvc_apps_01.Controllers
{
    public class TmDbTrendingsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TmDbTrendingsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: TmDbTrendings
        [Authorize]
        public async Task<IActionResult> Index(string searchString, int? pageNumber)
        {
            ViewData["CurrentFilter"] = searchString;
            var tmDbTrendings = from s in _context.TmDbTrendings
                                  select s;

            if (!String.IsNullOrEmpty(searchString))
            {
                tmDbTrendings = _context.TmDbTrendings.Where(s => s.Title.Contains(searchString));
            }
            tmDbTrendings = tmDbTrendings.OrderByDescending(x => x.Popularity);

            int pageSize = 10;
            return View(await PaginatedList<TmDbTrending>.CreateAsync(tmDbTrendings.AsNoTracking(), pageNumber ?? 1, pageSize));
        }

        // GET: TmDbTrendings
        public async Task<IActionResult> IndexNonAuthorize(string searchString, int? pageNumber)
        {
            ViewData["CurrentFilter"] = searchString;
            var tmDbTrendings = from s in _context.TmDbTrendings
                                select s;

            if (!String.IsNullOrEmpty(searchString))
            {
                tmDbTrendings = _context.TmDbTrendings.Where(s => s.Title.Contains(searchString));
            }
            tmDbTrendings = tmDbTrendings.OrderByDescending(x => x.Popularity);

            int pageSize = 10;
            return View(await PaginatedList<TmDbTrending>.CreateAsync(tmDbTrendings.AsNoTracking(), pageNumber ?? 1, pageSize));
        }

        // GET: TmDbTrendings/Details/5
        [Authorize]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tmDbTrending = await _context.TmDbTrendings
                .FirstOrDefaultAsync(m => m.ID == id);
            if (tmDbTrending == null)
            {
                return NotFound();
            }

            return View(tmDbTrending);
        }

        // GET: TmDbTrendings/Details/5
        public async Task<IActionResult> DetailsNonAuthorize(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tmDbTrending = await _context.TmDbTrendings
                .FirstOrDefaultAsync(m => m.ID == id);
            if (tmDbTrending == null)
            {
                return NotFound();
            }

            return View(tmDbTrending);
        }

        // GET: TmDbTrendings/Create
        [Authorize]
        public IActionResult Create()
        {
            return View();
        }

        // POST: TmDbTrendings/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,MovieId,Title,ReleaseDate,Popularity,BackdropPath,PosterPath,UpdateDate")] TmDbTrending tmDbTrending)
        {
            if (ModelState.IsValid)
            {
                _context.Add(tmDbTrending);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(tmDbTrending);
        }

        // GET: TmDbTrendings/Edit/5
        [Authorize]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tmDbTrending = await _context.TmDbTrendings.FindAsync(id);
            if (tmDbTrending == null)
            {
                return NotFound();
            }
            return View(tmDbTrending);
        }

        // POST: TmDbTrendings/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,MovieId,Title,ReleaseDate,Popularity,BackdropPath,PosterPath,UpdateDate")] TmDbTrending tmDbTrending)
        {
            if (id != tmDbTrending.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    tmDbTrending.UpdateDate = DateTime.Now;
                    _context.Update(tmDbTrending);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TmDbTrendingExists(tmDbTrending.ID))
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
            return View(tmDbTrending);
        }

        // GET: TmDbTrendings/Delete/5
        [Authorize]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tmDbTrending = await _context.TmDbTrendings
                .FirstOrDefaultAsync(m => m.ID == id);
            if (tmDbTrending == null)
            {
                return NotFound();
            }

            return View(tmDbTrending);
        }

        // POST: TmDbTrendings/Delete/5
        [Authorize]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var tmDbTrending = await _context.TmDbTrendings.FindAsync(id);
            _context.TmDbTrendings.Remove(tmDbTrending);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TmDbTrendingExists(int id)
        {
            return _context.TmDbTrendings.Any(e => e.ID == id);
        }
    }
}
