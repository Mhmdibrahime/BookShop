using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.UserVM
{
    public class UserVM
    {
        [Display(Name = "Id")]
        public string Id { get; set; }
        [Display(Name = "Name")]
        public string Name { get; set; }
        [Display(Name = "Email Address")]
        public string Email { get; set; }
        [Display(Name = "Address")]
        public string Address { get; set; }
        [Display(Name = "Phone Number")]
        [Required(ErrorMessage = "Phone number is required")]
        [RegularExpression(@"^(\d{10,15})$", ErrorMessage = "Please enter a valid phone number")]
        public string PhoneNumber { get; set; }
        [Display(Name = "Gender")]
        public string Gender { get; set; }
        [Display(Name = "Username")]
        public string UserName { get; set; }
        [Display(Name = "Roles in system")]
        [ValidateNever]
        public IEnumerable<string> Roles { get; set; }
        [Display(Name = "Photo")]
        public byte[]? Image { get; set; }
        public string? Lock { get; set; }

    }
}
