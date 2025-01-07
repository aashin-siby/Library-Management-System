namespace LibraryManagementSystem.Services
{
     using Microsoft.Extensions.Logging;

     public interface ILoggerService
     {
          void LogInformation(string message);

          void LogError(string message);
     }


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
