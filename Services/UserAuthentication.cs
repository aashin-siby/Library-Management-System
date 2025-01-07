using System;
using LibraryManagementSystem.Exceptions;
using LibraryManagementSystem.Models;
using Microsoft.Extensions.Configuration;
using LibraryManagementSystem.Utils;

namespace LibraryManagementSystem.Services
{

    // Class responsible for user authentication, including registration and login functionalities
    public class UserAuthentication
    {
        private readonly IConfiguration _configuration;
        private readonly ILoggerService _logger;
        private readonly DatabaseHelper _dbHelper;


        public UserAuthentication(IConfiguration configuration, ILoggerService logger)
        {
            _configuration = configuration;
            string? connectionString = _configuration.GetConnectionString("LibraryDb");
            _dbHelper = new DatabaseHelper(connectionString);
            _logger = logger;
        }
 
        // Method to Register a new user 
        public void RegisterUser()
        {
            try
            {
                Console.Write("Enter username: ");
                string? username = Console.ReadLine();
                Validation.ValidateUsername(username);

                Console.Write("Enter password: ");
                string password = PasswordHashing();
                Validation.ValidatePassword(password);

                int roleId = 0;
                Console.Write("Enter the Role (1.User, 2.Admin): ");
                string? input = Console.ReadLine();
                try
                {

                    if (int.TryParse(input, out roleId))
                    {

                        Validation.ValidateRole(roleId);

                    }
                    else
                    {
                        throw new ValidationException("Role ID must be a numeric value.");
                    }
                }
                catch (ValidationException ex)
                {
                    _logger.LogError(ex.Message);
                }

                if (_dbHelper.UserExists(username))
                {
                    throw new UserAlreadyExistsException("Username already exists. Please try a different username.");
                }

                var user = new User
                {
                    Username = username,
                    Password = password,
                    RoleId = roleId,
                    CreatedAt = DateTime.Now
                };

                _dbHelper.AddUser(user);
                _logger.LogInformation("Registration successful!");
            }
            catch (Exception registrationError)
            {
                _logger.LogError($"Error: {registrationError.Message}");
            }
        }

        // Method to log in a user
        public User? LoginUser()
        {
            try
            {
                Console.WriteLine("\nEnter username to login:");
                string? username = Console.ReadLine();
                Console.WriteLine("Enter password:");
                string password = PasswordHashing();

                var user = _dbHelper.GetUserByUsername(username);

                if (user != null && user.Password == password)
                {
                    _logger.LogInformation("Login successful!");
                    return user;
                }
                else
                {
                    _logger.LogError("Invalid username or password. Please try again.");
                    return null;
                }
            }
            catch (Exception loginError)
            {
                _logger.LogError($"Error during login: {loginError.Message}");
                return null;
            }
        }

        // Method to mask the password input while typing
        private string PasswordHashing()
        {
            string password = string.Empty;
            ConsoleKeyInfo keyInfo;

            while ((keyInfo = Console.ReadKey(true)).Key != ConsoleKey.Enter)
            {
                if (keyInfo.Key == ConsoleKey.Backspace)
                {
                    if (password.Length > 0)
                    {
                        password = password.Substring(0, password.Length - 1);
                        Console.Write("\b \b");
                    }
                }
                else
                {
                    password += keyInfo.KeyChar;
                    Console.Write('*');
                }
            }
            Console.WriteLine();
            return password;
        }
    }
}
