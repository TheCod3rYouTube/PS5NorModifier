using Microsoft.Extensions.DependencyInjection;
using PS5_NOR_Modifier.Dialogs;
using PS5_NOR_Modifier.Extensions;
using UART.Core.Abstractions;
using UART.Core.Data;

namespace PS5_NOR_Modifier
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            IServiceProvider provider = new ServiceCollection()
                .AddInMemoryConfiguration()
                .AddSingleton<IUartProvider, XmlUartProvider>()
                .AddSingleton<INotificationHandler, MessageBoxNotificationHandler>()
                .BuildServiceProvider();
            
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();
            Application.Run(
                new Form1(
                    provider.GetRequiredService<IUartProvider>(),
                    provider.GetRequiredService<INotificationHandler>()
                )
            );
        }
    }
}