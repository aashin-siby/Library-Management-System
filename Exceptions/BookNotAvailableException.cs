namespace LibraryManagementSystem.Exceptions
{
    using System;

    public class BookNotAvailableException : Exception
    {
        public BookNotAvailableException() : base("The requested book is not available.")
        {
        }

        public BookNotAvailableException(string message) : base(message)
        {
        }

        public BookNotAvailableException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}
