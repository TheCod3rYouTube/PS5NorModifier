using UART.Core.Models;
namespace UART.Core.Abstractions;

public interface INotificationHandler
{
    void HandleMessage(Notification notificationDetails);
    
    void HandleQuestion(Question question);
    
    Task HandleQuestion(AsyncQuestion asyncQuestion);
}