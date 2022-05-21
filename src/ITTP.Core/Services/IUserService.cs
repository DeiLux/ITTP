using ITTP.Core.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ITTP.Core.Services
{
    public interface IUserService
    {
        /// <summary>
        /// Создание пользователя по логину, паролю, имени, полу и дате рождения + указание будет ли
        /// пользователь админом (Доступно Админам)
        /// </summary>
        /// <param name="user">User кор модель</param>
        /// <returns>true - User добавлен в базу данных, false - User не добавлен в базу данных</returns>
        public Task<bool> CreateUserAsync(User user);

        /// <summary>
        /// Изменение User
        /// </summary>
        /// <param name="user">User кор модель</param>
        /// <returns>true - User изменен, false - User не изменен</returns>
        public Task<bool> UpdateUserAsync(User user);

        /// <summary>
        /// Запрос списка всех активных (отсутствует RevokedOn) пользователей, список отсортирован по
        /// CreatedOn
        /// </summary>
        /// <returns>Список User кор модель</returns>
        public Task<List<User?>> ReadUsersAsync();

        /// <summary>
        /// Запрос пользователя по логину  в списке долны быть имя, пол и дата рождения статус активный
        /// или нет
        /// </summary>
        /// <param name="login">Уникальный Логин (запрещены все символы кроме латинских букв и цифр)</param>
        /// <returns>User кор модель</returns>
        public Task<User?> ReadUserLoginAsync(string login);

        /// <summary>
        /// Запрос пользователя по логину и паролю 
        /// </summary>
        /// <param name="login">Уникальный Логин (запрещены все символы кроме латинских букв и цифр)</param>
        /// <param name="password">Пароль (запрещены все символы кроме латинских букв и цифр)</param>
        /// <returns>User кор модель</returns>
        public Task<User?> ReadUserAsync(string login, string password);

        /// <summary>
        /// Запрос всех пользователей старше определённого возраста 
        /// </summary>
        /// <param name="age">Возраста пользователей</param>
        /// <returns>Список User кор модель</returns>
        public Task<List<User?>> ReadUsersAgeAsync(int age);

        /// <summary>
        /// Удаление пользователя по логину полное 
        /// </summary>
        /// <param name="login">Уникальный Логин (запрещены все символы кроме латинских букв и цифр)</param>
        /// <returns>true - User удален, false - User не удален</returns>
        public Task<bool> DeleteUserFullAsync(string login);

        /// <summary>
        /// Запрос списка всех активных (отсутствует RevokedOn) пользователей, список отсортирован по
        /// CreatedOn
        /// </summary>
        /// <returns>Список User кор модель</returns>
        Task<List<User?>> ReadUsersRevokedOnNullAsync();

        /// <summary>
        /// Запрос пользователя по guid
        /// </summary>
        /// <param name="guid">guid</param>
        /// <returns>User кор модель</returns>
        Task<User?> ReadUserAsync(Guid guid);

        /// <summary>
        /// Изменение логина админ
        /// </summary>
        /// <param name="user">User кор модель</param>
        /// <param name="oldLogin">Старый логин</param>
        /// <returns>true - User изменен, false - User не изменен</returns>
        Task<bool> UpdateAdminLoginAsync(User user, string oldLogin);
    }
}
