namespace LibraryManagementSystem.Services
{
    using System;
    using System.Collections.Generic;
    using System.Data.SqlClient;
    using LibraryManagementSystem.Models;

    // Class responsible for handling database operations related to users 
    // and books in the library management system.
    public class DatabaseHelper
    {
        private readonly string _connectionString;

        public DatabaseHelper(string connectionString)
        {
            _connectionString = connectionString;
        }

        // Add the user to db: Inserts a new user into the database
        public void AddUser(User user)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand("AddUser", connection))
                    {
                        command.CommandType = System.Data.CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@Username", user.Username);
                        command.Parameters.AddWithValue("@Password", user.Password);
                        command.Parameters.AddWithValue("@RoleId", user.RoleId);
                        command.Parameters.AddWithValue("@CreatedAt", DateTime.Now);
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception addingError)
            {
                throw new Exception($"Error adding user: {addingError.Message}");
            }
        }

        // Get the username from db for authentication: Retrieves a user by their username for authentication
        public User? GetUserByUsername(string username)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand("GetUserByUsername", connection))
                    {
                        command.CommandType = System.Data.CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@Username", username);

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return new User
                                {
                                    Username = reader["Username"].ToString(),
                                    Password = reader["Password"].ToString(),
                                    RoleId = (int)reader["RoleId"],
                                    CreatedAt = (DateTime)reader["CreatedAt"]
                                };
                            }
                        }
                    }
                }
                return null;
            }
            catch (Exception retrievingError)
            {
                throw new Exception($"Error retrieving user: {retrievingError.Message}");
            }
        }
        // Check for if user already exists: Checks if a user with the given username already exists in the database
        public bool UserExists(string username)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand("UserExists", connection))
                    {
                        command.CommandType = System.Data.CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@Username", username);
                        int count = (int)command.ExecuteScalar();
                        return count > 0;
                    }
                }
            }
            catch (Exception existenceError)
            {
                throw new Exception($"Error checking user existence: {existenceError.Message}");
            }
        }

        // Get all books: Retrieves a list of all books from the database
        public IEnumerable<Book> GetAllBooks()
        {
            var books = new List<Book>();

            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand("GetAllBooks", connection))
                    {
                        command.CommandType = System.Data.CommandType.StoredProcedure;
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                books.Add(new Book
                                {
                                    BookId = (int)reader["BookId"],
                                    Title = reader["Title"].ToString(),
                                    Author = reader["Author"].ToString(),
                                    AvailableCopies = (int)reader["AvailableCopies"]
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception retrivingBookError)
            {
                throw new Exception($"Error retrieving books: {retrivingBookError.Message}");
            }

            return books;
        }

        // Borrow a book: Records the borrowing of a book by a user in the database
        public void BorrowBook(int bookId, string userName)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand("BorrowBook", connection))
                    {
                        command.CommandType = System.Data.CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@BookId", bookId);
                        command.Parameters.AddWithValue("@UserName", userName);
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception borrowingError)
            {
                throw new Exception($"Error borrowing book: {borrowingError.Message}");
            }
        }

        // Return a book: Records the return of a book by a user in the database
        public void ReturnBook(int bookId, string userName)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand("ReturnBook", connection))
                    {
                        command.CommandType = System.Data.CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@BookId", bookId);
                        command.Parameters.AddWithValue("@UserName", userName);
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception returningError)
            {
                throw new Exception($"Error returning book: {returningError.Message}");
            }
        }

        // Get the book details from ID: Retrieves a book's details by its ID
        public Book GetBookById(int bookId)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand("GetBookById", connection))
                    {
                        command.CommandType = System.Data.CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@BookId", bookId);
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return new Book
                                {
                                    BookId = (int)reader["BookId"],
                                    Title = reader["Title"].ToString(),
                                    Author = reader["Author"].ToString(),
                                    AvailableCopies = (int)reader["AvailableCopies"]
                                };
                            }
                        }
                    }
                }
            }
            catch (Exception retrivingBookError)
            {
                throw new Exception($"Error retrieving book by ID: {retrivingBookError.Message}");
            }

            return null;
        }

        // Update book copies: Updates the number of available copies of a book in the database
        public void UpdateBookCopies(int bookId, int quantity)
        {
            try
            {
                var book = GetBookById(bookId);
                if (book == null)
                {
                    throw new Exception("Book not found.");
                }

                int newAvailableCopies = book.AvailableCopies + quantity;
                if (newAvailableCopies < 0)
                {
                    throw new Exception("Invalid operation: Available copies cannot be less than zero.");
                }

                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand("UpdateBookCopies", connection))
                    {
                        command.CommandType = System.Data.CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@BookId", bookId);
                        command.Parameters.AddWithValue("@Quantity", quantity);
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception updatingError)
            {
                throw new Exception($"Error updating book copies: {updatingError.Message}");
            }
        }

        // Add a new book: Inserts a new book into the database
        public void AddBook(Book book)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand("AddBook", connection))
                    {
                        command.CommandType = System.Data.CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@Title", book.Title);
                        command.Parameters.AddWithValue("@Author", book.Author);
                        command.Parameters.AddWithValue("@AvailableCopies", book.AvailableCopies);
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception addingBookError)
            {
                throw new Exception($"Error adding book: {addingBookError.Message}");
            }
        }

        // Remove a book: Deletes a book from the database by its ID
        public void RemoveBook(int bookId)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand("RemoveBook", connection))
                    {
                        command.CommandType = System.Data.CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@BookId", bookId);
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception removingBookError)
            {
                throw new Exception($"Error removing book: {removingBookError.Message}");
            }
        }

    }
}

