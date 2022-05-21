using ITTP.Core.Configurations;
using ITTP.Core.Exceptions;
using ITTP.Core.Models;
using ITTP.Core.Repositories;
using ITTP.Core.Services;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace ITTP.Services.AuthService
{
    public class AuthService : IAuthService
    {
        private readonly IAuthConfiguration _authConfiguration;
        private readonly IUserRepository _userRepositories;
        private readonly ILogger<AuthService> _logger;

        public AuthService(IAuthConfiguration authConfiguration,
            IUserRepository userRepositories,
            ILogger<AuthService> logger)
        {
            _authConfiguration = authConfiguration;
            _userRepositories = userRepositories;
            _logger = logger;
        }

        public async Task<AuthData?> CreateTokenAsync(string login, string password)
        {
            try
            {
                var storedUser = await _userRepositories.ReadUserAsync(login, password);

                if (storedUser == null)
                {
                    throw new AuthException($"User with login {login} is not found");
                }

                string token = CreateJwt(storedUser);

                return new AuthData(storedUser.Login, storedUser.IsAdmin, token);
            }
            catch (AuthException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Service: {ServiceName} in Method: {MethodName},",
                    nameof(AuthService),
                    nameof(CreateTokenAsync));
            }

            return null;
        }

        private string CreateJwt(User user)
        {
            var claims = new List<Claim>
                {
                    new Claim(nameof(AuthData.Login), user.Login),
                    new Claim(nameof(AuthData.IsAdmin), user.IsAdmin.ToString())
                };

            ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, "Token", nameof(AuthData.Login), nameof(AuthData.IsAdmin));

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_authConfiguration.Key);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = claimsIdentity,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature),
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }
    }
}
