
using LibraryManagementSystem.Exceptions;
//validation control, buildin validation
//using keyword, is , for 
namespace LibraryManagementSystem.Utils
{
    using System;
    public static class Validation
    {
        public static void ValidateUsername(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
            {
                throw new ValidationException("Username cannot be empty or whitespace.");
            }
        }

        public static void ValidatePassword(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
            {
                throw new ValidationException("Password cannot be empty or whitespace.");
            }
            if (password.Length < 6)
            {
                throw new ValidationException("Password must be at least 6 characters long.");
            }
        }
        public static void ValidateRole(int roleId)
        {
            if (roleId != 1 && roleId != 2)
            {
                throw new ValidationException("Role ID must be either 1 or 2.");
            }
        }
    }
}
