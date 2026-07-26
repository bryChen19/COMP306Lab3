using _301379036_chen_lab3.Data;
using _301379036_chen_lab3.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace _301379036_chen_lab3.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ApplicationDbContext _context;

        public AdminController(UserManager<IdentityUser> userManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Episodes()
        {
            var episodes = await _context.Episodes
                .Include(e => e.Podcast)
                .Where(e => !e.IsApproved)
                .ToListAsync();

            return View(episodes);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ApproveEpisode(int id)
        {
            var episode = await _context.Episodes
                .FirstOrDefaultAsync(e => e.EpisodeId == id);

            if (episode == null)
                return NotFound();

            episode.IsApproved = true;

            await _context.SaveChangesAsync();

            return RedirectToAction("Index", "Episode");
        }

        [HttpGet]
        public async Task<IActionResult> Users()
        {
            var users = new List<UserRoleViewModel>();

            var allUsers = _userManager.Users.ToList();

            foreach (var user in allUsers)
            {
                var roles = await _userManager.GetRolesAsync(user);
                users.Add(new UserRoleViewModel
                {
                    Id = user.Id,
                    Email = user.Email,
                    Username = user.UserName,
                    Role = roles.FirstOrDefault() ?? "No role assigned"
                });
            }

            return View(users);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user != null)
            {
                var result = await _userManager.DeleteAsync(user);
                if (result.Succeeded)
                {
                    return RedirectToAction("Users");
                }
                else
                {
                    // Handle errors here
                    ModelState.AddModelError("", "Error deleting user.");
                }
            }
            else
            {
                ModelState.AddModelError("", "User not found.");
            }
            return RedirectToAction("Users");
        }

        [HttpGet]
        public async Task<IActionResult> EditUser(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            var model = new UserRoleViewModel
            {
                Id = user.Id,
                Email = user.Email,
                Username = user.UserName
            };

            var roles = await _userManager.GetRolesAsync(user);
            model.Role = roles.FirstOrDefault() ?? "";

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditUser(UserRoleViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.FindByIdAsync(model.Id);

            if (user == null)
            {
                return NotFound();
            }

            user.Email = model.Email;
            user.UserName = model.Username;

            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded)
            {
                var currentRoles = await _userManager.GetRolesAsync(user);
                await _userManager.RemoveFromRolesAsync(user, currentRoles);
                await _userManager.AddToRoleAsync(user, model.Role);

                return RedirectToAction(nameof(Users));
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }

            return View(model);
        }

    }
}
