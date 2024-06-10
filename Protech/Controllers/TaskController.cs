using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Protech.Models;
using Protech.Services;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace Protech.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TaskController : ControllerBase
    {
        private readonly ProtechContext _context;
        private IConfiguration _configuration;

        public TaskController(ProtechContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }
        [HttpGet]
        [Route("GetTasks")]
        public IActionResult GetTasks(int employeeId)
        {
            var options = new JsonSerializerOptions
            {
                ReferenceHandler = ReferenceHandler.Preserve
            };

            var tasks = _context.TicketAdditionalTasks
                        .Where(tat => tat.IdEmployee == employeeId)
                        .Select(tat => new TicketAdditionalTask
                        {
                            IdTicketAdditionalTask = tat.IdTicketAdditionalTask,
                            Description = tat.Description,
                            Finished = tat.Finished,
                            IdTicket = tat.IdTicket,
                            IdEmployeeNavigation = _context.Users
                                                    .Where(u => u.IdUser == tat.IdEmployee)
                                                    .Select(u => new User
                                                    {
                                                        IdUser = u.IdUser,
                                                        Name = u.Name,
                                                        Cellphone = u.Cellphone,
                                                        Email = u.Email,
                                                        CompanyName = u.CompanyName
                                                    })
                                                    .FirstOrDefault()
                        })
                        .ToList();

            var json = JsonSerializer.Serialize(tasks, options);

            var contentResult = new ContentResult
            {
                Content = json,
                ContentType = "application/json",
                StatusCode = 200
            };

            return contentResult;
        }
        [HttpPost]
        [Route("CreateTask")]
        public IActionResult CreateTask(int employeeId, [FromBody] TicketAdditionalTask task) 
        {
            try {
                var employee = (from e in _context.Users
                                where e.IdUser == employeeId
                                select e).FirstOrDefault();

                if (employee == null)
                {
                    return NotFound("Employee Not Found");
                }

                var newTask = new TicketAdditionalTask {
                    IdTicket = task.IdTicket,
                    IdEmployee = employeeId,
                    Description = task.Description,
                    Finished = false
                };
                _context.TicketAdditionalTasks.Add(newTask);
                _context.SaveChanges();

                correo enviarCorreo = new correo(_configuration);

                enviarCorreo.TaskAssignment(employee.Email, employee.Name, newTask.IdTicketAdditionalTask, newTask.IdTicket, DateTime.Now, newTask.Description);

                var options = new JsonSerializerOptions
                {
                    ReferenceHandler = ReferenceHandler.Preserve // Agregar esta línea para preservar las referencias circulares
                };

                var json = JsonSerializer.Serialize(newTask, options);

                var contentResult = new ContentResult
                {
                    Content = json,
                    ContentType = "application/json",
                    StatusCode = 200
                };

                return contentResult;
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPut]
        [Route("ChangeState")]
        public IActionResult ChangeTaskState(int taskId, bool state) 
        {
            try {
                var task = (from t in _context.TicketAdditionalTasks
                            where t.IdTicketAdditionalTask == taskId
                            select t).FirstOrDefault();

                if (task == null)
                {
                    return NotFound("Task not found");
                }

                var ticket = (from tk in _context.Tickets
                              where tk.IdTicket == task.IdTicket
                              select tk).FirstOrDefault();
                if (ticket == null)
                {
                    return NotFound("Ticket not found");
                }

                var user = (from u in _context.Users
                            where u.IdUser == ticket.IdEmployee
                            select u).FirstOrDefault();
                if (user == null)
                {
                    return NotFound("User not found");
                }

                task.Finished = state;
                _context.SaveChanges();
                correo enviarCorreo = new correo(_configuration);

                enviarCorreo.UpdateTaskStatus(user.Email, user.Name, ticket.IdTicket, DateTime.Now, taskId, task.Description);

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
