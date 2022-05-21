using System;
using System.ComponentModel.DataAnnotations;

namespace ITTP.Datatransfer.HttpDto
{
    //Модель для изменения пользователя
    public class ChangeUserRequest
    {
        /// <summary>
        /// Имя (запрещены все символы кроме латинских и русских букв)
        /// </summary>
        [RegularExpression(@"[A-Za-zА-Яа-я]+", ErrorMessage = "Запрещены все символы кроме латинских и русских букв")]
        public string UserName { get; set; }

        /// <summary>
        /// Пол 0 - женщина, 1 - мужчина, 2 - неизвестно
        /// </summary>
        [RegularExpression(@"[0-2]", ErrorMessage = "Пол 0 - женщина, 1 - мужчина, 2 - неизвестно")]
        public int? UserGender { get; set; }

        /// <summary>
        /// Поле даты рождения может быть Null
        /// </summary>
        public DateTime? UserBirthday { get; set; }
    }
}
