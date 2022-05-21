using ITTP.Database.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;

namespace ITTP.Database.Context.Configuration
{
    internal class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.HasIndex(u => u.Guid);
            builder.HasKey(u => u.Guid);

            builder.Property(u => u.Guid);
            builder.Property(u => u.Login);
            builder.Property(u => u.Password);
            builder.Property(u => u.Name);
            builder.Property(u => u.Gender);
            builder.Property(u => u.Birthday);
            builder.Property(u => u.Admin);
            builder.Property(u => u.CreatedOn);
            builder.Property(u => u.CreatedBy);
            builder.Property(u => u.ModifiedOn);
            builder.Property(u => u.ModifiedBy);
            builder.Property(u => u.RevokedOn);
            builder.Property(u => u.RevokedBy);

            builder.HasData(
                new User()
                {
                    Guid = Guid.NewGuid(),
                    Login = "Gena",
                    Password = "Gena",
                    Name = "Gena",
                    Gender = 1,
                    Birthday = DateTime.UtcNow.AddYears(-22),
                    Admin = true,
                    CreatedOn = DateTime.UtcNow,
                    CreatedBy = null,
                    ModifiedOn = null,
                    ModifiedBy = null,
                    RevokedOn = null,
                    RevokedBy = null,
                });
        }
    }
}
