using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Platform.Storage;
using DialogHostAvalonia;
using Label = Avalonia.Controls.Label;

namespace PS5NORModifier;

public partial class MainWindow : Window
{
    // I don't like this
    public static MainWindow Instance;
    public MainWindow()
    {
        Instance = this;
        InitializeComponent();
        UartCommander.RefreshComPorts();
    }

    private void SponsorLink_OnClick(object? sender, RoutedEventArgs e)
    {
        Process.Start(new ProcessStartInfo("https://www.consolefix.shop")
        {
            UseShellExecute = true
        });
    }

    private void DonateLink_OnClick(object? sender, RoutedEventArgs e)
    {
        Process.Start(new ProcessStartInfo("https://streamelements.com/thecod3r/tip")
        {
            UseShellExecute = true
        });
    }

    private void PayPalLink_OnClick(object? sender, RoutedEventArgs e)
    {
        Process.Start(new ProcessStartInfo("https://streamelements.com/thecod3r/tip")
        {
            UseShellExecute = true
        });
    }

    internal void ShowError(string error)
    {
        DialogHost.Show(new DialogContents(Dialog, error, "An error occurred.", "OK"), Dialog);
    }

    internal void SetStatus(string status)
    {
        StatusLabel.Content = status;
    }
}