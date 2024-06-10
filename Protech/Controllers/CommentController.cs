using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Protech.Models;
using Protech.Services;

namespace Protech.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommentController : ControllerBase
    {
        private readonly ProtechContext _context;
        private IConfiguration _configuration;

        public CommentController(ProtechContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }
        [HttpPost]
        [Route("AddComment")]
        public IActionResult AddComment(int ticketId, [FromBody] TicketComment comment) {
            try {
                var ticket = (from t in _context.Tickets
                              where t.IdTicket == ticketId
                              select t).FirstOrDefault();
                if (ticket == null)
                {
                    return NotFound("Ticket not found");
                }

                var user = (from u in _context.Users
                            where u.IdUser == ticket.IdUser
                            select u).FirstOrDefault();
                if (user == null)
                {
                    return NotFound("User not found");
                }

                var newComment = new TicketComment
                {
                    IdTicket = ticketId,
                    IdUser = comment.IdUser,
                    Comment = comment.Comment,
                    Date = DateTime.Now,
                };

                _context.TicketComments.Add(newComment);
                _context.SaveChanges();

                correo enviarCorreo = new correo(_configuration);

                enviarCorreo.NewTicketComment(user.Email, user.Name, ticket.IdTicket, DateTime.Now, newComment.Comment, ticket.Name);


                return Ok(newComment);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

    }
}
