using ITTP.Core.Exceptions;
using ITTP.Core.Models;
using ITTP.Core.Services;
using ITTP.Datatransfer.HttpDto;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace ITTP.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IAuthService authService, ILogger<AuthController> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        /// <summary>
        /// Авторизация пользователя 
        /// </summary>
        /// <param name="authRequest"></param>
        /// <returns>Токин</returns>
        /// <response code="200">Токин</response>
        /// <response code="401">Пользователь не авторизован</response>
        /// <response code="500">Ошибка на стороне сервера</response>
        [HttpPost("login")]
        [SwaggerOperation("Авторизация пользователя")]
        [SwaggerResponse(statusCode: 200, type: typeof(AuthData), description: "Токин")]
        [SwaggerResponse(statusCode: 401, type: typeof(EmptyResult), description: "Пользователь не авторизован")]
        [SwaggerResponse(statusCode: 500, type: typeof(EmptyResult), description: "Ошибка на стороне сервера")]
        public async Task<IActionResult> Login([FromQuery] AuthRequest authRequest)
        {
            try
            {
                var token = await _authService.CreateTokenAsync(authRequest.Login!, authRequest.Password!);

                return Ok(token);
            }
            catch (AuthException ex)
            {
                _logger.LogError(ex, "Error in controller {controllerName} in method {methodName}.",
                     nameof(AuthController), nameof(Login));
                return StatusCode(401, "Пользователь не авторизован");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in controller {controllerName} in method {methodName}.",
                     nameof(AuthController), nameof(Login));
                return StatusCode(500);
            }
        }
    }
}
