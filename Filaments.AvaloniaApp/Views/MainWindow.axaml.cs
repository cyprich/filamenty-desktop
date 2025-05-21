using System;
using System.IO;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Filaments.AvaloniaApp.ViewModels;
using Filaments.CommonLibrary;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;

namespace Filaments.AvaloniaApp.Views
{
    public partial class MainWindow : Window
    {

        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainWindowViewModel();
        }

        private async void HandleSettingsWindow(object? sender, RoutedEventArgs e)
        {
            var window = new SettingsWindow();
            _ = await window.ShowDialog<bool>(this);

            if (DataContext is MainWindowViewModel vm)
            {
                _ = vm.LoadFilamentsAsync();
            }
        }

        private void HandleAdd(object? sender, RoutedEventArgs e)
        {
            var window = new AddFilamentWindow();
            _ = window.ShowDialog(this);
        }

        private void HandleEdit(object? sender, RoutedEventArgs e)
        {
            _ = 0;
            //throw new NotImplementedException();
        }

        private void HandleDelete(object? sender, RoutedEventArgs e)
        {
            _ = 0;
            //throw new NotImplementedException();
        }
    }
}