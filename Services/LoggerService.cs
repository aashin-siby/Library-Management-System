namespace LibraryManagementSystem.Services
{
     using Microsoft.Extensions.Logging;

     // Interface defining the logging operations for the library management system
     public interface ILoggerService
     {
          // Logs an informational message
          void LogInformation(string message);

          // Logs an error message
          void LogError(string message);
     }

     // Implementation of the ILoggerService interface 
     public class LoggerService : ILoggerService
     {
          private readonly ILogger<LoggerService> _logger;

          public LoggerService(ILogger<LoggerService> logger)
          {
               _logger = logger;
          }

          public void LogInformation(string message)
          {
               _logger.LogInformation(message);
          }

          public void LogError(string message)
          {
               _logger.LogError(message);
          }
     }
}
