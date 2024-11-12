namespace TrivialAPI.Model
{
    public class User
    {
        int id;
        string name;
        string password;

        List<CategoryGames> categoryGames;

        public User(int id, string name, string password)
        {
            this.id = id;
            this.name = name;
            this.password = password;

            categoryGames = new List<CategoryGames>();
        }

        public User()
        {
        }

        public List<CategoryGames> CategoryGames
        {
            get => categoryGames;
            set => categoryGames = value;
        }

        public int Id
        {
            get => id;
            set => id = value;
        }

        public string Name
        {
            get => name;
            set => name = value;
        }

        public string Password
        {
            get => password;
            set => password = value;
                
            
        }
    }
}
