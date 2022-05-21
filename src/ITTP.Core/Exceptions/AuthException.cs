using System;

namespace ITTP.Core.Exceptions
{
    /// <summary>
    /// Исключение при нахождении пользователя в бд
    /// </summary>
    public class AuthException : ApplicationException
    {
        public AuthException(string message) : base(message)
        {
        }
    }
}
