// Title : Library Management System Application
// Author: Aashin Siby
// Created at : 22/12/2024
// Updated at : 25/12/2024
// Reviewed by : Sabapathi Shanmugam
// Reviewed at : 26/12/2024
using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using LibraryManagementSystem.Services;
using Microsoft.Extensions.Configuration;
using LibraryManagementSystem.Models;

namespace LibraryManagementSystem
{

    // Main entry point for the Library Management System application
    class Program
    {
        private static ILoggerService? _logger;
        public static User? loggedInUser = null;

        // Main method to run the application
        static void Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();
            _logger = host.Services.GetRequiredService<ILoggerService>();
            var userAuthentication = host.Services.GetRequiredService<UserAuthentication>();

            bool exit = false;

            while (!exit)
            {
                Console.WriteLine("\n");
                Console.WriteLine(new string('#', 42));
                Console.WriteLine(" Welcome to the LMS, Choose an option : ");
                Console.WriteLine(new string('#', 42));

                Console.WriteLine("1. Register User");
                Console.WriteLine("2. Login User");
                Console.WriteLine("3. Exit");
                string? choice = Console.ReadLine();
                Console.WriteLine(new string('_', 38));

                switch (choice)
                {
                    case "1":
                        userAuthentication.RegisterUser();
                        break;
                    case "2":
                        loggedInUser = userAuthentication.LoginUser();
                        if (loggedInUser != null) exit = true;
                        break;
                    case "3":
                        exit = true;
                        _logger.LogInformation("Exiting the application.");
                        break;
                    default:
                        _logger.LogError("Choose a valid option.");
                        break;
                }
            }

            if (loggedInUser != null)
            {
                var bookCatalogue = host.Services.GetRequiredService<BookCatalogue>();
                bool isRunning = true;
                Console.WriteLine($"\nWelcome-----{loggedInUser.Username}----to the Library Management System\n");

                while (isRunning)
                {
                    try
                    {
                        Console.WriteLine("\nPlease choose an option:");
                        Console.WriteLine("1. View Available Books");

                        if (loggedInUser.RoleId == 1)
                        {
                            Console.WriteLine("2. Borrow a Book");
                            Console.WriteLine("3. Return a Book");
                        }
                        else if (loggedInUser.RoleId == 2)
                        {
                            Console.WriteLine("2. Add a New Book");
                            Console.WriteLine("3. Remove a Book");
                            Console.WriteLine("4. Increase Book Copies");
                        }

                        Console.WriteLine("5. Exit");
                        string? choice = Console.ReadLine();

                        switch (choice)
                        {
                            case "1":
                                bookCatalogue.ViewAvailableBooks();
                                break;

                            case "2":
                                if (loggedInUser.RoleId == 1)
                                {
                                    Console.Write("Enter the ID of the book you want to borrow: ");
                                    if (int.TryParse(Console.ReadLine(), out int borrowId))
                                    {
                                        bookCatalogue.BorrowBook(borrowId, loggedInUser);
                                    }
                                    else
                                    {
                                        _logger.LogError("Invalid ID format. Please enter a number.");
                                    }
                                }
                                else if (loggedInUser.RoleId == 2)
                                {
                                    Console.Write("Enter the title of the book you want to add: ");
                                    string? title = Console.ReadLine();

                                    Console.Write("Enter the author of the book: ");
                                    string? author = Console.ReadLine();

                                    Console.Write("Enter the number of copies: ");
                                    if (int.TryParse(Console.ReadLine(), out int copies))
                                    {
                                        var newBook = new Book
                                        {
                                            Title = title,
                                            Author = author,
                                            AvailableCopies = copies
                                        };
                                        bookCatalogue.AddBook(newBook, loggedInUser);
                                    }
                                    else
                                    {
                                        _logger.LogError("Invalid number format. Please enter a number.");
                                    }
                                }
                                break;

                            case "3":
                                if (loggedInUser.RoleId == 1)
                                {
                                    Console.Write("Enter the ID of the book you want to return: ");
                                    if (int.TryParse(Console.ReadLine(), out int returnId))
                                    {
                                        bookCatalogue.ReturnBook(returnId, loggedInUser);
                                    }
                                    else
                                    {
                                        _logger.LogError("Invalid ID format. Please enter a number.");
                                    }
                                }
                                else if (loggedInUser.RoleId == 2)
                                {
                                    Console.Write("Enter the ID of the book you want to remove: ");
                                    if (int.TryParse(Console.ReadLine(), out int removeId))
                                    {
                                        bookCatalogue.RemoveBook(removeId, loggedInUser);
                                    }
                                    else
                                    {
                                        _logger.LogError("Invalid ID format. Please enter a number.");
                                    }
                                }
                                break;

                            case "4":
                                if (loggedInUser.RoleId == 2)
                                {
                                    Console.Write("Enter the ID of the book you want to increase copies for: ");
                                    if (int.TryParse(Console.ReadLine(), out int bookId))
                                    {
                                        Console.Write("Enter the number of copies to add: ");
                                        if (int.TryParse(Console.ReadLine(), out int copies))
                                        {
                                            bookCatalogue.IncreaseBookCopies(bookId, copies, loggedInUser);
                                        }
                                        else
                                        {
                                            _logger.LogError("Invalid number format. Please enter a number.");
                                        }
                                    }
                                    else
                                    {
                                        _logger.LogError("Invalid ID format. Please enter a number.");
                                    }
                                }
                                break;

                            case "5":
                                isRunning = false;
                                _logger.LogInformation("Exiting the Library Management System.");
                                break;

                            default:
                                _logger.LogError("Invalid choice. Please try again.");
                                break;
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError($"An error occurred: {ex.Message}");
                    }
                }

                _logger.LogInformation("Thank you for using the Library Management System!");
            }
            else
            {
                _logger.LogError("User login failed. Exiting the application.");
            }
        }
        // Configure and create the host builder
        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((context, config) =>
                {
                    config.AddJsonFile("Utils/appsettings.json", optional: false, reloadOnChange: true);
                })
                .ConfigureServices((context, services) =>
                {
                    var configuration = context.Configuration;
                    string? connectionString = configuration.GetConnectionString("LibraryDb");

                    if (string.IsNullOrEmpty(connectionString))
                    {
                        throw new InvalidOperationException("Connection string 'LibraryDb' is not configured.");
                    }

                    services.AddSingleton(new DatabaseHelper(connectionString));
                    services.AddLogging();
                    services.AddSingleton<ILoggerService, LoggerService>();
                    services.AddSingleton<BookCatalogue>();
                    services.AddSingleton<UserAuthentication>();
                });
    }
}