using Microsoft.AspNetCore.Http;
using Protech.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Protech.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TicketController : ControllerBase
    {
        private readonly ProtechContext _context;

        public TicketController(ProtechContext context)
        {
            _context = context;
        }
        [HttpGet]
        [Route("GetAll")]
        public IActionResult Get()
        {
            List<Ticket> ticketList = (from t in _context.Tickets
                                       select t).ToList();

            if (ticketList.Count == 0)
            {
                return NotFound();
            }
            return Ok(ticketList);
        }
        [HttpGet]
        [Route("Stats")]
        public IActionResult GetStats(int userId)
        {
            var user = (from u in _context.Users
                        where u.IdUser == userId
                        select u).FirstOrDefault();

            if (user == null)
            {
                return NotFound();
            }

            var categoria = (from uc in _context.UserCategories
                             where uc.IdUserCategory == user.IdUserCategory
                             select uc.IdUserCategory).FirstOrDefault();

            int total = 0;
            int resueltos = 0;
            int progreso = 0;
            int espera = 0;

            switch (categoria)
            {
                case 1:
                    total = _context.Tickets.Count();
                    resueltos = _context.Tickets.Count(t => t.State == "RESUELTO");
                    progreso = _context.Tickets.Count(t => t.State == "EN PROGRESO");
                    espera = _context.Tickets.Count(t => t.State == "EN ESPERA");
                    break;
                case 2:
                    total = _context.Tickets.Count(t => t.IdUser == userId);
                    resueltos = _context.Tickets.Count(t => t.IdUser == userId && t.State == "RESUELTO");
                    progreso = _context.Tickets.Count(t => t.IdUser == userId && t.State == "EN PROGRESO");
                    espera = _context.Tickets.Count(t => t.IdUser == userId && t.State == "EN ESPERA");
                    break;
                case 3:
                    total = _context.Tickets.Count(t => t.IdEmployee == userId);
                    resueltos = _context.Tickets.Count(t => t.IdEmployee == userId && t.State == "RESUELTO");
                    progreso = _context.Tickets.Count(t => t.IdEmployee == userId && t.State == "EN PROGRESO");
                    espera = _context.Tickets.Count(t => t.IdEmployee == userId && t.State == "EN ESPERA");
                    break;
                default:
                    return BadRequest();
            }
            var stats = new
            {
                Total = total,
                Resueltos = resueltos,
                Progreso = progreso,
                Espera = espera
            };

            return Ok(stats);
        }
        [HttpGet]
        [Route("UserTickets")]
        public IActionResult GetUserTickets(int userId)
        {
            List<Ticket> tickets = (from t in _context.Tickets
                                    where t.IdUser == userId
                                    select t).ToList();
            if (tickets.Count == 0)
            {
                return NotFound();
            }
            return Ok(tickets);
        }
        [HttpGet]
        [Route("Assigned")]
        public IActionResult GetAssignedTickets(int employeeId) 
        {
            List<Ticket> tickets = (from t in _context.Tickets
                                   where t.IdEmployee == employeeId
                                   select t).ToList();
            if (tickets.Count == 0)
            {
                return NotFound();
            }
            return Ok(tickets);
        }
        [HttpGet]
        [Route("Filter")]
        public IActionResult GetFilteredTickets(string? estado = null, string? empleado = null, DateTime? fechaInicio = null, DateTime? fechaFinal = null) {

            var consulta = from t in _context.Tickets
                           join u in _context.Users on t.IdEmployee equals u.IdUser
                           select new {
                               Ticket = t,
                               Empleado = u.Name
                           };
            
            if (estado != null)
            {
                consulta = from t in consulta
                           where t.Ticket.State == estado
                           select t;
            }
            if (empleado != null) 
            {
                consulta = from t in consulta
                           where t.Empleado.Contains(empleado)
                           select t;
            }
            if (fechaInicio.HasValue) 
            { 
                consulta = from t in consulta
                           where t.Ticket.CreationDate >= fechaInicio.Value
                           select t;
            }
            if (fechaFinal.HasValue)
            {
                consulta = from t in consulta
                           where t.Ticket.CreationDate <= fechaFinal.Value
                           select t;
            }
            var tickets = consulta.Select(t => t.Ticket).ToList();

            if (tickets.Count == 0) {
                return NotFound();
            }
            return Ok(tickets);
        }
        [HttpPost]
        [Route("Create")]
        public IActionResult CreateTicket(int userId, [FromBody] Ticket ticket) 
        {
            //falta ver lo de los archivos de respaldo
            try {
                var newTicket = new Ticket
                {
                    IdUser = userId,
                    Name = ticket.Name,
                    Description = ticket.Description,
                    Priority = ticket.Priority,
                    State = "EN ESPERA",
                    CreationDate = DateTime.Now
                };

                _context.Tickets.Add(newTicket);
                _context.SaveChanges();
                return Ok(newTicket);
            }
            catch (Exception ex) 
            {
                return BadRequest(ex.Message);
            }
        }


    }
}
