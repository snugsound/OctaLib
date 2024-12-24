using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Interactivity;
using OctaLib;
using OctaLibAvalonia.ViewModels;
using System;

namespace OctaLibAvalonia.Views;

public partial class BankSwapWindow : Window
{
    public BankSwapWindow()
    {
        InitializeComponent();
    }

    private void OnBankSwap(object sender, RoutedEventArgs e)
    {
        if (string.IsNullOrEmpty(Bank1.Text) || string.IsNullOrEmpty(Bank2.Text))
        {
            Error.Text = "Please enter a valid bank number (empty)";
            return;
        }

        if (string.IsNullOrWhiteSpace(Bank1.Text) || string.IsNullOrWhiteSpace(Bank2.Text))
        {
            Error.Text = "Please enter a valid bank number (whitespace)";
            return;
        }

        var b1 = Int32.Parse(Bank1.Text);
        var b2 = Int32.Parse(Bank2.Text);
        if (b1 < 1 || b1 > 16)
        {
            Error.Text = "Invalid bank number (expected: 1-16)";
            return;
        }
        if (b2 < 1 || b2 > 16)
        {
            Error.Text = "Invalid bank number (expected: 1-16)";
            return;
        }
        if (b1 == b2)
        {
            Error.Text = "Bank numbers must be different";
            return;
        }

        // TODO: Do the swap, close window
        BankUtils.SwapBanks(MainViewModel.LoadedProject, b1, b2);

        this.Close();
        

    }
}