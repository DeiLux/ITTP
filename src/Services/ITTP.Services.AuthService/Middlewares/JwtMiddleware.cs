using ITTP.Core.Configurations;
using ITTP.Core.Models;
using ITTP.Core.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace ITTP.Services.AuthService.Middlewares
{
    public class JwtMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IAuthConfiguration _authConfiguration;
        private readonly ILogger<JwtMiddleware> _logger;

        public JwtMiddleware(RequestDelegate next, IAuthConfiguration authConfiguration, ILogger<JwtMiddleware> logger)
        {
            _next = next;
            _authConfiguration = authConfiguration;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context, IUserService userService)
        {
            var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

            if (token != null)
            {
                await AttachUserToContext(context, token, userService);
            }

            await _next(context);
        }

        private async Task AttachUserToContext(HttpContext context, string token, IUserService userService)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(_authConfiguration.Key!);
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero,

                }, out SecurityToken validatedToken);

                var jwtToken = (JwtSecurityToken)validatedToken;
                var userLogin = jwtToken.Claims.First(claim => claim.Type == nameof(AuthData.Login)).Value;

                context.Items[nameof(AuthData)] = await userService.ReadUserAsync(userLogin);
            }
            catch
            {
                _logger.LogError("User not authorized.");
            }
        }
    }
}
