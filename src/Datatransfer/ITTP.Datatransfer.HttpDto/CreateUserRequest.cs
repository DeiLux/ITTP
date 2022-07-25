using System.ComponentModel.DataAnnotations;

namespace ITTP.Datatransfer.HttpDto
{
    //Модель для создания пользователя
    public class CreateUserRequest
    {
        /// <summary>
        /// Уникальный Логин (запрещены все символы кроме латинских букв и цифр)
        /// </summary>
        [RegularExpression(@"[0-9A-Za-z]+", ErrorMessage = "Запрещены все символы кроме латинских букв и цифр")]
        [Required]
        public string? UserLogin { get; set; }

        /// <summary>
        /// Пароль (запрещены все символы кроме латинских букв и цифр)
        /// </summary>
        [RegularExpression(@"[0-9A-Za-z]+", ErrorMessage = "Запрещены все символы кроме латинских букв и цифр")]
        [Required]
        public string? UserPassword { get; set; }

        /// <summary>
        /// Имя (запрещены все символы кроме латинских и русских букв)
        /// </summary>
        [RegularExpression(@"[A-Za-zА-Яа-я]+", ErrorMessage = "Запрещены все символы кроме латинских и русских букв")]
        [Required]
        public string? UserName { get; set; }

        /// <summary>
        /// Пол 0 - женщина, 1 - мужчина, 2 - неизвестно
        /// </summary>
        [RegularExpression(@"[0-2]", ErrorMessage = "Пол 0 - женщина, 1 - мужчина, 2 - неизвестно")]
        [Required]
        public int UserGender { get; set; }

        /// <summary>
        /// Поле даты рождения может быть Null
        /// </summary>
        public DateTime? UserBirthday { get; set; }

        /// <summary>
        /// Указание - является ли пользователь админом
        /// </summary>
        [Required]
        public bool UserIsAdmin { get; set; }
    }
}
