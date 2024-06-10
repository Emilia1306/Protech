using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Protech.Models;

namespace Protech.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TaskController : ControllerBase
    {
        private readonly ProtechContext _context;

        public TaskController(ProtechContext context)
        {
            _context = context;
        }
        [HttpGet]
        [Route("GetTasks")]
        public IActionResult GetTasks(int employeeId)
        {
            List<TicketAdditionalTask> tasks = (from t in _context.TicketAdditionalTasks
                                where t.IdEmployee == employeeId
                                select t).ToList();
            if (tasks.Count == 0)
            {
                return NotFound();
            }
            return Ok(tasks);
        }
        [HttpPost]
        [Route("CreateTask")]
        public IActionResult CreateTask(int employeeId, [FromBody] TicketAdditionalTask task) 
        {
            try {
                var newTask = new TicketAdditionalTask {

                    IdTicket = task.IdTicket,
                    IdEmployee = employeeId,
                    Description = task.Description,
                    Finished = false
                };
                _context.TicketAdditionalTasks.Add(newTask);
                _context.SaveChanges();
                return Ok(newTask);
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

                task.Finished = state;
                _context.SaveChanges();
                return Ok(task);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
