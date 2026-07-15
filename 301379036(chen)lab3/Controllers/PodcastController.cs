using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using _301379036_chen_lab3.Data;
using _301379036_chen_lab3.Models;

namespace _301379036_chen_lab3.Controllers
{
    public class PodcastController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PodcastController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Podcast
        public async Task<IActionResult> Index()
        {
            return View(await _context.Podcasts.ToListAsync());
        }

        // GET: Podcast/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var podcastModel = await _context.Podcasts
                .FirstOrDefaultAsync(m => m.PodcastID == id);
            if (podcastModel == null)
            {
                return NotFound();
            }

            return View(podcastModel);
        }

        // GET: Podcast/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Podcast/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PodcastID,Title,Description,CreatorID")] PodcastModel podcastModel)
        {
            if (ModelState.IsValid)
            {
                podcastModel.CreatedDate = DateTime.Now;
                _context.Add(podcastModel);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(podcastModel);
        }

        // GET: Podcast/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var podcastModel = await _context.Podcasts.FindAsync(id);
            if (podcastModel == null)
            {
                return NotFound();
            }
            return View(podcastModel);
        }

        // POST: Podcast/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("PodcastID,Title,Description,CreatorID,CreatedDate")] PodcastModel podcastModel)
        {
            if (id != podcastModel.PodcastID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(podcastModel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PodcastModelExists(podcastModel.PodcastID))
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
            return View(podcastModel);
        }

        // GET: Podcast/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var podcastModel = await _context.Podcasts
                .FirstOrDefaultAsync(m => m.PodcastID == id);
            if (podcastModel == null)
            {
                return NotFound();
            }

            return View(podcastModel);
        }

        // POST: Podcast/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var podcastModel = await _context.Podcasts.FindAsync(id);
            if (podcastModel != null)
            {
                _context.Podcasts.Remove(podcastModel);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PodcastModelExists(int id)
        {
            return _context.Podcasts.Any(e => e.PodcastID == id);
        }
    }
}
