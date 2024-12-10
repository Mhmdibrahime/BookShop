using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Text;
using Microsoft.EntityFrameworkCore;
using System.Data;
using Microsoft.VisualBasic;
using System.Net;
using WebUI.Commponents;
using Entities.Domains;
using Services.UserVM;

namespace WebUI.Areas.Admin.Controllers
{
    [Area("admin")]
    [Authorize(Roles =RoleConst.AdminRole)] 
    public class UserController : Controller
    {
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;
        private readonly IUserStore<User> _userStore;
        private readonly IUserEmailStore<User> _emailStore;
        private readonly ILogger<UserController> _logger;
        private readonly IEmailSender _emailSender;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UserController(
            UserManager<User> userManager,
            IUserStore<User> userStore,
            SignInManager<User> signInManager,
            ILogger<UserController> logger,
            IEmailSender emailSender,
            RoleManager<IdentityRole> roleManager
           )
        {
            _userManager = userManager;
            _userStore = userStore;
            _emailStore = GetEmailStore();
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
            _roleManager = roleManager;
        }


 
        public async Task<IActionResult> Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity!;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            string userId = claim!.Value;
            var users = await _userManager.Users.Where(u => u.Id != userId && u.IsDeleted != true).Select( user => new UserVM
            {
                Id = user.Id,
                Name = user.FullName,
                Address = user.Address!,
                Email = user.Email!,
                PhoneNumber = user.PhoneNumber!,
                Gender = user.Gender!,
                UserName = user.UserName!,
                Roles = _userManager.GetRolesAsync(user).GetAwaiter().GetResult(),
                Lock = _userManager.Users.FirstOrDefault(u => u.Id == user.Id)!.LockoutEnd == null
                | _userManager.Users.FirstOrDefault(u => u.Id == user.Id)!.LockoutEnd < DateTime.Now ? "UNLOCK" : "LOCK",
            }).ToListAsync();
            
            return Json(new { data = users });
        }
        [HttpGet]
        public async Task<IActionResult> Create(string? id)
        {

             return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AddUserVM userModel)
        {
            
            if (ModelState.IsValid)
            {

                var user = new User();
                user.FullName = userModel.FirstName + " " + userModel.LastName;
                user.FirstName = userModel.FirstName;
                user.LastName = userModel.LastName;
                user.Gender = userModel.Gender;
                user.BirthDate = userModel.BirthDate;
                user.PhoneNumber = userModel.PhoneNumber;
                user.EmailConfirmed = true;
                

                await _userStore.SetUserNameAsync(user, userModel.Email, CancellationToken.None);
                await _emailStore.SetEmailAsync(user, userModel.Email, CancellationToken.None);

                var result = await _userManager.CreateAsync(user, userModel.Password);

                if (result.Succeeded)
                {
                    if (userModel.Roles is not null)
                    {
                        foreach (var role in userModel.Roles)
                        {
                            await _userManager.AddToRoleAsync(user, role);
                        }
                    }
                    else
                    {
                        await _userManager.AddToRoleAsync(user, RoleConst.UserRole);
                    }
                    TempData["SuccessRegister"] = "Your account has been created successfully";

                    return RedirectToAction(nameof(Index));
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }

            }
            return View();
        }
        
        public async Task<IActionResult> Details(string? id)
        {
            var user = await _userManager.Users.Select(user => new UserVM
            {
                Id = user.Id,
                Name = user.FullName,
                Address = user.Address!,
                Email = user.Email!,
                PhoneNumber = user.PhoneNumber!,
                Gender = user.Gender!,
                UserName = user.UserName!,
                Roles = _userManager.GetRolesAsync(user).GetAwaiter().GetResult(),
                Image = user.ImageUrl
            }).FirstOrDefaultAsync(x => x.Id == id);
            
            return View(user);
        }

        public async Task<IActionResult> Edit(string? id)
        {
            var user = await _userManager.Users.Select(user => new UserVM
            {
                Id = user.Id,
                Name = user.FullName,
                Address = user.Address!,
                Email = user.Email!,
                PhoneNumber = user.PhoneNumber!,
                Gender = user.Gender!,
                UserName = user.UserName!,
                Roles = _userManager.GetRolesAsync(user).GetAwaiter().GetResult(),
                Image = user.ImageUrl
            }).FirstOrDefaultAsync(x => x.Id == id);
            return View(user);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string? id, UserVM userVM )
        {
            var user = _userManager.Users.FirstOrDefault(x => x.Id == id);
            user!.PhoneNumber = userVM.PhoneNumber;
            user.Gender = userVM.Gender;
            user.UserName = userVM.UserName;
            user.Email = userVM.Email;
            user.FullName = userVM.Name;
            user.Address = userVM.Address;
            var currentRoles = await _userManager.GetRolesAsync(user);

            foreach (var role in userVM.Roles)
            {
                if (!currentRoles.Contains(role))
                {
                    await _userManager.AddToRoleAsync(user, role);
                }
            }
            foreach (var role in currentRoles)
            {
                if (!userVM.Roles.Contains(role))
                {
                    await _userManager.RemoveFromRoleAsync(user, role);
                }
            }
            await _userManager.UpdateAsync(user);
            if (Request.Form.Files.Count > 0)
            {
                var file = Request.Form.Files.FirstOrDefault();


                using (var dataStream = new MemoryStream())
                {
                    await file!.CopyToAsync(dataStream);
                    user.ImageUrl = dataStream.ToArray();
                }
                await _userManager.UpdateAsync(user);
            }

            return RedirectToAction("Edit","User",id);
        }

        public async Task<IActionResult> Delete(string? id)
        {


            var user = _userManager.Users.FirstOrDefault(user => user.Id == id);

            if (user == null)
            {
                return Json(new { success = false, message = "User not found" });
            }

            user.IsDeleted = true;
            await  _userManager.UpdateAsync(user);

            return Json(new { success = true, message = "User has been deleted" });
        }

        public async Task<IActionResult> LockUser(string id)
        {
            var user = await _userManager.Users.FirstOrDefaultAsync(user => user.Id == id);
            if (user == null)
            {
                return NotFound();
            }

            if (user.LockoutEnd == null || user.LockoutEnd < DateTime.Now)
            {
                user.LockoutEnd = DateTime.Now.AddYears(1);
                await _userManager.SetLockoutEndDateAsync(user, user.LockoutEnd);
                return Json(new { success = true, message = "User has been locked.", isLocked = true });
            }
            else
            {
                user.LockoutEnd = DateTime.Now;
                await _userManager.SetLockoutEndDateAsync(user, user.LockoutEnd);
                return Json(new { success = true, message = "User has been unlocked.", isLocked = false });
            }
        }
        private IUserEmailStore<User> GetEmailStore()
        {
            if (!_userManager.SupportsUserEmail)
            {
                throw new NotSupportedException("The default UI requires a user store with email support.");
            }
            return (IUserEmailStore<User>)_userStore;
        }
    }
}
