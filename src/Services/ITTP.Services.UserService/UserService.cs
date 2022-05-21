using ITTP.Core.Models;
using ITTP.Core.Repositories;
using ITTP.Core.Services;
using Microsoft.Extensions.Logging;

namespace ITTP.Services.UserService
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepositories;
        private readonly ILogger<UserService> _logger;
        public UserService(IUserRepository userRepositories,
            ILogger<UserService> logger)
        {
            _userRepositories = userRepositories;
            _logger = logger;
        }

        public async Task<bool> CreateUserAsync(User user)
        {
            try
            {
                if (!await _userRepositories.UserExistsAsync(user.Login))
                {
                    await _userRepositories.CreateUserAsync(user);
                    return true;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Service: {ServiceName} in Method: {MethodName},",
                    nameof(UserService),
                    nameof(CreateUserAsync));
            }

            return false;
        }

        public async Task<bool> DeleteUserFullAsync(string login)
        {
            try
            {
                if (await _userRepositories.UserExistsAsync(login))
                {
                    await _userRepositories.DeleteUserFullAsync(login);
                    return true;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Service: {ServiceName} in Method: {MethodName},",
                    nameof(UserService),
                    nameof(DeleteUserFullAsync));
            }

            return false;
        }

        public async Task<User?> ReadUserAsync(string login, string password)
        {
            try
            {
                return await _userRepositories.ReadUserAsync(login, password);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Service: {ServiceName} in Method: {MethodName},",
                    nameof(UserService),
                    nameof(ReadUserAsync));
            }

            return null;
        }

        public async Task<User?> ReadUserAsync(Guid guid)
        {
            try
            {
                return await _userRepositories.ReadUserAsync(guid);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Service: {ServiceName} in Method: {MethodName},",
                    nameof(UserService),
                    nameof(ReadUserAsync));
            }

            return null;
        }

        public async Task<User?> ReadUserLoginAsync(string login)
        {
            try
            {
                return await _userRepositories.ReadUserLoginAsync(login);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Service: {ServiceName} in Method: {MethodName},",
                    nameof(UserService),
                    nameof(ReadUserLoginAsync));
            }

            return null;
        }

        public async Task<List<User?>> ReadUsersAgeAsync(int age)
        {
            try
            {
                return await _userRepositories.ReadUsersAgeAsync(age);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Service: {ServiceName} in Method: {MethodName},",
                    nameof(UserService),
                    nameof(ReadUsersAgeAsync));
            }

            return new List<User?>();
        }

        public async Task<List<User?>> ReadUsersAsync()
        {
            try
            {
                return await _userRepositories.ReadUsersAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Service: {ServiceName} in Method: {MethodName},",
                    nameof(UserService),
                    nameof(ReadUsersAsync));
            }

            return new List<User?>();
        }

        public async Task<List<User?>> ReadUsersRevokedOnNullAsync()
        {
            try
            {
                return await _userRepositories.ReadUsersRevokedOnNullAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Service: {ServiceName} in Method: {MethodName},",
                    nameof(UserService),
                    nameof(ReadUsersAsync));
            }

            return new List<User?>();
        }

        public async Task<bool> UpdateUserAsync(User user)
        {
            try
            {
                if (await _userRepositories.UserExistsAsync(user.Guid))
                {
                    await _userRepositories.UpdateUserAsync(user);
                    return true;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Service: {ServiceName} in Method: {MethodName},",
                    nameof(UserService),
                    nameof(UpdateUserAsync));
            }

            return false;
        }

        public async Task<bool> UpdateAdminLoginAsync(User user, string oldLogin)
        {
            try
            {
                if (await _userRepositories.UserExistsAsync(user.Guid))
                {
                    await _userRepositories.UpdateUserAsync(user);
                    var users = await _userRepositories.ReadUsersUpdateAsync(oldLogin);

                    if (users != null)
                    {
                        foreach (var userItem in users)
                        {
                            if (userItem!.CreatedBy == oldLogin) userItem.CreatedBy = user.Login;
                            if (userItem!.ModifiedBy == oldLogin) userItem.ModifiedBy = user.Login;
                            if (userItem!.RevokedBy == oldLogin) userItem.RevokedBy = user.Login;

                            await _userRepositories.UpdateUserAsync(userItem);
                        }
                    }
                    return true;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Service: {ServiceName} in Method: {MethodName},",
                    nameof(UserService),
                    nameof(UpdateAdminLoginAsync));
            }

            return false;
        }
    }
}
