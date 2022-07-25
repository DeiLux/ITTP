using ITTP.Core.Exceptions;
using ITTP.Core.Models;

namespace ITTP.Core.Services
{
    public interface IAuthService
    {
        /// <summary>
        /// Создание токина
        /// </summary>
        /// <param name="login">Уникальный Логин (запрещены все символы кроме латинских букв и цифр)</param>
        /// <param name="password">Пароль (запрещены все символы кроме латинских букв и цифр)</param>
        /// <returns></returns>
        /// <exception cref="AuthException">Исключение при авторизации</exception>
        Task<AuthData?> CreateTokenAsync(string login, string password);
    }
}
