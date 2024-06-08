using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Protech.Models;

namespace Protech.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly ProtechContext _context;

        public UserController(ProtechContext context)
        {
            _context = context;
        }
        [HttpGet]
        [Route("GetAll")]
        public IActionResult GetAll() {

            List<User> userList = (from t in _context.Users
                                   select t).ToList();

            if (userList.Count == 0)
            {
                return NotFound("Users not found");
            }
            return Ok(userList);
        }

        [HttpPost]
        [Route("Create")]
        public IActionResult crearUsuario([FromBody] User user) {
            try {
                _context.Users.Add(user);
                _context.SaveChanges();
                return Ok(user);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpGet]
        [Route("UserInfo")]
        public IActionResult GetUser(int id) {
            User? user = (from u in _context.Users
                          where u.IdUser == id
                          select u).FirstOrDefault();
            if (user == null)
            {
                return NotFound("User not found");
            }
            return Ok(user);
        }
        [HttpPut]
        [Route("ToClient")]
        public IActionResult ToClient(int userId) {
            try{
                var user = (from u in _context.Users
                            where u.IdUser == userId
                            select u).FirstOrDefault();
                if (user == null)
                {
                    return NotFound("User not found");
                }
                user.IdUserCategory = 2;
                _context.SaveChanges();
                return Ok(user);
            }
            catch (Exception ex) 
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPut]
        [Route("ToSupport")]
        public IActionResult ToSupport(int userId)
        {
            try
            {
                var user = (from u in _context.Users
                            where u.IdUser == userId
                            select u).FirstOrDefault();
                if (user == null)
                {
                    return NotFound("User not found");
                }
                user.IdUserCategory = 3;
                _context.SaveChanges();
                return Ok(user);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
