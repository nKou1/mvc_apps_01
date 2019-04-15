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
    public class TmDbComingSoonsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TmDbComingSoonsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: TmDbComingSoons
        [Authorize]
        public async Task<IActionResult> Index(string searchString,int? pageNumber)
        {
            ViewData["CurrentFilter"] = searchString;
            var tmDbComingSoons = from s in _context.TmDbComingSoon
                           select s;

            if (!String.IsNullOrEmpty(searchString))
            {
                tmDbComingSoons = _context.TmDbComingSoon.Where(s => s.Title.Contains(searchString));
            }
            tmDbComingSoons = tmDbComingSoons.OrderBy(x => x.ReleaseDate);
            int pageSize = 10;
            return View(await PaginatedList<TmDbComingSoon>.CreateAsync(tmDbComingSoons.AsNoTracking(), pageNumber ?? 1, pageSize));
        }

        // GET: TmDbComingSoons
        public async Task<IActionResult> IndexNonAuthorize(string searchString, int? pageNumber)
        {
            ViewData["CurrentFilter"] = searchString;
            var tmDbComingSoons = from s in _context.TmDbComingSoon
                                  select s;

            if (!String.IsNullOrEmpty(searchString))
            {
                tmDbComingSoons = _context.TmDbComingSoon.Where(s => s.Title.Contains(searchString));
            }
            tmDbComingSoons = tmDbComingSoons.OrderBy(x => x.ReleaseDate);
            int pageSize = 10;
            return View(await PaginatedList<TmDbComingSoon>.CreateAsync(tmDbComingSoons.AsNoTracking(), pageNumber ?? 1, pageSize));
        }

        // GET: TmDbComingSoons/Details/5
        [Authorize]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tmDbComingSoon = await _context.TmDbComingSoon
                .FirstOrDefaultAsync(m => m.ID == id);
            if (tmDbComingSoon == null)
            {
                return NotFound();
            }

            return View(tmDbComingSoon);
        }

        // GET: TmDbComingSoons/Details/5
        public async Task<IActionResult> DetailsNonAuthorize(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tmDbComingSoon = await _context.TmDbComingSoon
                .FirstOrDefaultAsync(m => m.ID == id);
            if (tmDbComingSoon == null)
            {
                return NotFound();
            }

            return View(tmDbComingSoon);
        }

        // GET: TmDbComingSoons/Create
        [Authorize]
        public IActionResult Create()
        {
            return View();
        }

        // POST: TmDbComingSoons/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,MovieId,Title,ReleaseDate,Popularity,BackdropPath,PosterPath,UpdateDate")] TmDbComingSoon tmDbComingSoon)
        {
            if (ModelState.IsValid)
            {
                _context.Add(tmDbComingSoon);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(tmDbComingSoon);
        }

        // GET: TmDbComingSoons/Edit/5
        [Authorize]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tmDbComingSoon = await _context.TmDbComingSoon.FindAsync(id);
            if (tmDbComingSoon == null)
            {
                return NotFound();
            }
            return View(tmDbComingSoon);
        }

        // POST: TmDbComingSoons/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,MovieId,Title,ReleaseDate,Popularity,BackdropPath,PosterPath,UpdateDate")] TmDbComingSoon tmDbComingSoon)
        {
            if (id != tmDbComingSoon.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    tmDbComingSoon.UpdateDate = DateTime.Now;
                    _context.Update(tmDbComingSoon);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TmDbComingSoonExists(tmDbComingSoon.ID))
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
            return View(tmDbComingSoon);
        }

        // GET: TmDbComingSoons/Delete/5
        [Authorize]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tmDbComingSoon = await _context.TmDbComingSoon
                .FirstOrDefaultAsync(m => m.ID == id);
            if (tmDbComingSoon == null)
            {
                return NotFound();
            }

            return View(tmDbComingSoon);
        }

        // POST: TmDbComingSoons/Delete/5
        [Authorize]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var tmDbComingSoon = await _context.TmDbComingSoon.FindAsync(id);
            _context.TmDbComingSoon.Remove(tmDbComingSoon);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TmDbComingSoonExists(int id)
        {
            return _context.TmDbComingSoon.Any(e => e.ID == id);
        }
    }
}
