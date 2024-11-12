using System.Security.Cryptography;
using System.Text;

namespace TrivialAPI.Model
{
    public static class Utils
    {
        public static string EncryptPassword(string password)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(password));

                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }

        //comprobar que la contraseña té més de 6 caràcters, un número, una lletra majúscula i una minúscula i un caràcter especial
        //retornar true si la contraseña és vàlida, false si no ho és
        public static bool CheckPassword(string value)
        {
            //comprobar que la contraseña té més de 6 caràcters
            if (value.Length < 6)
            {
                return false;
            }

            //comprobar que la contraseña té un número
            //comprobar que la contraseña té una lletra majúscula
            //comprobar que la contraseña té una lletra minúscula
            bool hasNumber = false;
            bool hasUpper = false;
            bool hasLower = false;
            foreach (char c in value)
            {
                if (char.IsDigit(c))
                    hasNumber = true;

                if (char.IsLower(c))
                    hasLower = true;

                if (char.IsUpper(c))
                    hasUpper = true;


                if (hasNumber && hasUpper && hasLower)
                    break;

            }

            if (!hasNumber)
                return false;

            if (!hasUpper)
                return false;

            if (!hasLower)
                return false;

            return true;
        }
    }
}
