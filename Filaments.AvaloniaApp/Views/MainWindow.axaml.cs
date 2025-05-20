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

            Opened += MainWindow_Opened;

        }

        private async void MainWindow_Opened(object? sender, EventArgs e)
        {
            if (File.Exists(".env"))
            {
                var box = MessageBoxManager.GetMessageBoxStandard("Load credentials?",
                    "Configuration file for database credentials found. Do you want to load it?",
                    ButtonEnum.YesNo, MsBox.Avalonia.Enums.Icon.Database
                );

                var result = await box.ShowAsync();
                if (result == ButtonResult.Yes)
                {
                    Configuration.Change(new FileInfo(".env"));

                    if (DataContext is MainWindowViewModel vm)
                    {
                        await vm.LoadFilamentsAsync();
                    }
                }
            }
        }

        private async void HandleSettingsWindow(object? sender, RoutedEventArgs e)
        {
            var window = new SettingsWindow();
            var result = await window.ShowDialog<bool>(this);

            if (result && DataContext is MainWindowViewModel vm)
            {
                _ = vm.LoadFilamentsAsync();
            }
        }
    }
}