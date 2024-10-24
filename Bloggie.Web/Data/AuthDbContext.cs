using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Bloggie.Web.Data;

public class AuthDbContext : IdentityDbContext
{
    public AuthDbContext(DbContextOptions options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Seed Roles (User, Admin, SuperAdmin)
        var adminRoleId = "2fd7ce15-d70b-468f-bbca-3a75d0b8dd26";
        var superAdminRoleId = "6b2374b5-48a8-462b-a3de-6426737985c0";
        var userRoleId = "65a5e53d-7032-4594-95b4-219434de9be3";

        var roles = new List<IdentityRole>
        {

            new IdentityRole
            {
                Name = "Admin",
                NormalizedName = "Admin",
                Id = adminRoleId,
                ConcurrencyStamp = adminRoleId
            },
             new IdentityRole
            {
                Name = "SuperAdmin",
                NormalizedName = "SuperAdmin",
                Id = superAdminRoleId,
                ConcurrencyStamp = superAdminRoleId
            },
                  new IdentityRole
            {
                Name = "User",
                NormalizedName = "User",
                Id = userRoleId,
                ConcurrencyStamp = userRoleId
            },
        };


        builder.Entity<IdentityRole>().HasData(roles);

        // Seed SuperAdminUser
        var superAdminId = "82dd5e49-1f0c-41ce-947a-3ca73f725eac";
        var superAdminUser = new IdentityUser
        {
            UserName = "superadmin@bloggie.com",
            Email = "superadmin@bloggie.com",
            NormalizedEmail = "superadmin@bloggie.com".ToUpper(),
            NormalizedUserName = "superadmin@bloggie.com".ToUpper(),
            Id = superAdminId
        };

        superAdminUser.PasswordHash = new PasswordHasher<IdentityUser>()
            .HashPassword(superAdminUser, "Superadmin@123");

        builder.Entity<IdentityUser>().HasData(superAdminUser);

        // Add All roles to SuperAdminUser

        var superAdminRoles = new List<IdentityUserRole<string>>
        {
            new IdentityUserRole<string>
            {
                RoleId = adminRoleId,
                UserId = superAdminId
            },
               new IdentityUserRole<string>
            {
                RoleId = superAdminRoleId,
                UserId = superAdminId
            },
                  new IdentityUserRole<string>
            {
                RoleId = userRoleId,
                UserId = superAdminId
            },
        };
        builder.Entity<IdentityUserRole<string>>().HasData(superAdminRoles);
    }

}
