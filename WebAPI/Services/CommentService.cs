using Microsoft.EntityFrameworkCore;
using WebAPI.DTOs;
using WebAPI.Models;
using WebAPI.Services.Interfaces;

namespace WebAPI.Services
{
    public class CommentService:ICommentService
    {
        private readonly CulturalHeritageDbContext _dbContext;

        public CommentService(CulturalHeritageDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<CommentDto>> GetCommentsByCulturalHeritageId(int heritageId)
        {
            return await _dbContext.Comments
                .Where(c => c.HeritageId == heritageId)
                .OrderByDescending(c => c.CreatedAt)
                .Select(c => new CommentDto
                {
                    CommentID = c.CommentId,
                    Content = c.Content,
                    UserID = c.UserId,
                    HeritageID = c.HeritageId,
                    CreatedAt = c.CreatedAt
                })
                .ToListAsync();
        }

        public async Task<int> AddComment(CommentDto commentDto)
        {
            var comment = new Comment
            {
                Content = commentDto.Content,
                UserId = commentDto.UserID,
                HeritageId = commentDto.HeritageID,
                CreatedAt = DateTime.UtcNow
            };

            _dbContext.Comments.Add(comment);
            await _dbContext.SaveChangesAsync();

            return comment.CommentId;
        }

        public async Task<bool> UpdateComment(int commentId, CommentDto commentDto)
        {
            var comment = await _dbContext.Comments.FindAsync(commentId);

            if (comment == null) return false;

            comment.Content = commentDto.Content;
            await _dbContext.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteComment(int commentId)
        {
            var comment = await _dbContext.Comments.FindAsync(commentId);

            if (comment == null) return false;

            _dbContext.Comments.Remove(comment);
            await _dbContext.SaveChangesAsync();

            return true;
        }
    }
}

