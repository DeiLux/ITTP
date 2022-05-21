using System.ComponentModel.DataAnnotations;

namespace ITTP.Datatransfer.HttpDto
{
    //Авторизация
    public class AuthRequest
    {
        /// <summary>
        /// Уникальный Логин (запрещены все символы кроме латинских букв и цифр)
        /// </summary>
        [RegularExpression(@"[0-9A-Za-z]+", ErrorMessage = "Запрещены все символы кроме латинских букв и цифр")]
        [Required]
        public string Login { get; set; }

        /// <summary>
        /// Пароль (запрещены все символы кроме латинских букв и цифр)
        /// </summary>
        [RegularExpression(@"[0-9A-Za-z]+", ErrorMessage = "Запрещены все символы кроме латинских букв и цифр")]
        [Required]
        public string Password { get; set; }
    }
}
