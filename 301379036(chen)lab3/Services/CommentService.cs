using Microsoft.AspNetCore.Mvc;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using _301379036_chen_lab3.Models;

namespace _301379036_chen_lab3.Services
{
    public class CommentService : ICommentService
    {
        private readonly IDynamoDBContext _context;

        public CommentService(IDynamoDBContext context)
        {
            _context = context;
        }

        public async Task<CommentsModel> AddCommentAsync(
            string episodeId,
            string podcastId,
            string userId,
            string text,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                throw new ArgumentException("Comment text cannot be empty.", nameof(text));
            }
            var comment = new CommentsModel
            {
                CommentId = Guid.NewGuid().ToString(),
                EpisodeId = episodeId,
                PodcastId = podcastId,
                UserId = userId,
                Text = text,
                Timestamp = DateTime.UtcNow
            };
            await _context.SaveAsync(comment, cancellationToken);
            return comment;
        }

        public async Task<IReadOnlyList<CommentsModel>> GetCommentsByEpisodeAsync(
            string episodeId,
            CancellationToken cancellationToken = default)
        {
            var comments = await _context.QueryAsync<CommentsModel>(episodeId).GetRemainingAsync(cancellationToken);
            return comments;
        }

        public async Task<CommentsModel?> GetCommentAsync(
            string episodeId,
            string commentId,
            CancellationToken cancellationToken = default)
        {
            var comment = await _context.LoadAsync<CommentsModel>(episodeId, commentId, cancellationToken);
            return comment;
        }

        public async Task<bool> UserOwnsCommentAsync(
            string episodeId,
            string commentId,
            string currentUserId,
            CancellationToken cancellationToken = default)
        {
            var comment = await GetCommentAsync(episodeId, commentId, cancellationToken);
            return comment != null && comment.UserId == currentUserId;
        }

        public async Task<bool> IsCommentEditableAsync(
            string episodeId,
            string commentId,
            CancellationToken cancellationToken = default)
        {
            var comment = await GetCommentAsync(episodeId, commentId, cancellationToken);
            if (comment == null)
            {
                return false;
            }
            // Check if the comment is older than 24 hours
            return (DateTime.UtcNow - comment.Timestamp).TotalHours <= 24;
        }

        public async Task<CommentUpdateResult> UpdateCommentAsync(
            string episodeId,
            string commentId,
            string currentUserId,
            string newText,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(newText))
            {
                return new CommentUpdateResult
                {
                    Status = CommentUpdateStatus.InvalidText,
                    Message = "Comment text cannot be empty."
                };
            }
            var comment = await GetCommentAsync(episodeId, commentId, cancellationToken);
            if (comment == null)
            {
                return new CommentUpdateResult
                {
                    Status = CommentUpdateStatus.NotFound,
                    Message = "Comment not found."
                };
            }

            if (comment.UserId != currentUserId)
            {
                return new CommentUpdateResult
                {
                    Status = CommentUpdateStatus.NotOwner,
                    Message = "User does not own the comment."
                };
            }

            if (!await IsCommentEditableAsync(episodeId, commentId, cancellationToken))
            {
                return new CommentUpdateResult
                {
                    Status = CommentUpdateStatus.EditWindowExpired,
                    Message = "Comment is no longer editable."
                };
            }

            comment.Text = newText;

            await _context.SaveAsync(comment, cancellationToken);
            return new CommentUpdateResult
            {
                Status = CommentUpdateStatus.Success,
                Comment = comment,
                Message = "Comment updated successfully."
            };
        }
    }
}
