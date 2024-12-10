// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Entities.Domains;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace WebUI.Areas.Identity.Pages.Account.Manage
{
	public class IndexModel : PageModel
	{
		private readonly UserManager<User> _userManager;
		private readonly SignInManager<User> _signInManager;

		public IndexModel(
			UserManager<User> userManager,
			SignInManager<User> signInManager)
		{
			_userManager = userManager;
			_signInManager = signInManager;
		}

		/// <summary>
		///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
		///     directly from your code. This API may change or be removed in future releases.
		/// </summary>
		public string Username { get; set; }

		/// <summary>
		///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
		///     directly from your code. This API may change or be removed in future releases.
		/// </summary>
		[TempData]
		public string StatusMessage { get; set; }

		/// <summary>
		///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
		///     directly from your code. This API may change or be removed in future releases.
		/// </summary>
		[BindProperty]
		public InputModel Input { get; set; }

		/// <summary>
		///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
		///     directly from your code. This API may change or be removed in future releases.
		/// </summary>
		public class InputModel
		{
			/// <summary>
			///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
			///     directly from your code. This API may change or be removed in future releases.
			/// </summary>
			[Phone]
			[Display(Name = "Phone number")]
			public string PhoneNumber { get; set; }
			[Display(Name = "Profile Picture")]
			public byte[] ProfilePicture { get; set; }
			[MaxLength(50)]
			[Display(Name = "First Name")]
			public string FirstName { get; set; }

			[MaxLength(50)]
			[Display(Name = "Last Name")]
			public string LastName { get; set; }

			[MaxLength(50)]
			[Display(Name = "Address")]
			public string Address { get; set; }

		}

		private async Task LoadAsync(User user)
		{
			var userName = await _userManager.GetUserNameAsync(user);
			var phoneNumber = await _userManager.GetPhoneNumberAsync(user);

			Username = userName;

			Input = new InputModel
			{
				PhoneNumber = phoneNumber,
				ProfilePicture = user.ImageUrl,
				FirstName = user.FirstName,
				LastName = user.LastName,
				Address = user.Address,
			};
		}

		public async Task<IActionResult> OnGetAsync()
		{
			var user = await _userManager.GetUserAsync(User);
			if (user == null)
			{
				return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
			}

			await LoadAsync(user);
			return Page();
		}

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            if (!ModelState.IsValid)
            {
                await LoadAsync(user);
                return Page();
            }

            // Update Phone Number
            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
            if (Input.PhoneNumber != phoneNumber)
            {
                var setPhoneResult = await _userManager.SetPhoneNumberAsync(user, Input.PhoneNumber);
                if (!setPhoneResult.Succeeded)
                {
                    StatusMessage = "Unexpected error when trying to set phone number.";
                    return RedirectToPage();
                }
            }

            // Update User Profile Fields
            var userUP = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == user.Id);
            if (userUP == null)
            {
                return NotFound("User not found.");
            }

            // Update First Name and Last Name
            if (Input.FirstName != userUP.FirstName)
            {
                userUP.FirstName = Input.FirstName;
            }

            if (Input.LastName != userUP.LastName)
            {
                userUP.LastName = Input.LastName;
            }

            // Update Full Name using the new first and last names
            userUP.FullName = $"{userUP.FirstName} {userUP.LastName}";

            // Update Address
            if (Input.Address != userUP.Address)
            {
                userUP.Address = Input.Address;
            }

            // Save updated user profile
            var updateResult = await _userManager.UpdateAsync(userUP);
            if (!updateResult.Succeeded)
            {
                StatusMessage = "Unexpected error when trying to update profile.";
                return RedirectToPage();
            }

            // Process Image Upload if provided
            if (Request.Form.Files.Count > 0)
            {
                var file = Request.Form.Files.FirstOrDefault();

                // Check file size (limit to 2MB) and file type (only allow jpg and png)
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
                var fileExtension = Path.GetExtension(file.FileName).ToLower();
                if (file.Length <= 2097152 && allowedExtensions.Contains(fileExtension))
                {
                    using (var dataStream = new MemoryStream())
                    {
                        await file.CopyToAsync(dataStream);
                        userUP.ImageUrl = dataStream.ToArray();
                    }

                    await _userManager.UpdateAsync(userUP);
                }
                else
                {
                    StatusMessage = "File must be a .jpg or .png image and less than 2MB.";
                    return RedirectToPage();
                }
            }

            await _signInManager.RefreshSignInAsync(user);
            StatusMessage = "Your profile has been updated";
            return RedirectToPage();
        }

    }
}
