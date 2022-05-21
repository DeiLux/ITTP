using CoreModels = ITTP.Core.Models;
using DbModels = ITTP.Database.Models;

namespace ITTP.Database.Repositories.Converters
{
    internal static class CoreToDbUserConverter
    {
        public static DbModels.User? Convert(CoreModels.User? core)
        {
            if (core == null) return null;

            return new DbModels.User()
            {
                Guid = core.Guid,
                Login = core.Login,
                Password = core.Password,
                Name = core.Name,
                Gender = core.Gender,
                Birthday = core.Birthday,
                Admin = core.IsAdmin,
                CreatedOn = core.CreatedOn,
                CreatedBy = core.CreatedBy,
                ModifiedOn = core.ModifiedOn,
                ModifiedBy = core.ModifiedBy,
                RevokedOn = core.RevokedOn,
                RevokedBy = core.RevokedBy,
            };
        }

        public static CoreModels.User? ConvertBack(DbModels.User? db)
        {
            if (db == null) return null;

            return new CoreModels.User()
            {
                Guid = db.Guid,
                Login = db.Login,
                Password = db.Password,
                Name = db.Name,
                Gender = db.Gender,
                Birthday = db.Birthday,
                IsAdmin = db.Admin,
                CreatedOn = db.CreatedOn,
                CreatedBy = db.CreatedBy,
                ModifiedOn = db.ModifiedOn,
                ModifiedBy = db.ModifiedBy,
                RevokedOn = db.RevokedOn,
                RevokedBy = db.RevokedBy,
            };
        }
    }
}
