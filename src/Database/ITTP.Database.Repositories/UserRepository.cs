using ITTP.Core.Repositories;
using ITTP.Database.Context;
using ITTP.Database.Repositories.Converters;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoreModels = ITTP.Core.Models;
using DbModels = ITTP.Database.Models;

namespace ITTP.Database.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly NpgSqlContext _dbContext;

        public UserRepository(NpgSqlContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task CreateUserAsync(CoreModels.User user)
        {
            await _dbContext.AddAsync(CoreToDbUserConverter.Convert(user)!);
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteUserFullAsync(string login)
        {
            DbModels.User user = await _dbContext.Users
                .FirstAsync(u => u.Login == login);
            _dbContext.Users.Remove(user);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<CoreModels.User?> ReadUserAsync(string login, string password)
        {
            DbModels.User? user = await _dbContext.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Login == login && u.Password == password);
            return CoreToDbUserConverter.ConvertBack(user);
        }

        public async Task<CoreModels.User> ReadUserAsync(Guid guid)
        {
            DbModels.User? user = await _dbContext.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Guid == guid);
            return CoreToDbUserConverter.ConvertBack(user);
        }

        public async Task<CoreModels.User?> ReadUserLoginAsync(string login)
        {
            DbModels.User? user = await _dbContext.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Login == login);
            return CoreToDbUserConverter.ConvertBack(user);
        }

        public async Task<List<CoreModels.User?>> ReadUsersAgeAsync(int age)
        {
            return await _dbContext.Users
                .AsNoTracking()
                .Where(u => DateTime.Now.Year - u.Birthday.Value.Year > age)
                .Select(data => CoreToDbUserConverter.ConvertBack(data)).ToListAsync();
        }

        public async Task<List<CoreModels.User?>> ReadUsersUpdateAsync(string login)
        {
            return await _dbContext.Users
                .AsNoTracking()
                .Where(u => u.CreatedBy == login || u.ModifiedBy == login || u.RevokedBy == login)
                .Select(data => CoreToDbUserConverter.ConvertBack(data)).ToListAsync();
        }

        public async Task<List<CoreModels.User>> ReadUsersAsync()
        {
            return await _dbContext.Users
                .AsNoTracking()
                .Select(data => CoreToDbUserConverter.ConvertBack(data)).ToListAsync();
        }

        public async Task<List<CoreModels.User?>> ReadUsersRevokedOnNullAsync()
        {
            return await _dbContext.Users
                .AsNoTracking()
                .Where(u => u.RevokedOn == null)
                .Select(data => CoreToDbUserConverter.ConvertBack(data)).ToListAsync();
        }

        public async Task UpdateUserAsync(CoreModels.User user)
        {
            DbModels.User? updatedUser = CoreToDbUserConverter.Convert(user);
            _dbContext.Users.Update(updatedUser);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<bool> UserExistsAsync(Guid guid)
        {
            return await _dbContext.Users.AsNoTracking().AnyAsync(user => user.Guid == guid);
        }

        public async Task<bool> UserExistsAsync(string login)
        {
            return await _dbContext.Users.AsNoTracking().AnyAsync(user => user.Login == login);
        }
    }
}
