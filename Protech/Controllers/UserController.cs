using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.Data;
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
        [HttpPost]
        [Route("Login")]
        public IActionResult Login([FromBody] Login request)
        {
            try
            {
                var user = _context.Users.SingleOrDefault(u => u.Email == request.Email);
                if (user == null)
                {
                    return NotFound("User not found");
                }

                bool isPasswordValid = BCrypt.Net.BCrypt.Verify(request.Password, user.Password);
                if (!isPasswordValid)
                {
                    return NotFound("Invalid password");
                }
                var userInfo = new
                {
                    user.IdUser,
                    user.Name,
                    user.Email,
                    user.Cellphone,
                    user.ChangePassword,
                    user.CompanyName,
                    user.JobPosition,
                    user.IdUserCategory
                };

                return Ok(userInfo);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
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
        [HttpGet]
        [Route("GetEmployees")]
        public IActionResult GetEmployees()
        {

            List<User> userList = (from t in _context.Users
                                   where t.IdUserCategory == 3
                                   select t).ToList();

            if (userList.Count == 0)
            {
                return NotFound("Employees not found");
            }
            return Ok(userList);
        }

        [HttpPost]
        [Route("CreateClient")]
        public IActionResult createClient([FromBody] User user) {
            try {
                var hashedPassword = BCrypt.Net.BCrypt.HashPassword(user.Password);

                var client = new User
                {
                    IdUserCategory = 2,
                    Name = user.Name,
                    Email = user.Email,
                    Cellphone = user.Cellphone,
                    Password = hashedPassword,
                    ChangePassword = true,
                    CompanyName = user.CompanyName,
                    JobPosition = user.JobPosition
                };
                _context.Users.Add(client);
                _context.SaveChanges();
                return Ok(client);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("CreateSupport")]
        public IActionResult createSupport([FromBody] User user)
        {
            try
            {
                var hashedPassword = BCrypt.Net.BCrypt.HashPassword(user.Password);

                var client = new User
                {
                    IdUserCategory = 3,
                    Name = user.Name,
                    Email = user.Email,
                    Cellphone = user.Cellphone,
                    Password = hashedPassword,
                    ChangePassword = true,
                    CompanyName = user.CompanyName,
                    JobPosition = user.JobPosition
                };
                _context.Users.Add(client);
                _context.SaveChanges();
                return Ok(client);
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

        [HttpPut]
        [Route("ChangePassword")]
        public IActionResult changePassword(int userId, string password) {
            try
            {
                var user = (from u in _context.Users
                            where u.IdUser == userId
                            select u).FirstOrDefault();
                if (user == null)
                {
                    return NotFound("User not found");
                }
                var hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);
                user.Password = hashedPassword;
                _context.SaveChanges();
                return Ok(user);
            }
            catch(Exception ex) {
                return BadRequest(ex.Message);
            }
        }
    }
}
