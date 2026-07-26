using _301379036_chen_lab3.Data;
using _301379036_chen_lab3.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace _301379036_chen_lab3.Controllers
{
    public class SubscriptionController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SubscriptionController(ApplicationDbContext context)
        {
            _context = context;
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Subscribe(int podcastId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null)
                return Unauthorized();


            var exists = await _context.Subscriptions
                .AnyAsync(s =>
                    s.UserId == userId &&
                    s.PodcastId == podcastId);


            if (!exists)
            {
                var subscription = new SubscriptionModel
                {
                    UserId = userId,
                    PodcastId = podcastId,
                    SubscribedDate = DateTime.Now
                };

                _context.Subscriptions.Add(subscription);
                await _context.SaveChangesAsync();
            }


            return RedirectToAction(
                "Details",
                "Podcast",
                new { id = podcastId });
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Unsubscribe(int podcastId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var subscription = await _context.Subscriptions
                .FirstOrDefaultAsync(s =>
                    s.UserId == userId &&
                    s.PodcastId == podcastId);

            if (subscription != null)
            {
                _context.Subscriptions.Remove(subscription);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(
                "Details",
                "Podcast",
                new { id = podcastId });
        }

        // GET: Subscription
        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var subscriptions = await _context.Subscriptions
                .Include(s => s.Podcast)
                .Where(s => s.UserId == userId)
                .ToListAsync();

            return View(subscriptions);
        }

        // GET: Subscription/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var podcast = await _context.Podcasts
        .FirstOrDefaultAsync(p => p.PodcastID == id);
            var subscriptionModel = await _context.Subscriptions
                .FirstOrDefaultAsync(m => m.SubscriptionId == id);
            if (subscriptionModel == null)
            {
                return NotFound();
            }
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            bool isSubscribed = false;
            if (userId != null)
            {
                isSubscribed = await _context.Subscriptions
                    .AnyAsync(s =>
                        s.UserId == userId &&
                        s.PodcastId == podcast.PodcastID);
            }


            var viewModel = new PodcastDetailsViewModel
            {
                Podcast = podcast,
                IsSubscribed = isSubscribed
            };

            return View(subscriptionModel);
        }

        // GET: Subscription/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Subscription/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("SubscriptionId,UserId,PodcastId,SubscribedDate")] SubscriptionModel subscriptionModel)
        {
            if (ModelState.IsValid)
            {
                _context.Add(subscriptionModel);

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(subscriptionModel);
        }

        // GET: Subscription/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var subscriptionModel = await _context.Subscriptions.FindAsync(id);
            if (subscriptionModel == null)
            {
                return NotFound();
            }
            return View(subscriptionModel);
        }

        // POST: Subscription/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("SubscriptionId,UserId,PodcastId,SubscribedDate")] SubscriptionModel subscriptionModel)
        {
            if (id != subscriptionModel.SubscriptionId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(subscriptionModel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SubscriptionModelExists(subscriptionModel.SubscriptionId))
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
            return View(subscriptionModel);
        }

        // GET: Subscription/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var subscriptionModel = await _context.Subscriptions
                .FirstOrDefaultAsync(m => m.SubscriptionId == id);
            if (subscriptionModel == null)
            {
                return NotFound();
            }

            return View(subscriptionModel);
        }

        // POST: Subscription/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var subscriptionModel = await _context.Subscriptions.FindAsync(id);
            if (subscriptionModel != null)
            {
                _context.Subscriptions.Remove(subscriptionModel);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SubscriptionModelExists(int id)
        {
            return _context.Subscriptions.Any(e => e.SubscriptionId == id);
        }
    }
}
