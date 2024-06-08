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
        private readonly string _uploadFolderPath;

        public TicketController(ProtechContext context)
        {
            _context = context;
            _uploadFolderPath = Path.Combine(Directory.GetCurrentDirectory(), "uploads");

            if (!Directory.Exists(_uploadFolderPath))
            {
                Directory.CreateDirectory(_uploadFolderPath);
            }
        }
        [HttpGet]
        [Route("GetAll")]
        public IActionResult Get()
        {
            List<Ticket> ticketList = (from t in _context.Tickets
                                       select t).ToList();

            if (ticketList.Count == 0)
            {
                return NotFound("Tickets not found");
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
                return NotFound("User not found");
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
                return NotFound("User tickets not found");
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
                return NotFound("Assigned tickets not found");
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
        [Route("createTicket")]
        public async Task<IActionResult> CreateTicket([FromForm] TicketCreateModel model)
        {
            if (model == null)
            {
                return BadRequest();
            }

            var ticket = new Ticket
            {
                IdUser = model.IdUser,
                IdEmployee = model.IdEmployee,
                Name = model.Name,
                Description = model.Description,
                Priority = model.Priority,
                State = "EN ESPERA",
                CreationDate = DateTime.Now
            };

            _context.Tickets.Add(ticket);
            await _context.SaveChangesAsync();

            if (model.Files != null && model.Files.Count > 0)
            {
                foreach (var file in model.Files)
                {
                    var filePath = Path.Combine(_uploadFolderPath, file.FileName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }

                    var backupFile = new BackupFile
                    {
                        IdTicket = ticket.IdTicket,
                        Name = file.FileName
                    };

                    _context.BackupFiles.Add(backupFile);
                }

                await _context.SaveChangesAsync();
            }

            return Ok(new { message = "Ticket creado con éxito", ticketId = ticket.IdTicket });
        }
        [HttpPut]
        [Route("ChangeState")]
        public IActionResult ChangeTicketState(int ticketId, string state) 
        {

            try {
                var ticket = (from t in _context.Tickets
                              where t.IdTicket == ticketId
                              select t).FirstOrDefault();
                if (ticket == null)
                {
                    return NotFound("Ticket not found");
                }
                ticket.State = state;
                _context.SaveChanges();
                return Ok(ticket);
            }
            catch (Exception ex) {
                return BadRequest(ex.Message);
            }
        }
        public class TicketCreateModel
        {
            public int? IdUser { get; set; }
            public int? IdEmployee { get; set; }
            public string Name { get; set; }
            public string Description { get; set; }
            public string Priority { get; set; }
            public List<IFormFile> Files { get; set; }
        }

    }
}
