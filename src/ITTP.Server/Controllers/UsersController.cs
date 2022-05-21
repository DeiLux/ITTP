using ITTP.Core.Models;
using ITTP.Core.Services;
using ITTP.Datatransfer.HttpDto;
using ITTP.Services.AuthService.Filters;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using System.Threading.Tasks;

namespace ITTP.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ILogger<UsersController> _logger;

        public UsersController(IUserService userService, ILogger<UsersController> logger)
        {
            _userService = userService;
            _logger = logger;
        }

        /// <summary>
        /// Создание пользователя по логину, паролю, имени, полу и дате рождения + указание будет ли
        /// пользователь админом (Доступно Админам)
        /// </summary>
        /// <param name="createUserRequest">Модель для создания пользователя</param>
        /// <returns>Пользователь создан</returns>
        /// <response code="200">Пользователь создан</response>
        /// <response code="400">Пользователь не создан</response>
        /// <response code="401">Пользователь не авторизован</response>
        /// <response code="500">Ошибка на стороне сервера</response>
        [Authorize(true)]
        [HttpPost("сreate-user")]
        [SwaggerOperation("Создание пользователя")]
        [SwaggerResponse(statusCode: 200, type: typeof(EmptyResult), description: "Пользователь создан")]
        [SwaggerResponse(statusCode: 400, type: typeof(EmptyResult), description: "Пользователь не создан")]
        [SwaggerResponse(statusCode: 401, type: typeof(EmptyResult), description: "Пользователь не авторизован")]
        [SwaggerResponse(statusCode: 500, type: typeof(EmptyResult), description: "Ошибка на стороне сервера")]
        public async Task<IActionResult> CreateUser([FromQuery] CreateUserRequest createUserRequest)
        {
            try
            {
                var user = (User)HttpContext.Items[nameof(AuthData)];
                var isCreateUser = await _userService.CreateUserAsync(new User()
                {
                    Guid = Guid.NewGuid(),
                    Login = createUserRequest.UserLogin,
                    Password = createUserRequest.UserPassword,
                    Name = createUserRequest.UserName,
                    Gender = createUserRequest.UserGender,
                    Birthday = createUserRequest.UserBirthday != null ?
                    DateTime.SpecifyKind(createUserRequest.UserBirthday.Value, DateTimeKind.Utc) : null,
                    IsAdmin = createUserRequest.UserIsAdmin,
                    CreatedOn = DateTime.UtcNow,
                    CreatedBy = user.Login,
                    ModifiedOn = null,
                    ModifiedBy = null,
                    RevokedOn = null,
                    RevokedBy = null,
                });

                if (!isCreateUser) return BadRequest("Пользователь не создан");

                return Ok("Пользователь создан");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in controller {controllerName} in method {methodName}.",
                     nameof(UsersController), nameof(CreateUser));
                return StatusCode(500);
            }
        }


        /// <summary>
        /// Изменение имени, пола или даты рождения пользователя лично пользователь, если он активен (отсутствует RevokedOn)
        /// </summary>
        /// <param name="changeUserRequest">Модель для изменения пользователя</param>
        /// <returns>Пользователь изменен</returns>
        /// <response code="200">Пользователь изменен</response>
        /// <response code="400">Пользователь не изменен</response>
        /// <response code="401">Пользователь не авторизован</response>
        /// <response code="500">Ошибка на стороне сервера</response>
        [Authorize(false)]
        [HttpPost("change-user")]
        [SwaggerOperation("Изменение пользователя")]
        [SwaggerResponse(statusCode: 200, type: typeof(EmptyResult), description: "Пользователь изменен")]
        [SwaggerResponse(statusCode: 400, type: typeof(EmptyResult), description: "Пользователь не изменен")]
        [SwaggerResponse(statusCode: 401, type: typeof(EmptyResult), description: "Пользователь не авторизован")]
        [SwaggerResponse(statusCode: 500, type: typeof(EmptyResult), description: "Ошибка на стороне сервера")]
        public async Task<IActionResult> ChangeUser([FromQuery] ChangeUserRequest changeUserRequest)
        {
            try
            {
                if (changeUserRequest.UserName == null &&
                    changeUserRequest.UserGender == null &&
                    changeUserRequest.UserBirthday == null)
                    return BadRequest("Пользователь не изменен");

                var user = (User)HttpContext.Items[nameof(AuthData)];
                if (user.RevokedOn != null) return BadRequest("Пользователь не изменен");
                if (changeUserRequest.UserName != null) user.Name = changeUserRequest.UserName;
                if (changeUserRequest.UserGender != null) user.Gender = (int)changeUserRequest.UserGender;
                if (changeUserRequest.UserBirthday != null) user.Birthday = DateTime.SpecifyKind(changeUserRequest.UserBirthday.Value, DateTimeKind.Utc);

                user.ModifiedBy = user.Login;
                user.ModifiedOn = DateTime.UtcNow;

                if (!await _userService.UpdateUserAsync(user)) return BadRequest("Пользователь не изменен");

                return Ok("Пользователь изменен");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in controller {controllerName} in method {methodName}.",
                     nameof(UsersController), nameof(ChangeUser));
                return StatusCode(500);
            }
        }

        /// <summary>
        /// Изменение имени, пола или даты рождения пользователя от администратора
        /// </summary>
        /// <param name="changeUserAdminRequest">Модель для изменения пользователя от администратора</param>
        /// <returns>Пользователь изменен</returns>
        /// <response code="200">Пользователь изменен</response>
        /// <response code="400">Пользователь не изменен</response>
        /// <response code="401">Пользователь не авторизован</response>
        /// <response code="500">Ошибка на стороне сервера</response>
        [Authorize(true)]
        [HttpPost("change-user-admin")]
        [SwaggerOperation("Изменение пользователя от администратора")]
        [SwaggerResponse(statusCode: 200, type: typeof(EmptyResult), description: "Пользователь изменен")]
        [SwaggerResponse(statusCode: 400, type: typeof(EmptyResult), description: "Пользователь не изменен")]
        [SwaggerResponse(statusCode: 401, type: typeof(EmptyResult), description: "Пользователь не авторизован")]
        [SwaggerResponse(statusCode: 500, type: typeof(EmptyResult), description: "Ошибка на стороне сервера")]
        public async Task<IActionResult> ChangeUserAdmin([FromQuery] ChangeUserAdminRequest changeUserAdminRequest)
        {
            try
            {
                if (changeUserAdminRequest.UserName == null &&
                    changeUserAdminRequest.UserGender == null &&
                    changeUserAdminRequest.UserBirthday == null)
                    return BadRequest("Пользователь не изменен");

                var userAdmin = (User)HttpContext.Items[nameof(AuthData)];
                var user = await _userService.ReadUserLoginAsync(changeUserAdminRequest.UserLogin);
                if (user == null) return BadRequest("Пользователь не изменен");

                if (changeUserAdminRequest.UserName != null) user.Name = changeUserAdminRequest.UserName;
                if (changeUserAdminRequest.UserGender != null) user.Gender = (int)changeUserAdminRequest.UserGender;
                if (changeUserAdminRequest.UserBirthday != null) user.Birthday = DateTime.SpecifyKind(changeUserAdminRequest.UserBirthday.Value, DateTimeKind.Utc);

                user.ModifiedBy = userAdmin.Login;
                user.ModifiedOn = DateTime.UtcNow;

                if (!await _userService.UpdateUserAsync(user)) return BadRequest("Пользователь не изменен");

                return Ok("Пользователь изменен");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in controller {controllerName} in method {methodName}.",
                     nameof(UsersController), nameof(ChangeUser));
                return StatusCode(500);
            }
        }

        /// <summary>
        /// Изменение пароля пользователь, если он активен (отсутствует RevokedOn)
        /// </summary>
        /// <param name="password">Новый пароль</param>
        /// <returns>Пользователь изменен</returns>
        /// <response code="200">Пользователь изменен</response>
        /// <response code="400">Пользователь не изменен</response>
        /// <response code="401">Пользователь не авторизован</response>
        /// <response code="500">Ошибка на стороне сервера</response>
        [Authorize(false)]
        [HttpPost("change-user-password")]
        [SwaggerOperation("Изменение пароля")]
        [SwaggerResponse(statusCode: 200, type: typeof(EmptyResult), description: "Пользователь изменен")]
        [SwaggerResponse(statusCode: 400, type: typeof(EmptyResult), description: "Пользователь не изменен")]
        [SwaggerResponse(statusCode: 401, type: typeof(EmptyResult), description: "Пользователь не авторизован")]
        [SwaggerResponse(statusCode: 500, type: typeof(EmptyResult), description: "Ошибка на стороне сервера")]
        public async Task<IActionResult> ChangeUserPassword([FromQuery,
            RegularExpression(@"[0-9A-Za-z]+",
            ErrorMessage = "Запрещены все символы кроме латинских букв и цифр"),
            Required] string password)
        {
            try
            {
                var user = (User)HttpContext.Items[nameof(AuthData)];
                if (user.RevokedOn != null) return BadRequest("Пользователь не изменен");
                user.Password = password;
                user.ModifiedBy = user.Login;
                user.ModifiedOn = DateTime.UtcNow;
                if (!await _userService.UpdateUserAsync(user)) return BadRequest("Пользователь не изменен");

                return Ok("Пользователь изменен");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in controller {controllerName} in method {methodName}.",
                     nameof(UsersController), nameof(ChangeUser));
                return StatusCode(500);
            }
        }

        /// <summary>
        /// Изменение пароля от администратора
        /// </summary>
        /// <param name="login">Логин для изменения</param>
        /// <param name="password">Новый пароль</param>
        /// <returns>Пользователь изменен</returns>
        /// <response code="200">Пользователь изменен</response>
        /// <response code="400">Пользователь не изменен</response>
        /// <response code="401">Пользователь не авторизован</response>
        /// <response code="500">Ошибка на стороне сервера</response>
        [Authorize(true)]
        [HttpPost("change-user-admin-password")]
        [SwaggerOperation("Изменение пароля")]
        [SwaggerResponse(statusCode: 200, type: typeof(EmptyResult), description: "Пользователь изменен")]
        [SwaggerResponse(statusCode: 400, type: typeof(EmptyResult), description: "Пользователь не изменен")]
        [SwaggerResponse(statusCode: 401, type: typeof(EmptyResult), description: "Пользователь не авторизован")]
        [SwaggerResponse(statusCode: 500, type: typeof(EmptyResult), description: "Ошибка на стороне сервера")]
        public async Task<IActionResult> ChangeUserAdminPassword([FromQuery,
            RegularExpression(@"[0-9A-Za-z]+",
            ErrorMessage = "Запрещены все символы кроме латинских букв и цифр"),
            Required] string login,
            [FromQuery,
            RegularExpression(@"[0-9A-Za-z]+",
            ErrorMessage = "Запрещены все символы кроме латинских букв и цифр"),
            Required] string password)
        {
            try
            {
                var userAdmin = (User)HttpContext.Items[nameof(AuthData)];
                var user = await _userService.ReadUserLoginAsync(login);
                if (user == null) return BadRequest("Пользователь не изменен");
                user.Password = password;
                user.ModifiedBy = userAdmin.Login;
                user.ModifiedOn = DateTime.UtcNow;
                if (!await _userService.UpdateUserAsync(user)) return BadRequest("Пользователь не изменен");

                return Ok("Пользователь изменен");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in controller {controllerName} in method {methodName}.",
                     nameof(UsersController), nameof(ChangeUser));
                return StatusCode(500);
            }
        }

        /// <summary>
        /// Изменение логина пользователь, если он активен (отсутствует RevokedOn)
        /// </summary>
        /// <param name="login">Новый логин</param>
        /// <returns>Пользователь изменен</returns>
        /// <response code="200">Пользователь изменен</response>
        /// <response code="400">Пользователь не изменен</response>
        /// <response code="401">Пользователь не авторизован</response>
        /// <response code="500">Ошибка на стороне сервера</response>
        [Authorize(false)]
        [HttpPost("change-user-login")]
        [SwaggerOperation("Изменение логина")]
        [SwaggerResponse(statusCode: 200, type: typeof(EmptyResult), description: "Пользователь изменен")]
        [SwaggerResponse(statusCode: 400, type: typeof(EmptyResult), description: "Пользователь не изменен")]
        [SwaggerResponse(statusCode: 401, type: typeof(EmptyResult), description: "Пользователь не авторизован")]
        [SwaggerResponse(statusCode: 500, type: typeof(EmptyResult), description: "Ошибка на стороне сервера")]
        public async Task<IActionResult> ChangeUserLogin([FromQuery,
            RegularExpression(@"[0-9A-Za-z]+",
            ErrorMessage = "Запрещены все символы кроме латинских букв и цифр"),
            Required] string login)
        {
            try
            {
                var user = (User)HttpContext.Items[nameof(AuthData)];
                if (user.RevokedOn != null) return BadRequest("Пользователь не изменен");

                user.ModifiedBy = login;
                user.ModifiedOn = DateTime.UtcNow;

                if (user.IsAdmin)
                {
                    string oldLogin = user.Login;
                    user.Login = login;
                    if (!await _userService.UpdateAdminLoginAsync(user, oldLogin)) return BadRequest("Пользователь не изменен");
                }
                else
                {
                    user.Login = login;
                    if (!await _userService.UpdateUserAsync(user)) return BadRequest("Пользователь не изменен");
                }


                return Ok("Пользователь изменен");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in controller {controllerName} in method {methodName}.",
                     nameof(UsersController), nameof(ChangeUser));
                return StatusCode(500);
            }
        }

        /// <summary>
        /// Изменение логина от администратора
        /// </summary>
        /// <param name="login">Логин для изменения</param>
        /// <param name="loginNew">Новый логин</param>
        /// <returns>Пользователь изменен</returns>
        /// <response code="200">Пользователь изменен</response>
        /// <response code="400">Пользователь не изменен</response>
        /// <response code="401">Пользователь не авторизован</response>
        /// <response code="500">Ошибка на стороне сервера</response>
        [Authorize(true)]
        [HttpPost("change-user-admin-login")]
        [SwaggerOperation("Изменение логина")]
        [SwaggerResponse(statusCode: 200, type: typeof(EmptyResult), description: "Пользователь изменен")]
        [SwaggerResponse(statusCode: 400, type: typeof(EmptyResult), description: "Пользователь не изменен")]
        [SwaggerResponse(statusCode: 401, type: typeof(EmptyResult), description: "Пользователь не авторизован")]
        [SwaggerResponse(statusCode: 500, type: typeof(EmptyResult), description: "Ошибка на стороне сервера")]
        public async Task<IActionResult> ChangeUserAdminLogin([FromQuery,
            RegularExpression(@"[0-9A-Za-z]+",
            ErrorMessage = "Запрещены все символы кроме латинских букв и цифр"),
            Required] string login,
            [FromQuery,
            RegularExpression(@"[0-9A-Za-z]+",
            ErrorMessage = "Запрещены все символы кроме латинских букв и цифр"),
            Required] string loginNew)
        {
            try
            {
                var userAdmin = (User)HttpContext.Items[nameof(AuthData)];
                var user = await _userService.ReadUserLoginAsync(login);
                if (user == null || user.IsAdmin) return BadRequest("Пользователь не изменен");
                user.Login = loginNew;
                user.ModifiedBy = userAdmin.Login;
                user.ModifiedOn = DateTime.UtcNow;
                if (!await _userService.UpdateUserAsync(user)) return BadRequest("Пользователь не изменен");

                return Ok("Пользователь изменен");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in controller {controllerName} in method {methodName}.",
                     nameof(UsersController), nameof(ChangeUser));
                return StatusCode(500);
            }
        }

        /// <summary>
        /// Запрос списка всех активных (отсутствует RevokedOn) пользователей, список отсортирован по 
        /// CreatedOn (Доступно Админам)
        /// </summary>
        /// <returns>Cписк всех активных пользователей</returns>
        /// <response code="200">Cписк всех активных пользователей</response>
        /// <response code="401">Пользователь не авторизован</response>
        /// <response code="500">Ошибка на стороне сервера</response>
        [Authorize(true)]
        [HttpGet("read-users-revoked-null-admin")]
        [SwaggerResponse(statusCode: 200, type: typeof(EmptyResult), description: "Cписк всех активных пользователей")]
        [SwaggerResponse(statusCode: 401, type: typeof(EmptyResult), description: "Пользователь не авторизован")]
        [SwaggerResponse(statusCode: 500, type: typeof(EmptyResult), description: "Ошибка на стороне сервера")]
        public async Task<IActionResult> ReadUsersRevokedOnNullAsync()
        {
            try
            {
                var users = await _userService.ReadUsersRevokedOnNullAsync();

                return Ok(users);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in controller {controllerName} in method {methodName}.",
                     nameof(UsersController), nameof(ChangeUser));
                return StatusCode(500);
            }
        }

        /// <summary>
        /// Запрос пользователя по логину, в списке долны быть имя, пол и дата рождения, статус активный 
        /// или нет (Доступно Админам)
        /// </summary>
        /// <param name="login">Логин пользователя</param>
        /// <returns>Списк имя,пол и дата рождения, статус активный или нет</returns>
        /// <response code="200">Списк имя,пол и дата рождения, статус активный или нет</response>
        /// <response code="400">Пользователь не найден</response>
        /// <response code="401">Пользователь не авторизован</response>
        /// <response code="500">Ошибка на стороне сервера</response>
        [Authorize(true)]
        [HttpPost("read-user-admin")]
        [SwaggerResponse(statusCode: 200, type: typeof(EmptyResult), description: "Списк имя,пол и дата рождения, статус активный или нет")]
        [SwaggerResponse(statusCode: 400, type: typeof(EmptyResult), description: "Пользователь не найден")]
        [SwaggerResponse(statusCode: 401, type: typeof(EmptyResult), description: "Пользователь не авторизован")]
        [SwaggerResponse(statusCode: 500, type: typeof(EmptyResult), description: "Ошибка на стороне сервера")]
        public async Task<IActionResult> ReadUserLoginAsync([FromQuery,
            RegularExpression(@"[0-9A-Za-z]+",
            ErrorMessage = "Запрещены все символы кроме латинских букв и цифр"),
            Required] string login)
        {
            try
            {
                var user = await _userService.ReadUserLoginAsync(login);
                if (user == null) return BadRequest($"Пользователь login \"{login}\" не найден");

                return Ok(JsonSerializer.Serialize(new
                {
                    Name = user.Name,
                    Gender = user.Gender,
                    Birthday = user.Birthday,
                    CreatedBy = user.CreatedBy,
                }));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in controller {controllerName} in method {methodName}.",
                     nameof(UsersController), nameof(ChangeUser));
                return StatusCode(500);
            }
        }

        /// <summary>
        /// Запрос пользователя (Доступно только самому пользователю, если он активен (отсутствует RevokedOn)
        /// </summary>
        /// <returns>Пользователь</returns>
        /// <response code="200">Пользователь</response>
        /// <response code="400">Пользователь не найден</response>
        /// <response code="401">Пользователь не авторизован</response>
        /// <response code="500">Ошибка на стороне сервера</response>
        [Authorize(false)]
        [HttpGet("read-user")]
        [SwaggerResponse(statusCode: 200, type: typeof(EmptyResult), description: "Пользователь")]
        [SwaggerResponse(statusCode: 400, type: typeof(EmptyResult), description: "Пользователь неактивный")]
        [SwaggerResponse(statusCode: 401, type: typeof(EmptyResult), description: "Пользователь не авторизован")]
        [SwaggerResponse(statusCode: 500, type: typeof(EmptyResult), description: "Ошибка на стороне сервера")]
        public async Task<IActionResult> ReadUserAsync()
        {
            try
            {
                var user = (User)HttpContext.Items[nameof(AuthData)];
                user = await _userService.ReadUserAsync(user.Guid);

                if (user.RevokedOn == null) BadRequest("Пользователь неактивный");

                return Ok(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in controller {controllerName} in method {methodName}.",
                     nameof(UsersController), nameof(ChangeUser));
                return StatusCode(500);
            }
        }

        /// <summary>
        /// Запрос всех пользователей старше определённого возраста (Доступно Админам)
        /// </summary>
        /// <param name="age">Возраста</param>
        /// <returns>Пользователи старше определённого возраста</returns>
        /// <response code="200">Пользователи старше определённого возраста"</response>
        /// <response code="400">Пользователи не найдены</response>
        /// <response code="401">Пользователь не авторизован</response>
        /// <response code="500">Ошибка на стороне сервера</response>
        [Authorize(true)]
        [HttpPost("read-users-age")]
        [SwaggerResponse(statusCode: 200, type: typeof(EmptyResult), description: "Пользователи старше определённого возраста")]
        [SwaggerResponse(statusCode: 400, type: typeof(EmptyResult), description: "Пользователи не найдены")]
        [SwaggerResponse(statusCode: 401, type: typeof(EmptyResult), description: "Пользователь не авторизован")]
        [SwaggerResponse(statusCode: 500, type: typeof(EmptyResult), description: "Ошибка на стороне сервера")]
        public async Task<IActionResult> ReadUsersAsync([FromQuery, Required] int age)
        {
            try
            {
                var users = await _userService.ReadUsersAgeAsync(age);

                if (users.Count == 0) BadRequest("Пользователи не найдены");

                return Ok(users);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in controller {controllerName} in method {methodName}.",
                     nameof(UsersController), nameof(ChangeUser));
                return StatusCode(500);
            }
        }

        /// <summary>
        /// Удаление пользователя по логину полное (Доступно Админам)
        /// </summary>
        /// <param name="login">Логин</param>
        /// <returns>Пользователь удален</returns>
        /// <response code="200">Пользователи удален"</response>
        /// <response code="400">Пользователи не удален</response>
        /// <response code="401">Пользователь не авторизован</response>
        /// <response code="500">Ошибка на стороне сервера</response>
        [Authorize(true)]
        [HttpDelete("delete-user")]
        [SwaggerResponse(statusCode: 200, type: typeof(EmptyResult), description: "Пользователь удален")]
        [SwaggerResponse(statusCode: 400, type: typeof(EmptyResult), description: "Пользователь не удален")]
        [SwaggerResponse(statusCode: 401, type: typeof(EmptyResult), description: "Пользователь не авторизован")]
        [SwaggerResponse(statusCode: 500, type: typeof(EmptyResult), description: "Ошибка на стороне сервера")]
        public async Task<IActionResult> DeleteUserFullAsync([FromQuery,
            RegularExpression(@"[0-9A-Za-z]+",
            ErrorMessage = "Запрещены все символы кроме латинских букв и цифр"),
            Required] string login)
        {
            try
            {
                var isDelete = await _userService.DeleteUserFullAsync(login);

                if (!isDelete) BadRequest("Пользователь не удален");

                return Ok("Пользователь удален");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in controller {controllerName} in method {methodName}.",
                     nameof(UsersController), nameof(ChangeUser));
                return StatusCode(500);
            }
        }

        /// <summary>
        /// Удаление пользователя по логину мягкое должна происходить простановка RevokedOn и RevokedBy (Доступно Админам)
        /// </summary>
        /// <param name="login">Логин</param>
        /// <returns>Удаление пользователя</returns>
        /// <response code="200">Пользователи удален"</response>
        /// <response code="400">Пользователи не удален</response>
        /// <response code="401">Пользователь не авторизован</response>
        /// <response code="500">Ошибка на стороне сервера</response>
        [Authorize(true)]
        [HttpPost("delete-soft-user")]
        [SwaggerResponse(statusCode: 200, type: typeof(EmptyResult), description: "Пользователь удален")]
        [SwaggerResponse(statusCode: 400, type: typeof(EmptyResult), description: "Пользователь не удален")]
        [SwaggerResponse(statusCode: 401, type: typeof(EmptyResult), description: "Пользователь не авторизован")]
        [SwaggerResponse(statusCode: 500, type: typeof(EmptyResult), description: "Ошибка на стороне сервера")]
        public async Task<IActionResult> DeleteUserSoftAsync([FromQuery,
            RegularExpression(@"[0-9A-Za-z]+",
            ErrorMessage = "Запрещены все символы кроме латинских букв и цифр"),
            Required] string login)
        {
            try
            {
                var userAdmin = (User)HttpContext.Items[nameof(AuthData)];
                var user = await _userService.ReadUserLoginAsync(login);
                if (user == null) return BadRequest("Пользователь не удален");
                user.RevokedBy = userAdmin.Login;
                user.RevokedOn = DateTime.UtcNow;
                user.ModifiedBy = userAdmin.Login;
                user.ModifiedOn = DateTime.UtcNow;
                if (!await _userService.UpdateUserAsync(user)) return BadRequest("Пользователь не удален");

                return Ok("Пользователь удален");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in controller {controllerName} in method {methodName}.",
                     nameof(UsersController), nameof(ChangeUser));
                return StatusCode(500);
            }
        }

        /// <summary>
        /// Восстановление пользователя - Очистка полей (RevokedOn, RevokedBy) (Доступно Админам)
        /// </summary>
        /// <param name="login">Логин</param>
        /// <returns>Пользователь восстановлен</returns>
        /// <response code="200">Пользователи восстановлен"</response>
        /// <response code="400">Пользователи не восстановлен</response>
        /// <response code="401">Пользователь не авторизован</response>
        /// <response code="500">Ошибка на стороне сервера</response>
        [Authorize(true)]
        [HttpPost("recovery-user")]
        [SwaggerResponse(statusCode: 200, type: typeof(EmptyResult), description: "Пользователь восстановлен")]
        [SwaggerResponse(statusCode: 400, type: typeof(EmptyResult), description: "Пользователь не восстановлен")]
        [SwaggerResponse(statusCode: 401, type: typeof(EmptyResult), description: "Пользователь не авторизован")]
        [SwaggerResponse(statusCode: 500, type: typeof(EmptyResult), description: "Ошибка на стороне сервера")]
        public async Task<IActionResult> RecoveryUserAsync([FromQuery,
            RegularExpression(@"[0-9A-Za-z]+",
            ErrorMessage = "Запрещены все символы кроме латинских букв и цифр"),
            Required] string login)
        {
            try
            {
                var userAdmin = (User)HttpContext.Items[nameof(AuthData)];
                var user = await _userService.ReadUserLoginAsync(login);
                if (user == null) return BadRequest("Пользователь не восстановлен");
                user.RevokedBy = null;
                user.RevokedOn = null;
                user.ModifiedBy = userAdmin.Login;
                user.ModifiedOn = DateTime.UtcNow;
                if (!await _userService.UpdateUserAsync(user)) return BadRequest("Пользователь не авторизован");

                return Ok("Пользователь восстановлен");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in controller {controllerName} in method {methodName}.",
                     nameof(UsersController), nameof(ChangeUser));
                return StatusCode(500);
            }
        }
    }
}
