using ITTP.Core.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ITTP.Core.Repositories
{
    public interface IUserRepository
    {
        /// <summary>
        /// Создание пользователя
        /// </summary>
        /// <param name="user">User кор модель</param>
        public Task CreateUserAsync(User user);

        /// <summary>
        /// Изменение пользователя
        /// </summary>
        /// <param name="user">User кор модель</param>
        public Task UpdateUserAsync(User user);

        /// <summary>
        /// Cписка всех пользователей
        /// </summary>
        /// <returns>Cписка всех пользователей</returns>
        public Task<List<User?>> ReadUsersAsync();

        /// <summary>
        /// Запрос пользователя по логину
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
        public Task DeleteUserFullAsync(string login);

        /// <summary>
        /// Проверяет, соответствуют ли элементы условию
        /// </summary>
        /// <param name="guid">Уникальный идентификатор пользователя</param>
        /// <returns>true - если хотя бы один элемент коллекции определенному условию,
        /// false - элементы коллекции не подходят по условию</returns>
        Task<bool> UserExistsAsync(Guid guid);

        /// <summary>
        /// Проверяет, соответствуют ли элементы условию
        /// </summary>
        /// <param name="login">Уникальный Логин (запрещены все символы кроме латинских букв и цифр)</param>
        /// <returns>true - если хотя бы один элемент коллекции определенному условию,
        /// false - элементы коллекции не подходят по условию</returns>
        Task<bool> UserExistsAsync(string login);

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
        /// Запрос на список пользователей где есть логин в полях CreatedBy, ModifiedBy, RevokedBy 
        /// </summary>
        /// <param name="login">Уникальный Логин (запрещены все символы кроме латинских букв и цифр)</param>
        /// <returns>Список User кор модель</returns>
        Task<List<User?>> ReadUsersUpdateAsync(string login);
    }
}
