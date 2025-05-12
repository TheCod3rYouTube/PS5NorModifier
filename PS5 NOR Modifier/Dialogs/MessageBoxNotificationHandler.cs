using PS5_NOR_Modifier.Extensions;
using UART.Core.Abstractions;
using UART.Core.Models;

namespace PS5_NOR_Modifier.Dialogs;

public class MessageBoxNotificationHandler : INotificationHandler
{
    public void HandleMessage(Notification notificationDetails)
    {
        MessageBox.Show(
            notificationDetails.Message, 
            notificationDetails.Title, 
            notificationDetails.Type.ToMessageBoxButtons(), 
            notificationDetails.Type.ToMessageBoxIcon());
    }

    public void HandleQuestion(Question question)
    {
        DialogResult result = MessageBox.Show(
            question.Message,
            question.Title,
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Question);
        
        if (result == DialogResult.Yes && question.OnYes != null)
            question.OnYes.Invoke();
        
        if (result == DialogResult.No && question.OnNo != null)
            question.OnNo.Invoke();
    }

    public async Task HandleQuestion(AsyncQuestion asyncQuestion)
    {
        DialogResult result = MessageBox.Show(
            asyncQuestion.Message,
            asyncQuestion.Title,
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Question);
        
        if (result == DialogResult.Yes && asyncQuestion.OnYes != null)
            await asyncQuestion.OnYes.Invoke();
        
        if (result == DialogResult.No && asyncQuestion.OnNo != null)
            await asyncQuestion.OnNo.Invoke();
    }
}