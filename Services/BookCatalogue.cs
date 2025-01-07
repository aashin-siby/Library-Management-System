namespace LibraryManagementSystem.Services
{
    using LibraryManagementSystem.Models;
    using System;
   
    // Class responsible for managing book-related operations in the library system
    public class BookCatalogue
    {
        private readonly ILoggerService _logger;
        private readonly DatabaseHelper _databaseHelper;

        public BookCatalogue(ILoggerService logger, DatabaseHelper databaseHelper)
        {
            _logger = logger;
            _databaseHelper = databaseHelper;
        }

        // View Available Books: Displays a list of all books in the library
        public void ViewAvailableBooks()
        {
            Console.WriteLine("\nAvailable Books:");
            var books = _databaseHelper.GetAllBooks();

            
            Console.WriteLine("{0,-10} {1,-36} {2,-25} {3,-10}", "Book ID", "Title", "Author", "Available Copies");
            Console.WriteLine(new string('-', 90));
            
            foreach (var book in books)
            {
                Console.WriteLine("{0,-10} {1,-36} {2,-25} {3,-10}", book.BookId, book.Title, book.Author, book.AvailableCopies);
            }
            Console.WriteLine();
        }

        // Borrow Book: Allows a user to borrow a book if they have the appropriate role and if the book is available
        public void BorrowBook(int bookId, User user)
        {
            if (user.RoleId != 1)
            {
                _logger.LogError("Only users can borrow books.");
                return;
            }

            try
            {
                var book = _databaseHelper.GetBookById(bookId);
                if (book == null)
                {
                    _logger.LogError("Book not found.");
                    return;
                }

                if (book.AvailableCopies > 0)
                {
                    _databaseHelper.BorrowBook(bookId, user.Username);
                    _logger.LogInformation($"{user.Username} successfully borrowed '{book.Title}'.");
                    _databaseHelper.UpdateBookCopies(bookId, -1);

                }
                else
                {
                    _logger.LogError("Sorry, the book is not available for borrowing.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error borrowing book: {ex.Message}");
            }
        }

        // Return Book: Allows a user to return a borrowed book
        public void ReturnBook(int bookId, User user)
        {
            if (user.RoleId != 1)
            {
                _logger.LogError("Only users can return books.");
                return;
            }

            try
            {
                var book = _databaseHelper.GetBookById(bookId);
                if (book == null)
                {
                    _logger.LogError("Book not found.");
                    return;
                }

                _databaseHelper.ReturnBook(bookId, user.Username);
                _logger.LogInformation($"{user.Username} successfully returned '{book.Title}'.");
                _databaseHelper.UpdateBookCopies(bookId, 1);

            }
            catch (Exception ex)
            {
                _logger.LogError($"Error returning book: {ex.Message}");

            }
        }

        // Add New Book: Allows an admin to add a new book to the catalogue
        public void AddBook(Book book, User user)
        {
            if (user.RoleId != 2)
            {
                _logger.LogError("Only admins can add new books.");
                return;
            }

            try
            {
                _databaseHelper.AddBook(book);
                _logger.LogInformation($"Book '{book.Title}' added successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error adding book: {ex.Message}");

            }
        }

        // Remove Book: Allows an admin to remove a book from the catalogue
        public void RemoveBook(int bookId, User user)
        {
            if (user.RoleId != 2)
            {
                _logger.LogError("Only admins can remove books.");
                return;
            }

            try
            {
                _databaseHelper.RemoveBook(bookId);
                _logger.LogInformation("Book removed successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error removing book: {ex.Message}");

            }
        }

        // Increase Book Copies: Allows an admin to add number of copies of the book

        public void IncreaseBookCopies(int bookId, int quantity, User user)
        {
            if (user.RoleId != 2)
            {
                _logger.LogError("Only admins can increase book copies.");
                return;
            }

            try
            {
                _databaseHelper.UpdateBookCopies(bookId, quantity);
                _logger.LogInformation("Book copies updated successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error updating book copies: {ex.Message}");

            }
        }


    }
}