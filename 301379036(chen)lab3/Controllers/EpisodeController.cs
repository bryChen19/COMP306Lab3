using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using _301379036_chen_lab3.Data;
using _301379036_chen_lab3.Models;
using _301379036_chen_lab3.Services;

namespace _301379036_chen_lab3.Controllers
{
    public class EpisodeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly S3Service _s3Service;

        public EpisodeController(ApplicationDbContext context, S3Service s3service)
        {
            _context = context;
            _s3Service = s3service;
        }

        // GET: Episode
        public async Task<IActionResult> Index(string searchTopic, string searchHost)
        {
            var episodes = _context.Episodes.AsQueryable();

            //search by topic
            if (!string.IsNullOrEmpty(searchTopic))
            {
                episodes = episodes.Where(e => e.Topic.Contains(searchTopic));
            }
            //search by host
            if (!string.IsNullOrEmpty(searchHost))
            {
                episodes = episodes.Where(e => e.Host.Contains(searchHost));
            }
            //sort by release date in descending order
            episodes = episodes.OrderByDescending(e => e.ReleaseDate);
            return View(await episodes.ToListAsync());
        }

        // GET: Episode/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var episodeModel = await _context.Episodes
                .FirstOrDefaultAsync(m => m.EpisodeId == id);
            if (episodeModel == null)
            {
                return NotFound();
            }

            return View(episodeModel);
        }

        // GET: Episode/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Episode/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(EpisodeModel episodeModel, IFormFile audioFile)
        {
            if (ModelState.IsValid)
            {
                episodeModel.PlayCount = 0;
                episodeModel.NumberOfViews = 0;
                if (audioFile != null && audioFile.Length > 0)
                {
                    // Upload the audio file to S3 and get the URL
                    var audioFileUrl = await _s3Service.UploadFileAsync(audioFile);
                    episodeModel.AudioFileURL = audioFileUrl;
                }
                _context.Add(episodeModel);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(episodeModel);
        }

        // GET: Episode/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var episodeModel = await _context.Episodes.FindAsync(id);
            if (episodeModel == null)
            {
                return NotFound();
            }
            return View(episodeModel);
        }

        // POST: Episode/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("EpisodeId,PodcastId,Title,ReleaseDate,Duration,PlayCount,AudioFileURL,NumberOfViews,Topic,Host")] EpisodeModel episodeModel)
        {
            if (id != episodeModel.EpisodeId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(episodeModel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EpisodeModelExists(episodeModel.EpisodeId))
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
            return View(episodeModel);
        }

        // GET: Episode/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var episodeModel = await _context.Episodes
                .FirstOrDefaultAsync(m => m.EpisodeId == id);
            if (episodeModel == null)
            {
                return NotFound();
            }

            return View(episodeModel);
        }

        // POST: Episode/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var episodeModel = await _context.Episodes.FindAsync(id);
            if (episodeModel != null)
            {
                _context.Episodes.Remove(episodeModel);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EpisodeModelExists(int id)
        {
            return _context.Episodes.Any(e => e.EpisodeId == id);
        }

        public async Task<IActionResult> ByPopular()
        {
            var episodes = await _context.Episodes.OrderByDescending(e => e.NumberOfViews).ToListAsync();

            return View("Index", episodes);
        }

        public async Task<IActionResult> Watch(int id)
        {
            var episode = await _context.Episodes.FindAsync(id);

            if (episode == null)
            {
                return NotFound();
            }

            episode.PlayCount++; //increment play count
            episode.NumberOfViews++; //increment number of views

            await _context.SaveChangesAsync();

            return View(episode);
        }

    }
}
