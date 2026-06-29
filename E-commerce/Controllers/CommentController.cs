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
        [HttpGet("product-id/{Id}")]
        public async Task<IActionResult> GetCommentByProductId(int Id)
        {
            var commentHistoryDto = await commentService.FindByProductId(Id);

            return Ok(ApiResponse<CommentHistoryDto>.Success(commentHistoryDto));

        }

        [HttpPost]
        public async Task<IActionResult> AddComment(AddCommentDto addCommentDto)
        {
            string UserId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

            string UserName = User.FindFirstValue(ClaimTypes.Name)!;

            var Comment = await commentService.Add(UserName, UserId, addCommentDto);

            if (Comment == null)
                return BadRequest(ApiResponse<object>.Failure("Product is not exist"));

            return CreatedAtAction(nameof(GetCommentByProductId), new { Id = Comment.Id }, ApiResponse<ShowCommentDto>.Success(Comment));
        }


        [HttpPatch]
        public async Task<IActionResult> EditComment(UpdateCommetDto updateCommetDto)
        {
            string UserId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;


            var Comment = await commentService.UpdateComment(UserId, updateCommetDto);

            if (Comment == null)
                return NotFound(ApiResponse<object>.Failure("Comment is Not Found"));

            return Ok(ApiResponse<ShowCommentDto>.Success(Comment));

        }



        [HttpDelete("{Id}")]

        public async Task<IActionResult> DeleteComment(int Id)
        {
            string UserId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

            string Role = User.FindFirstValue(ClaimTypes.Role)!;

            var result = await commentService.DeleteComment(Role, UserId, Id);

            if (result)
                return NoContent();

            return NotFound(ApiResponse<object>.Failure("Comment Not Found"));

        }

        [HttpGet]
        public async Task<IActionResult> GetCommentHistoryByUserId()
        {
            string UserId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

            var History = await commentService.GetHistoryOfCommentByUserId(UserId);

            return Ok(ApiResponse<CommentHistoryDto>.Success(History));
        }


    }
}
