using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Protech.Models;

namespace Protech.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommentController : ControllerBase
    {
        private readonly ProtechContext _context;

        public CommentController(ProtechContext context)
        {
            _context = context;
        }
        [HttpPost]
        [Route("AddComment")]
        public IActionResult AddComment(int ticketId, [FromBody] TicketComment comment) {
            try {
                var newComment = new TicketComment
                {
                    IdTicket = ticketId,
                    IdUser = comment.IdUser,
                    Comment = comment.Comment,
                    Date = DateTime.Now,
                };
                _context.TicketComments.Add(newComment);
                _context.SaveChanges();
                return Ok(newComment);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

    }
}
