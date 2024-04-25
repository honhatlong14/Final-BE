using System.ComponentModel.DataAnnotations;
using Common.Constants;
using Microsoft.AspNetCore.Http;

namespace Data.ViewModels.User;

public class UpdateRequest
    {
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
        public StringEnum.Roles Role { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public StringEnum.Gender Gender { get; set; }
        public IFormFile AvatarFile { get; set; }

        // public StringEnum.Roles Role
        // {
        //     get => _role;
        //     set => _role = StringEnum.Roles.User;
        // }
        //
        // // public StringEnum.Gender Gender
        // // {
        // //     get => _gender;
        // //     set => _gender = StringEnum.Roles.User;
        // // }
        //
        // [EmailAddress]
        // public string Email
        // {
        //     get => _email;
        //     set => _email = replaceEmptyWithNull(value);
        // }
        //
        //
        // [MinLength(3)]
        // public string Password
        // {
        //     get => _password;
        //     set => _password = replaceEmptyWithNull(value);
        // }
        //
        //
        // public string FullName
        // {
        //     get => _fullName;
        //     set => _fullName = replaceEmptyWithNull(value);
        // }
        //
        // [MinLength(3)]
        // public string PhoneNumber
        // {
        //     get => _phoneNumber;
        //     set => _phoneNumber = replaceEmptyWithNull(value);
        // }
        //
        //
        // [Compare("Password")]
        // public string ConfirmPassword 
        // {
        //     get => _confirmPassword;
        //     set => _confirmPassword = replaceEmptyWithNull(value);
        // }



        // helpers

        private string replaceEmptyWithNull(string value)
        {
            // replace empty string with null to make field optional
            return string.IsNullOrEmpty(value) ? null : value;
        }
        
    }