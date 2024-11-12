using System.Text.Json.Serialization;

namespace TrivialAPI.Model
{
    public class CategoryGames
    {
        int id;
        int categoryId;
        int userId;
        [JsonIgnore]
        User user;
        int correctAnswers;
        int totalGames;

        public CategoryGames()
        {
        }

        public CategoryGames(int categoryId, int userId, int correctAnswers, int totalGames)
        {
            this.categoryId = categoryId;
            this.userId = userId;
            this.correctAnswers = correctAnswers;
            this.totalGames = totalGames;
        }

        public int Id
        {
            get => id;
            set => id = value;
        }

        public int CategoryId
        {
            get => categoryId;
            set => categoryId = value;
        }

        public int UserId
        {
            get => userId;
            set => userId = value;
        }

        public User User
        {
            get => user;
            set => user = value;
        }

        public int CorrectAnswers
        {
            get => correctAnswers;
            set => correctAnswers = value;
        }

        public int TotalGames
        {
            get => totalGames;
            set => totalGames = value;
        }
    }
}
