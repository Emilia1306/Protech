using Microsoft.AspNetCore.Http;
using Protech.Models;
using Microsoft.AspNetCore.Mvc;

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
        [Route("Total")]
        public IActionResult GetTotalTickets() {
            int totalTickets = _context.Tickets.Count();

            if (totalTickets == 0)
            {
                return Ok(totalTickets);
            }
            return Ok(totalTickets);
        }
        [HttpGet]
        [Route("Resuelto")]
        public IActionResult GetResolvedTickets()
        {
            int ticketsResueltos = _context.Tickets.Count(t => t.State == "RESUELTO");

            if (ticketsResueltos == 0)
            {
                return Ok(ticketsResueltos);
            }

            return Ok(ticketsResueltos);
        }
        [HttpGet]
        [Route("Progreso")]
        public IActionResult GetProgressTickets()
        {
            int ticketsProgreso = _context.Tickets.Count(t => t.State == "EN PROGRESO");

            if (ticketsProgreso == 0)
            {
                return Ok(ticketsProgreso);
            }

            return Ok(ticketsProgreso);
        }
        [HttpGet]
        [Route("Espera")]
        public IActionResult GetPendingTickets()
        {
            int ticketsEspera = _context.Tickets.Count(t => t.State == "EN ESPERA");

            if (ticketsEspera == 0)
            {
                return Ok(ticketsEspera);
            }

            return Ok(ticketsEspera);
        }
        [HttpGet]
        [Route("Filtrar")]
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


    }
}
