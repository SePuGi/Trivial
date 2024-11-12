using Microsoft.AspNetCore.Mvc;
using TrivialAPI.Model;

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

        //POST: /api/SaveGame
        [HttpPost("SaveGame")]
        public async Task<ActionResult<User>> SaveGame(int userId, int category, int correctAnswers)
        {
            var userDB = await _context.User.FindAsync(userId);

            if (userDB == null)
                return NotFound();

            if (userDB.CategoryGames == null)
                userDB.CategoryGames = new List<CategoryGames>();

            //sumamos el total de respuestas correctas a las que ya tenia el usuario
            if (!userDB.CategoryGames.Exists(cg => cg.Id == category))
                await _context.CategoryGames.AddAsync(new CategoryGames(category, userId, correctAnswers, 1));
            //userDB.CategoryGames.Add(new CategoryGames(category, userId, correctAnswers, 1));
            else
            {
                userDB.CategoryGames.Find(cg => cg.Id == category).CorrectAnswers += correctAnswers;
                userDB.CategoryGames.Find(cg => cg.Id == category).TotalGames++;
            }

            await _context.SaveChangesAsync();

            return Ok();
        }


        #region Estadisticas
        
        //Mostrar els 10 usuaris amb més partides jugades
        //GET: /api/User/RankingMostGames/categoryId
        [HttpGet("RankingMostGames/{categoryId}")]
        public async Task<ActionResult<IEnumerable<User>>> RankingMostGames(int categoryId)
        {
            var users =  _context.User.Where(u => u.CategoryGames.Count > 0); //obtenemos todos los usuarios que han jugado alguna partida

            //obtenemos los usuarios que han jugado la categoria
            var usersWhoPlayedCategory = users.Where(u => u.CategoryGames.Any(cg => cg.CategoryId == categoryId)).ToList();

            //ordenamos los usuarios por el total de partidas jugadas
            var ranking = usersWhoPlayedCategory.OrderByDescending(u => u.CategoryGames).Take(10).ToList();

            return ranking;
        }

        //Mostrar els 10 usuaris amb més punts
        //GET: /api/User/RankingMostPoints/categoryId
        [HttpGet("RankingMostPoints/{categoryId}")]
        public async Task<ActionResult<IEnumerable<User>>> RankingMostPoints(int categoryId)
        {
            var users = _context.User.Where(u => u.CategoryGames.Count > 0); //obtenemos todos los usuarios que han jugado alguna partida

            //obtenemos los usuarios que han jugado la categoria
            var usersWhoPlayedCategory = users.Where(u => u.CategoryGames.Any(cg => cg.CategoryId == categoryId)).ToList();

            //AQUI PETA - PORQUE CategoryGames ES NULL, LA PRIMERA SOLUCION QUE SE ME OCURRE ES
            //  HACER UNA LISTA DEL LOS ID DE LOS USUARIOS Y HACER UNA CONSULTA PARA OBTENER LOS DATOS QUE QUEREMOS
            //ordenamos los usuarios por el total de respuestas correctas
            var ranking = usersWhoPlayedCategory.OrderByDescending(u => u.CategoryGames.Find(cg => cg.CategoryId == categoryId).CorrectAnswers).Take(10).ToList();

            return ranking;
        }

        //Mostrar els 10 usuaris amb la millor relació de partides jugades i puntuacio total
        [HttpGet("RankingGlobal")]
        public async Task<ActionResult<IEnumerable<User>>> RankingGlobal()
        {
            var users = _context.User.Where(u => u.CategoryGames.Count > 0); //obtenemos todos los usuarios que han jugado alguna partida

            var ranking = users.OrderByDescending(u => u.CategoryGames.Sum(cg => cg.CorrectAnswers / cg.TotalGames)).Take(10).ToList();

            return ranking;
        }
        
        #endregion
    }
}
