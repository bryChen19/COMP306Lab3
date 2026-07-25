using _301379036_chen_lab3.Models;
using _301379036_chen_lab3.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace _301379036_chen_lab3.Controllers
{
    [Authorize]
    public sealed class CommentsDemoController : Controller
    {
        private readonly ICommentService _commentService;
        private readonly ILogger<CommentsDemoController> _logger;

        public CommentsDemoController(
            ICommentService commentService,
            ILogger<CommentsDemoController> logger)
        {
            _commentService = commentService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Index(
            string episodeId = "1",
            CancellationToken cancellationToken = default)
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            IReadOnlyList<CommentsModel> comments =
                await _commentService
                    .GetCommentsByEpisodeAsync(
                        episodeId,
                        cancellationToken);

            ViewBag.EpisodeId = episodeId;
            ViewBag.CurrentUserId = currentUserId;

            return View(comments);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(
            string episodeId,
            string podcastID,
            string text,
            CancellationToken cancellationToken)
        {
            string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null)
                return Challenge();

            try
            {
                await _commentService.AddCommentAsync(
                    episodeId,
                    podcastID,
                    userId,
                    text,
                    cancellationToken);

                TempData["SuccessMessage"] = "Comment added.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }

            return RedirectToAction(
                "Details",
                "Episode",
                new { id = episodeId });
        }

        [HttpGet]
        public async Task<IActionResult> Edit(
            string episodeId,
            string commentId,
            CancellationToken cancellationToken)
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            CommentsModel? comment =
                await _commentService.GetCommentAsync(
                    episodeId,
                    commentId,
                    cancellationToken);

            if (comment == null)
            {
                return NotFound();
            }

            bool ownsComment =
                await _commentService.UserOwnsCommentAsync(
                    episodeId,
                    commentId,
                    currentUserId,
                    cancellationToken);

            if (!ownsComment)
            {
                TempData["ErrorMessage"] =
                    "The selected user does not own this comment.";

                return RedirectToAction(
                    "Details",
                    "Episode",
    new
    {
        id = episodeId
    });
            }

            bool editable =
                await _commentService.IsCommentEditableAsync(
                    episodeId,
                    commentId,
                    cancellationToken);

            if (!editable)
            {
                TempData["ErrorMessage"] =
                    "This comment is more than 24 hours old and cannot be edited.";

                return RedirectToAction(
    "Details",
    "Episode",
    new
    {
        id = episodeId
    });
            }

            ViewBag.CurrentUserId = currentUserId;

            return View(comment);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            string episodeId,
            string commentId,
            string text,
            CancellationToken cancellationToken)
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            CommentUpdateResult result =
                await _commentService.UpdateCommentAsync(
                    episodeId,
                    commentId,
                    currentUserId!,
                    text,
                    cancellationToken);

            if (result.IsSuccess)
            {
                TempData["SuccessMessage"] =
                    result.Message;
            }
            else
            {
                TempData["ErrorMessage"] =
                    result.Message;
            }

            return RedirectToAction(
                "Details",
                "Episode",
                new
                {
                    id = episodeId
                });
        }
    }
}
