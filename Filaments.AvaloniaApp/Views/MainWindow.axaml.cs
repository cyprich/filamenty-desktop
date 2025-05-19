using System;
using System.IO;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Filaments.AvaloniaApp.ViewModels;
using Filaments.CommonLibrary;

namespace Filaments.AvaloniaApp.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainWindowViewModel();
        }

        private void HandleSettingsWindow(object? sender, RoutedEventArgs e)
        {
            var window = new SettingsWindow();
            window.ShowDialog(this);
        }
    }
}