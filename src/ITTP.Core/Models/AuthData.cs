namespace ITTP.Core.Models
{
    /// <summary>
    /// Модель для авторизации
    /// </summary>
    public class AuthData
    {
        /// <summary>
        /// Уникальный Логин (запрещены все символы кроме латинских букв и цифр)
        /// </summary>
        public string Login { get; set; }

        /// <summary>
        /// Указание - является ли пользователь админом
        /// </summary>
        public bool IsAdmin { get; set; }
        
        /// <summary>
        /// Токен
        /// </summary>
        public string Token { get; set; }

        public AuthData(string login, bool isAdmin, string token)
        {
            Login = login;
            IsAdmin = isAdmin;
            Token = token;
        }
    }
}
