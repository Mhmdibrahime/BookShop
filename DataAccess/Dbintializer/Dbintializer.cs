using DataAccess.Data;
using Entities.Domains;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Dbintializer
{
    public class Dbintializer : IDbintializer
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _context;


        public Dbintializer(
            UserManager<User> userManager,
            RoleManager<IdentityRole> roleManager,
            ApplicationDbContext context)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
        }

        public async void Initialize()
        {
            try
            {
                if (_context.Database.GetPendingMigrations().Count() > 0)
                {
                    _context.Database.Migrate();
                }

            }
            catch (Exception)
            {

                throw;
            }

            if (!_roleManager.RoleExistsAsync("Admin").GetAwaiter().GetResult())
            {
                _roleManager.CreateAsync(new IdentityRole("Admin")).GetAwaiter().GetResult();
                _roleManager.CreateAsync(new IdentityRole("User")).GetAwaiter().GetResult();
                _roleManager.CreateAsync(new IdentityRole("Member")).GetAwaiter().GetResult();


                _userManager.CreateAsync(new User
                {
                    UserName = "Admin@Library.com",
                    FullName = "Administrator",
                    FirstName = "Elsayed",
                    LastName = "Farag",
                    Email = "Admin@Library.com",
                    BirthDate = DateOnly.FromDateTime(new DateTime(2003, 4, 5)),
                    Gender = "Male",
                    PhoneNumber = "01026498028",
                    Address = "Elhamoul,Kafrelshekh",
                    EmailConfirmed = true,
                }, "P@$$w0rd").GetAwaiter().GetResult();

                User user = _context.Users.FirstOrDefault(x=>x.Email == "Admin@Library.com")!;
                _userManager.AddToRoleAsync(user, "Admin").GetAwaiter().GetResult();
                _userManager.GenerateEmailConfirmationTokenAsync(user).GetAwaiter().GetResult();    
            }

            return;
        }
    }
}
