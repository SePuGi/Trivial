using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using TrivialAPI;
using TrivialAPI.Model;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Security.Claims;
using System.Text.Json.Serialization;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;

namespace TrivialAPI.Controllers
{
    [Authorize]//NECESARIO PARA QUE FUNCIONE EL JWT
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;

        public UserController(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        private int GetLoggedUserId()
        {
            //User és  no és el model User de la base de dades, sinó una propietat de la classe 
            // base ControllerBase en ASP.NET Core que representa l'usuari autenticat que fa la petició.
            // aquí estem retornant el ID de l'usuari autenticat
            int idUser = 0;
            try
            {
                idUser = int.Parse(User.FindFirst("UserId").Value);
            }
            catch (Exception e)
            {
                return -1;
            }

            return idUser;
        }

        // GET: api/User
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUser()
        {
            var loggedUserId = GetLoggedUserId();

            if (loggedUserId == -1)
                return Unauthorized();

            var user = await _context.User.FindAsync(loggedUserId);

            return Ok(user);
        }

        //GET: /api/User
        [HttpGet("stats")]
        public async Task<ActionResult<IEnumerable<User>>> GetStats()
        {
            var loggedUserId = GetLoggedUserId();

            if (loggedUserId == -1)
                return Unauthorized();

            var users = await _context.User.ToListAsync();
            var userGames = await _context.CategoryGames.Where(cg => cg.UserId == loggedUserId).ToListAsync();

            var options = new JsonSerializerOptions
            {
                ReferenceHandler = ReferenceHandler.IgnoreCycles
            };

            var userStats = new
            {
                avgCorrectAnswers = userGames.Average(cg => cg.CorrectAnswers),
                games = userGames
            };

            return new JsonResult(userStats, options);
        }

        // PUT: api/User/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUser(User user)
        {
            var loggedUserId = GetLoggedUserId();

            if (loggedUserId == -1)
                return Unauthorized();

            if (UserExists(loggedUserId))
                return BadRequest("User not found");

            if (Utils.CheckPassword(user.Password))
                user.Password = Utils.EncryptPassword(user.Password);
            else
                return BadRequest("Password does not meet the requirements");

            _context.Entry(user).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(loggedUserId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: /api/User/register
        [AllowAnonymous]//NECESARIO PARA QUE FUNCIONE EL JWT
        [HttpPost("register")]
        public async Task<ActionResult<User>> RegisterUser(User user)
        {
            //comprobar que la contraseña cumpleix els requisits
            if(!Utils.CheckPassword(user.Password))
                return BadRequest("Password does not meet the requirements");
            
            user.Password = Utils.EncryptPassword(user.Password);
            
            _context.User.Add(user);
            await _context.SaveChangesAsync();

            return CreatedAtAction("RegisterUser", new { id = user.Id });
        }

        // POST: /api/User/login
        [AllowAnonymous]//NECESARIO PARA QUE FUNCIONE EL JWT
        [HttpPost("login")]
        public async Task<ActionResult<User>> LoginUser(UserLoginDTO user)
        {
            user.Password = Utils.EncryptPassword(user.Password);

            var userDB = await _context.User.FirstOrDefaultAsync(u => u.Name == user.Name && u.Password == user.Password);

            if (userDB == null)
                return NotFound();

            //JWT //NECESARIO PARA QUE FUNCIONE EL JWT
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, userDB.Name),
                new Claim("UserId", userDB.Id.ToString())
            };

            // clau a appsettings.json
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);


            var token = new JwtSecurityToken(
                _configuration["Jwt:Issuer"],
                _configuration["Jwt:Audience"],
                claims,
                expires: DateTime.Now.AddMinutes(60),
                signingCredentials: creds);

            //return Ok();
            return Ok(new { token = new JwtSecurityTokenHandler().WriteToken(token) }); //retornem el token
        }

        // DELETE: api/User/5
        [HttpDelete]
        public async Task<IActionResult> DeleteUser()
        {
            var loggedUserId = GetLoggedUserId();

            if (loggedUserId == -1)
                return Unauthorized();

            var user = await _context.User.FindAsync(loggedUserId);

            if (user == null)
            {
                return NotFound();
            }

            _context.User.Remove(user);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool UserExists(int id)
        {
            return _context.User.Any(e => e.Id == id);
        }

    }
}
