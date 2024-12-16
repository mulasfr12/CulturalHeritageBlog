using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebAPI.DTOs;
using WebAPI.Models;
using WebAPI.Services;
using WebAPI.Services.Interfaces;
using WebApp.ViewModels;

namespace WebApp.Controllers
{
    [Authorize] // Accessible to any authenticated user
    public class CommentController : Controller
    {
        private readonly ICulturalHeritageService _culturalHeritageService;
        private readonly ICommentService _commentService;
        private readonly IMapper _mapper;

        public CommentController(ICulturalHeritageService culturalHeritageService,
            ICommentService commentService, IMapper mapper)
        {
            _culturalHeritageService = culturalHeritageService;
            _commentService = commentService;
            _mapper = mapper;
        }

        

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddComment(int heritageId, string content)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                    {
                        Console.WriteLine($"ModelState Error: {error.ErrorMessage}");
                    }
                }

                // Validate input
                if (string.IsNullOrWhiteSpace(content))
                {
                    return BadRequest(new { message = "Comment content cannot be empty." });
                }

                var userId = int.Parse(User.Claims.First(c => c.Type == "UserID").Value);

                var commentDto = new CommentDto
                {
                    HeritageID = heritageId,
                    Content = content,
                    UserID = userId
                };

                await _commentService.AddComment(commentDto);

                return RedirectToAction("Details", "CulturalHeritage", new { id = heritageId });
            }
            catch (Exception ex)
            {
                // Log the error and handle it appropriately
                Console.WriteLine($"Error adding comment: {ex.Message}");
                return StatusCode(500, new { message = "Internal server error." });
            }
        }

    }
}
//[HttpPost]
//[Authorize] // Ensures any authenticated user can access this
//public async Task<IActionResult> AddComment(int heritageId, string content)
//{
//    try
//    {
//        var userId = int.Parse(User.Claims.First(c => c.Type == "UserID").Value);

//        var commentDto = new CommentDto
//        {
//            HeritageID = heritageId,
//            Content = content,
//            UserID = userId
//        };

//        await _commentService.AddComment(commentDto);
//        return RedirectToAction(nameof(Details), new { id = heritageId });
//    }
//    catch (Exception ex)
//    
//        // Log and handle exception
//        return StatusCode(500, new { message = "Internal server error." });
//    }
//}

//[HttpPost]
//[Authorize]
//public async Task<IActionResult> DeleteComment(int id, int heritageId)
//{
//    // Fetch the comment details from the WebAPI
//    var comment = await _commentService.GetCommentById(id);

//    // Check if the comment exists and belongs to the current user
//    if (comment != null && comment.UserID == int.Parse(User.Claims.First(c => c.Type == "UserID").Value))
//    {
//        // Delete the comment if the conditions are met
//        await _commentService.DeleteComment(id);
//    }

//    return RedirectToAction(nameof(Details), new { id = heritageId });
//}