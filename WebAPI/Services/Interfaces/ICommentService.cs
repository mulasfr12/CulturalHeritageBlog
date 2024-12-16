using WebAPI.DTOs;

namespace WebAPI.Services.Interfaces
{
    public interface ICommentService
    {
        Task<IEnumerable<CommentDto>> GetCommentsByCulturalHeritageId(int heritageId);
        Task<int> AddComment(CommentDto commentDto);
        Task<bool> UpdateComment(int commentId, CommentDto commentDto);
        Task<bool> DeleteComment(int commentId);
        Task<CommentDto> GetCommentById(int commentId);
    }
}
