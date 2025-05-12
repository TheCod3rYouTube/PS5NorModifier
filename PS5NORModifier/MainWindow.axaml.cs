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
    
    public MainWindow()
    {
        InitializeComponent();
        RefreshComPorts();
        BoardVariantIn.ItemsSource = _boardVariants;
        PS5ModelIn.ItemsSource = _models;
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

    private void ShowError(string error)
    {
        DialogHost.Show(new DialogContents(Dialog, error, "An error occurred.", "OK"), Dialog);
    }
}