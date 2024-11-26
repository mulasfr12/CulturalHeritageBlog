using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebAPI.DTOs;
using WebAPI.Services.Interfaces;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CommentController : ControllerBase
    {
        private readonly ICommentService _commentService;
        private readonly ILogServices _logServices;

        public CommentController(ICommentService commentService, ILogServices logServices)
        {
            _commentService = commentService;
            _logServices = logServices;
        }

        // GET: api/comment/heritage/{heritageId}
        [HttpGet("heritage/{heritageId}")]
        [AllowAnonymous] // Allow public access to view comments
        public async Task<IActionResult> GetCommentsByCulturalHeritageId(int heritageId)
        {
            var comments = await _commentService.GetCommentsByCulturalHeritageId(heritageId);
            return Ok(comments);
        }

        // POST: api/comment
        [HttpPost]
        [Authorize] // Requires authentication
        public async Task<IActionResult> AddComment(CommentDto commentDto)
        {
            try
            {
                var userId = int.Parse(User.Claims.First(c => c.Type == "UserID").Value);
                commentDto.UserID = userId;

                var commentId = await _commentService.AddComment(commentDto);

                await _logServices.AddLog($"Comment with id={commentId} added by user id={userId}.", userId);
                return CreatedAtAction(nameof(GetCommentsByCulturalHeritageId), new { heritageId = commentDto.HeritageID }, commentDto);
            }
            catch (Exception ex)
            {
                var userId = User.Claims.FirstOrDefault(c => c.Type == "UserID")?.Value;
                await _logServices.AddLog($"Error adding comment: {ex.Message}", userId != null ? int.Parse(userId) : 0);
                return StatusCode(500, new { message = "Internal server error." });
            }
        }

        // PUT: api/comment/{commentId}
        [HttpPut("{commentId}")]
        [Authorize] // Requires authentication
        public async Task<IActionResult> UpdateComment(int commentId, CommentDto commentDto)
        {
            try
            {
                var userId = int.Parse(User.Claims.First(c => c.Type == "UserID").Value);

                // Optional: Add logic to check if user is the owner of the comment or Admin
                var result = await _commentService.UpdateComment(commentId, commentDto);

                if (!result)
                {
                    await _logServices.AddLog($"Comment with id={commentId} not found for update.", userId);
                    return NotFound(new { message = "Comment not found." });
                }

                await _logServices.AddLog($"Comment with id={commentId} updated by user id={userId}.", userId);
                return NoContent();
            }
            catch (Exception ex)
            {
                var userId = User.Claims.FirstOrDefault(c => c.Type == "UserID")?.Value;
                await _logServices.AddLog($"Error updating comment: {ex.Message}", userId != null ? int.Parse(userId) : 0);
                return StatusCode(500, new { message = "Internal server error." });
            }
        }

        // DELETE: api/comment/{commentId}
        [HttpDelete("{commentId}")]
        [Authorize] // Requires authentication
        public async Task<IActionResult> DeleteComment(int commentId)
        {
            try
            {
                var userId = int.Parse(User.Claims.First(c => c.Type == "UserID").Value);

                // Optional: Add logic to check if user is the owner of the comment or Admin
                var result = await _commentService.DeleteComment(commentId);

                if (!result)
                {
                    await _logServices.AddLog($"Comment with id={commentId} not found for deletion.", userId);
                    return NotFound(new { message = "Comment not found." });
                }

                await _logServices.AddLog($"Comment with id={commentId} deleted by user id={userId}.", userId);
                return NoContent();
            }
            catch (Exception ex)
            {
                var userId = User.Claims.FirstOrDefault(c => c.Type == "UserID")?.Value;
                await _logServices.AddLog($"Error deleting comment: {ex.Message}", userId != null ? int.Parse(userId) : 0);
                return StatusCode(500, new { message = "Internal server error." });
            }
        }
    }
}
