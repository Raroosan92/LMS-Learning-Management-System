using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LMS_Learning_Management_System.Models
{
    public class CustomUserValidator
    {
        public class CustomPhoneNumberValidator : IUserValidator<AppUser>
        //public class CustomPhoneNumberValidator<TUser> : IUserValidator<AppUser> where TUser : class
        {
            private readonly UserManager<AppUser> _userManager;

            public CustomPhoneNumberValidator(UserManager<AppUser> userManager)
            {
                _userManager = userManager;
            }

            public async Task<IdentityResult> ValidateAsync(UserManager<AppUser> manager, AppUser user)
            {
                var result = IdentityResult.Success;

                // Check for duplicate phone numbers
                var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
                var email = _userManager.Users.Where(c => c.PhoneNumber == user.PhoneNumber).FirstOrDefault().Email;

                AppUser appUser = await _userManager.FindByEmailAsync(email.ToString());
                if (!string.IsNullOrEmpty(phoneNumber))
                {
                    var existingUser = phoneNumber;
                    if (existingUser != null && !EqualityComparer<string>.Default.Equals(user.PhoneNumber, phoneNumber))
                    {
                        result = IdentityResult.Failed(new IdentityError { Code = "DuplicatePhoneNumber", Description = "Phone number is already taken." });
                    }
                }

                return result;
            }

        }
    }
}
