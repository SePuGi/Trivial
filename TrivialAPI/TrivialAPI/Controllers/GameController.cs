using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using System.Text.Json;
using TrivialAPI.Model;
using TrivialAPI.Model.Enum;
using Newtonsoft.Json;

namespace TrivialAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GameController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public GameController(ApplicationDbContext context)
        {
            _context = context;
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
            catch(Exception e)
            {
                return -1;
            }

            return idUser;
        }

        //POST: /api/SaveGame
        [HttpPost("SaveGame")]
        public async Task<ActionResult<User>> SaveGame(int categoryId, int correctAnswers)
        {
            var loggedUserId = GetLoggedUserId();

            if (loggedUserId == -1)
                return Unauthorized();

            var userDB = await _context.User.Include(u => u.CategoryGames).FirstOrDefaultAsync(u => u.Id == loggedUserId);

            if (userDB == null)
                return NotFound();
            
            if (userDB.CategoryGames == null)
                userDB.CategoryGames = new List<CategoryGames>();

            //sumamos el total de respuestas correctas a las que ya tenia el usuario
            if (!userDB.CategoryGames.Exists(cg => cg.Id == categoryId))
                await _context.CategoryGames.AddAsync(new CategoryGames(categoryId, loggedUserId, correctAnswers, 1));
            //userDB.CategoryGames.Add(new CategoryGames(category, userId, correctAnswers, 1));
            else
            {
                userDB.CategoryGames.Find(cg => cg.Id == categoryId).CorrectAnswers += correctAnswers;
                userDB.CategoryGames.Find(cg => cg.Id == categoryId).TotalGames++;
            }

            await _context.SaveChangesAsync();

            return Ok();
        }

        //GET: /api/category
        [HttpGet("category")]
        public ActionResult<List<Category>> GetCategories()
        {
            List<Category> categories = new List<Category>();

            foreach(CategoryEnum category in Enum.GetValues(typeof(CategoryEnum)))
            {
                categories.Add(new Category((int)category, category.ToString()));
            }

            return Ok(categories);
        }


        #region Estadisticas

        //Mostrar els 10 usuaris amb més partides jugades
        //GET: /api/User/RankingMostGames/categoryId
        [HttpGet("RankingMostGames/{categoryId}")]
        public async Task<ActionResult<IEnumerable<User>>> RankingMostGames(int categoryId)
        {
            var users =  _context.User.Include(u => u.CategoryGames).Where(u => u.CategoryGames.Count > 0); //obtenemos todos los usuarios que han jugado alguna partida

            //obtenemos los usuarios que han jugado la categoria
            var usersWhoPlayedCategory = users.Where(u => u.CategoryGames.Any(cg => cg.CategoryId == categoryId)).ToList();

            //ordenamos los usuarios por el total de partidas jugadas
            var ranking = usersWhoPlayedCategory.OrderByDescending(u => u.CategoryGames).Take(10).ToList();

            var options = new JsonSerializerOptions
            {
                ReferenceHandler = ReferenceHandler.IgnoreCycles
            };

            var result = ranking.Select(u => new
            {
                u.Id,
                u.Name,
                totalGames = u.CategoryGames.Sum(cg => cg.TotalGames),
                categoryId = categoryId
            });

            return new JsonResult(result, options);
        }

        //Mostrar els 10 usuaris amb més punts
        //GET: /api/User/RankingMostPoints/categoryId
        [HttpGet("RankingMostPoints/{categoryId}")]
        public async Task<ActionResult<IEnumerable<User>>> RankingMostPoints(int categoryId)
        {
            var users = _context.User.Include(u => u.CategoryGames).Where(u => u.CategoryGames.Count > 0); //obtenemos todos los usuarios que han jugado alguna partida

            //obtenemos los usuarios que han jugado la categoria
            var usersWhoPlayedCategory = users.Where(u => u.CategoryGames.Any(cg => cg.CategoryId == categoryId)).ToList();

            //ordenamos los usuarios por el total de respuestas correctas
            var ranking = usersWhoPlayedCategory.OrderByDescending(u => u.CategoryGames.Find(cg => cg.CategoryId == categoryId).CorrectAnswers).Take(10).ToList();

            var options = new JsonSerializerOptions
            {
                ReferenceHandler = ReferenceHandler.IgnoreCycles
            };

            var result = ranking.Select(u => new
            {
                u.Id,
                u.Name,
                totalCorrectAnswers = u.CategoryGames.Sum(cg => cg.CorrectAnswers),
                categoryId = categoryId
            });

            return new JsonResult(result, options);
        }

        //Mostrar els 10 usuaris amb la millor relació de partides jugades i puntuacio total
        [HttpGet("RankingGlobal")]
        public async Task<ActionResult<IEnumerable<User>>> RankingGlobal()
        {
            var users = _context.User.Include(u => u.CategoryGames).Where(u => u.CategoryGames.Count > 0); //obtenemos todos los usuarios que han jugado alguna partida

            var ranking = users.OrderByDescending(u => u.CategoryGames.Sum(cg => cg.CorrectAnswers / cg.TotalGames)).Take(10).ToList();

            var options = new JsonSerializerOptions
            {
                ReferenceHandler = ReferenceHandler.IgnoreCycles
            };

            //por cada usuario añadir un parametro con la media de respuestas correctas en un nuevo objeto, no hace falta guardar CategoryGames
            var result = ranking.Select(u => new
            {
                u.Id,
                u.Name,
                avgCorrectAnswers = u.CategoryGames.Sum(cg => cg.CorrectAnswers / cg.TotalGames)
            });

            return new JsonResult(result, options);
        }
        
        #endregion
    }
}
