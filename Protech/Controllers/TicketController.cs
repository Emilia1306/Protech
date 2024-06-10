using Microsoft.AspNetCore.Http;
using Protech.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using System.Text.Json;
using Microsoft.Extensions.Options;
using Protech.Services;

namespace Protech.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TicketController : ControllerBase
    {
        private readonly ProtechContext _context;
        private readonly string _uploadFolderPath;
        private IConfiguration _configuration;

        public TicketController(ProtechContext context, IConfiguration configuration)
        {
            _context = context;
            _uploadFolderPath = Path.Combine(Directory.GetCurrentDirectory(), "uploads");
            _configuration = configuration;

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
            var options = new JsonSerializerOptions
            {
                ReferenceHandler = ReferenceHandler.Preserve // Agregar esta línea para preservar las referencias circulares
            };

            var tickets = _context.Tickets
                .Where(t => t.IdUser == userId)
                .Select(t => new Ticket
                {
                    IdTicket = t.IdTicket,
                    IdUser = t.IdUser,
                    IdEmployee = t.IdEmployee,
                    Name = t.Name,
                    Description = t.Description,
                    CreationDate = t.CreationDate,
                    Priority = t.Priority,
                    State = t.State,
                    IdEmployeeNavigation = _context.Users
                                            .Where(u => u.IdUser == t.IdEmployee)
                                            .Select(u => new User
                                            {
                                                Name = u.Name,
                                            })
                                            .FirstOrDefault(),
                    BackupFiles = _context.BackupFiles
                                    .Where(bc => bc.IdTicket == t.IdTicket)
                                    .Select(bc => new BackupFile
                                    {
                                        IdBackupFile = bc.IdBackupFile,
                                        Name = bc.Name
                                    })
                                    .ToList(),
                    IdUserNavigation = (from u in _context.Users
                                       where u.IdUser == t.IdUser
                                       select u).FirstOrDefault(),
                    TicketComments = _context.TicketComments
                                        .Where(tc => tc.IdTicket == t.IdTicket)
                                        .Select(tc => new TicketComment
                                        {
                                            IdTicket = tc.IdTicket,
                                            IdUser = t.IdUser,
                                            Comment = tc.Comment,
                                            Date = tc.Date,
                                            IdUserNavigation = _context.Users
                                                                .Where(u => u.IdUser == tc.IdUser)
                                                                .Select(u => new User
                                                                {
                                                                    Name = u.Name,
                                                                }).FirstOrDefault()
                                        })
                                        .ToList(),
                    TicketAdditionalTasks = _context.TicketAdditionalTasks
                                            .Where(tat => tat.IdTicket == t.IdTicket)
                                            .Select(tat => new TicketAdditionalTask
                                            {
                                                IdTicketAdditionalTask = tat.IdTicketAdditionalTask,
                                                Description = tat.Description,
                                                Finished = tat.Finished,
                                                IdEmployeeNavigation = _context.Users
                                                                        .Where(u => u.IdUser == tat.IdEmployee)
                                                                        .Select(u => new User
                                                                        {
                                                                            Name = u.Name,
                                                                            TicketAdditionalTasks = null,
                                                                            TicketComments = null,
                                                                            TicketIdEmployeeNavigations = null,
                                                                            TicketIdUserNavigations = null
                                                                        })
                                                                        .FirstOrDefault(),
                                            })
                                            .ToList()
                })
                .ToList();

            var json = JsonSerializer.Serialize(tickets, options); // Serializar los tickets a JSON con las opciones de serialización configuradas

            var contentResult = new ContentResult
            {
                Content = json,
                ContentType = "application/json",
                StatusCode = 200
            };

            return contentResult; // Devolver el resultado como un ContentResult
        }
        [HttpGet]
        [Route("AllTickets")]
        public IActionResult GetAllTickets()
        {
            var options = new JsonSerializerOptions
            {
                ReferenceHandler = ReferenceHandler.Preserve // Agregar esta línea para preservar las referencias circulares
            };

            var tickets = _context.Tickets
                .Select(t => new
                {
                    IdTicket = t.IdTicket,
                    IdUser = t.IdUser,
                    IdEmployee = t.IdEmployee,
                    Name = t.Name,
                    Description = t.Description,
                    CreationDate = t.CreationDate,
                    Priority = t.Priority,
                    State = t.State,
                    IdEmployeeNavigation = _context.Users
                                            .Where(u => u.IdUser == t.IdEmployee)
                                            .Select(u => new User
                                            {
                                                Name = u.Name,
                                            })
                                            .FirstOrDefault(),
                    BackupFiles = _context.BackupFiles
                                    .Where(bc => bc.IdTicket == t.IdTicket)
                                    .Select(bc => new BackupFile
                                    {
                                        IdBackupFile = bc.IdBackupFile,
                                        Name = bc.Name
                                    })
                                    .ToList(),
                    IdUserNavigation = (from u in _context.Users
                                        where u.IdUser == t.IdUser
                                        select u).FirstOrDefault(),
                    TicketComments = _context.TicketComments
                                        .Where(tc => tc.IdTicket == t.IdTicket)
                                        .Select(tc => new TicketComment
                                        {
                                            IdTicket = tc.IdTicket,
                                            IdUser = t.IdUser,
                                            Comment = tc.Comment,
                                            Date = tc.Date,
                                            IdUserNavigation = _context.Users
                                                                .Where(u => u.IdUser == tc.IdUser)
                                                                .Select(u => new User
                                                                {
                                                                    Name = u.Name,
                                                                }).FirstOrDefault()
                                        })
                                        .ToList(),
                TicketAdditionalTasks = _context.TicketAdditionalTasks
                                            .Where(tat => tat.IdTicket == t.IdTicket)
                                            .Select(tat => new TicketAdditionalTask
                                            {
                                                IdTicketAdditionalTask = tat.IdTicketAdditionalTask,
                                                Description = tat.Description,
                                                Finished = tat.Finished,
                                                IdEmployeeNavigation = _context.Users
                                                                        .Where(u => u.IdUser == tat.IdEmployee)
                                                                        .Select(u => new User
                                                                        {
                                                                            Name = u.Name,
                                                                            TicketAdditionalTasks = null,
                                                                            TicketComments = null,
                                                                            TicketIdEmployeeNavigations = null,
                                                                            TicketIdUserNavigations = null
                                                                        })
                                                                        .FirstOrDefault(),
                                            })
                                            .ToList()
                })
                .ToList();

            if (tickets.Count == 0)
            {
                return NotFound("Tickets not found");
            }

            var json = JsonSerializer.Serialize(tickets, options); // Serializar los tickets a JSON con las opciones de serialización configuradas

            var contentResult = new ContentResult
            {
                Content = json,
                ContentType = "application/json",
                StatusCode = 200
            };

            return contentResult; // Devolver el resultado como un ContentResult
        }
        [HttpGet]
        [Route("Assigned")]
        public IActionResult GetAssignedTickets(int employeeId) 
        {
            var options = new JsonSerializerOptions
            {
                ReferenceHandler = ReferenceHandler.Preserve // Agregar esta línea para preservar las referencias circulares
            };

            var tickets = _context.Tickets
                .Where(t => t.IdEmployee == employeeId)
                .Select(t => new Ticket
                {
                    IdTicket = t.IdTicket,
                    IdUser = t.IdUser,
                    IdEmployee = t.IdEmployee,
                    Name = t.Name,
                    Description = t.Description,
                    CreationDate = t.CreationDate,
                    Priority = t.Priority,
                    State = t.State,
                    IdEmployeeNavigation = _context.Users
                                            .Where(u => u.IdUser == t.IdEmployee)
                                            .Select(u => new User
                                            {
                                                Name = u.Name,
                                            })
                                            .FirstOrDefault(),
                    BackupFiles = _context.BackupFiles
                                    .Where(bc => bc.IdTicket == t.IdTicket)
                                    .Select(bc => new BackupFile
                                    {
                                        IdBackupFile = bc.IdBackupFile,
                                        Name = bc.Name
                                    })
                                    .ToList(),
                    IdUserNavigation = (from u in _context.Users
                                        where u.IdUser == t.IdUser
                                        select u).FirstOrDefault(),
                    TicketComments = _context.TicketComments
                                        .Where(tc => tc.IdTicket == t.IdTicket)
                                        .Select(tc => new TicketComment
                                        {
                                            IdTicket = tc.IdTicket,
                                            IdUser = t.IdUser,
                                            Comment = tc.Comment,
                                            Date = tc.Date,
                                            IdUserNavigation = _context.Users
                                                                .Where(u => u.IdUser == tc.IdUser)
                                                                .Select(u => new User
                                                                {
                                                                    Name = u.Name,
                                                                }).FirstOrDefault()
                                        })
                                        .ToList()
                })
                .ToList();

            var json = JsonSerializer.Serialize(tickets, options); // Serializar los tickets a JSON con las opciones de serialización configuradas

            var contentResult = new ContentResult
            {
                Content = json,
                ContentType = "application/json",
                StatusCode = 200
            };

            return contentResult;
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
        [HttpPut]
        [Route("Assign")]
        public async Task<IActionResult> AssignTicket(int ticketId, int employeeId) {
            var ticket = (from t in _context.Tickets
                          where t.IdTicket == ticketId
                          select t).FirstOrDefault();
            if (ticket == null)
            {
                return NotFound("Ticket not found");
            }
            // Obtener el usuario que creó el ticket
            var user = (from u in _context.Users
                        where u.IdUser == ticket.IdUser
                        select u).FirstOrDefault();
            if (user == null)
            {
                return NotFound("User not found");
            }

            // Verificar si el empleado existe
            var employee = await (from e in _context.Users
                                  where e.IdUser == employeeId
                                  select e).FirstOrDefaultAsync();
            if (employee == null)
            {
                return NotFound("Employee not found.");
            }

            ticket.IdEmployee = employeeId;

            try
            {
                await _context.SaveChangesAsync();

                correo enviarCorreo = new correo(_configuration);

                enviarCorreo.CustomerTicketAssignment(user.Email, user.Name, ticket.IdTicket, DateTime.Now, ticket.Description, employee.Name, ticket.Name);

                enviarCorreo.EmployeeTicketAssignment(employee.Email, employee.Name, ticket.IdTicket, DateTime.Now, ticket.Description, ticket.Name);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            return Ok(ticket);

        }
        [HttpPost]
        [Route("createTicket")]
        public async Task<IActionResult> CreateTicket([FromForm] TicketCreateModel model)
        {
            if (model == null)
            {
                return BadRequest();
            }

            var user = await _context.Users.FindAsync(model.IdUser);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            var ticket = new Ticket
            {
                IdUser = model.IdUser,
                IdEmployee = null,
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

            correo enviarCorreo = new correo(_configuration);

            enviarCorreo.TicketCreationConfirmation(user.Email, user.Name, ticket.IdTicket, ticket.Name, DateTime.Now, ticket.Description);

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
                // Obtener la información del usuario
                var user = (from u in _context.Users
                            where u.IdUser == ticket.IdUser
                            select u).FirstOrDefault();
                if (user == null)
                {
                    return NotFound("User not found");
                }

                ticket.State = state;
                _context.SaveChanges();

                correo enviarCorreo = new correo(_configuration);

                enviarCorreo.UpdateTicketStatus(user.Email, user.Name, ticket.IdTicket, DateTime.Now, ticket.Name, ticket.State);

                return Ok(ticket);
            }
            catch (Exception ex) {
                return BadRequest(ex.Message);
            }
        }
        public class TicketCreateModel
        {
            public int? IdUser { get; set; }
            public string Name { get; set; }
            public string Description { get; set; }
            public string Priority { get; set; }
            public List<IFormFile> Files { get; set; }
        }

    }
}
