using IdentityModel;
using Mango.Services.Identity.DbContexts;
using Mango.Services.Identity.Models;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace Mango.Services.Identity.Initializer
{
    public class DbInitializer : IDbInitializer
    {
        private readonly ApplicationDbContext db;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;

        public DbInitializer(ApplicationDbContext db, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            this.db = db;
            this.userManager = userManager;
            this.roleManager = roleManager;
        }

        public void Initialize()
        {
            if (this.roleManager.FindByNameAsync(SD.Admin).Result == null)
            {
                this.roleManager.CreateAsync(new IdentityRole(SD.Admin)).GetAwaiter().GetResult();
                this.roleManager.CreateAsync(new IdentityRole(SD.Customer)).GetAwaiter().GetResult();
            }
            else { return; }

            ApplicationUser adminUser = new ApplicationUser
            {
                UserName = "admin1@gmail.com",
                Email = "admin1@gmail.com",
                EmailConfirmed = true,
                PhoneNumber = "1111111111",
                FirstName = "Mik",
                LastName = "Tsyvis"
            };

            this.userManager.CreateAsync(adminUser, "Qwerty_0").GetAwaiter().GetResult();
            this.userManager.AddToRoleAsync(adminUser, SD.Admin).GetAwaiter().GetResult();

            var temp1 = this.userManager.AddClaimsAsync(adminUser, new Claim[]
            {
                new Claim(JwtClaimTypes.Name, adminUser.FirstName + " " + adminUser.LastName),
                new Claim(JwtClaimTypes.GivenName, adminUser.FirstName),
                new Claim(JwtClaimTypes.FamilyName, adminUser.LastName),
                new Claim(JwtClaimTypes.Role, SD.Admin),
                new Claim(JwtClaimTypes.Email, adminUser.Email)
            }).Result;

            ApplicationUser customerUser = new ApplicationUser
            {
                UserName = "customer1@gmail.com",
                Email = "customer1@gmail.com",
                EmailConfirmed = true,
                PhoneNumber = "1111111111",
                FirstName = "Mik",
                LastName = "Customer"
            };

            this.userManager.CreateAsync(customerUser, "Qwerty_0").GetAwaiter().GetResult();
            this.userManager.AddToRoleAsync(customerUser, SD.Customer).GetAwaiter().GetResult();

            var temp2 = this.userManager.AddClaimsAsync(adminUser, new Claim[]
            {
                new Claim(JwtClaimTypes.Name, customerUser.FirstName + " " + customerUser.LastName),
                new Claim(JwtClaimTypes.GivenName, customerUser.FirstName),
                new Claim(JwtClaimTypes.FamilyName, customerUser.LastName),
                new Claim(JwtClaimTypes.Role, SD.Customer),
                new Claim(JwtClaimTypes.Email, customerUser.Email)
            }).Result;
        }
    }
}
