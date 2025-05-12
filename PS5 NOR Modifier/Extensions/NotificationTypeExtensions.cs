using UART.Core.Models;

namespace PS5_NOR_Modifier.Extensions;

public static class NotificationTypeExtensions
{
    public static MessageBoxButtons ToMessageBoxButtons(this NotificationType notificationType)
    {
        switch (notificationType)
        {
            default:
                return MessageBoxButtons.OK;
            
        }
    }

    public static MessageBoxIcon ToMessageBoxIcon(this NotificationType notificationType)
    {
        switch (notificationType)
        {
            case NotificationType.Error:
                return MessageBoxIcon.Error;
            case NotificationType.Warning:
                return MessageBoxIcon.Warning;
            case NotificationType.Information:
                return MessageBoxIcon.Information;
            default:
                return MessageBoxIcon.None;
        }
    }
}