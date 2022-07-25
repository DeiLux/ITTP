using ITTP.Core.Models;
using ITTP.Core.Services;
using ITTP.Datatransfer.HttpDto;
using ITTP.Services.AuthService.Filters;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;

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

                var user = (User)HttpContext.Items[nameof(AuthData)]!;
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
                var user = (User)HttpContext.Items[nameof(AuthData)]!;
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
                     nameof(UsersController), nameof(ChangeUserPassword));
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
                var user = (User)HttpContext.Items[nameof(AuthData)]!;
                if (user.RevokedOn != null) return BadRequest("Пользователь не изменен");

                user.ModifiedBy = login;
                user.ModifiedOn = DateTime.UtcNow;

                user.Login = login;
                if (!await _userService.UpdateUserAsync(user)) return BadRequest("Пользователь не изменен");

                return Ok("Пользователь изменен");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in controller {controllerName} in method {methodName}.",
                     nameof(UsersController), nameof(ChangeUserLogin));
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
                var user = (User)HttpContext.Items[nameof(AuthData)]!;
                user = await _userService.ReadUserAsync(user.Guid);

                if (user!.RevokedOn == null) BadRequest("Пользователь неактивный");

                return Ok(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in controller {controllerName} in method {methodName}.",
                     nameof(UsersController), nameof(ReadUserAsync));
                return StatusCode(500);
            }
        }
    }
}
