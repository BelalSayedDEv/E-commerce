using E_Commerce.DTos.CommentDTOs;
using E_Commerce.Model;
using E_Commerce.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace E_Commerce.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CommentController : ControllerBase
    {
        private readonly ICommentService commentService;

        public CommentController(ICommentService commentService)
        {
            this.commentService = commentService;
        }



        [AllowAnonymous]
        [HttpGet("{Id}")]
        public async Task<IActionResult> GetCommentByProductId(int Id)
        {
            var commentHistoryDto = await commentService.FindByProdcutId(Id);

            if (commentHistoryDto == null)
                return NotFound(ApiResponse<object>.Failure("Comment Not Found"));

            return Ok(ApiResponse<CommentHistoryDto>.Success(commentHistoryDto));

        }




        [HttpPost]
        public async Task<IActionResult> AddComment(AddCommentDto addCommentDto)
        {
            string UserId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

            string UserName = User.FindFirstValue(ClaimTypes.Name)!;
            var Comment = await commentService.Add(UserName, UserId, addCommentDto);

            if (Comment == null)
                return BadRequest(ApiResponse<object>.Failure("There Error Here"));

            return Ok(ApiResponse<ShowCommentDto>.Success(Comment));
        }

        [HttpPatch]
        public async Task<IActionResult> EditeComment(UpdateCommetDto updateCommetDto)
        {
            string UserId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;


            var Comment = await commentService.UpdateComment(UserId, updateCommetDto);

            if (Comment == null)
                return NotFound(ApiResponse<object>.Failure("Comment is Not Found "));

            return Ok(ApiResponse<ShowCommentDto>.Success(Comment));

        }

        [HttpDelete("{CommentId}")]

        public async Task<IActionResult> DeleteComment(int CommentId)
        {
            string UserId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

            string Role = User.FindFirstValue(ClaimTypes.Role)!;

            var result = await commentService.DeleteComment(Role, UserId, CommentId);

            if (result)
                return Ok(ApiResponse<object>.Success(null, "Comment Deleted"));

            return NotFound(ApiResponse<object>.Failure("Comment Not Found"));

        }

        [HttpGet("History")]
        public async Task<IActionResult> GetCommentHistoryByUserId()
        {
            string UserId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

            var History = await commentService.GetHistoryOfCommentByUserId(UserId);

            if (History == null)
                return NotFound(ApiResponse<object>.Failure("Comment Not Found"));

            return Ok(ApiResponse<CommentHistoryDto>.Success(History));
        }


    }
}
