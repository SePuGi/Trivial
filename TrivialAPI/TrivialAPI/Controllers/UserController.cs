using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TrivialAPI;
using TrivialAPI.Model;

namespace TrivialAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public UserController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/User
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUser()
        {
            return await _context.User.ToListAsync();
        }

        // GET: api/User/5
        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUser(int id)
        {
            var user = await _context.User.FindAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            return user;
        }

        // PUT: api/User/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUser(int id, User user)
        {
            if (id != user.Id)
            {
                return BadRequest();
            }

            if(Utils.CheckPassword(user.Password))
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
                if (!UserExists(id))
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

        // POST: /api/User/register
        [HttpPost("login")]
        public async Task<ActionResult<User>> LoginUser(User user)
        {
            user.Password = Utils.EncryptPassword(user.Password);

            var userDB = await _context.User.FirstOrDefaultAsync(u => u.Name == user.Name && u.Password == user.Password);

            if (userDB == null)
                return NotFound();

            return Ok();
            //return Ok(user); no creo que haga falta devolver el usuario
        }

        // DELETE: api/User/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _context.User.FindAsync(id);
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
