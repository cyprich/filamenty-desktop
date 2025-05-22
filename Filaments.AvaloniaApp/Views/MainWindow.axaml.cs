using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
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
            await UpdateUi();
        }

        private async void HandleAdd(object? sender, RoutedEventArgs e)
        {
            var window = new AddFilamentWindow();
            _ = await window.ShowDialog<bool>(this);
            await UpdateUi();
        }

        private async void HandleDelete(object? sender, RoutedEventArgs e)
        {
            var selectedItems = MainDataGrid.SelectedItems;
            if (selectedItems.Count > 0 && Configuration.Provider != null)
            {
                foreach (var i in selectedItems)
                {
                    await Configuration.Provider.DeleteFilament((Filament)i);
                }
            }
            await UpdateUi();
        }

        private async Task UpdateUi()
        {
            if (DataContext is MainWindowViewModel vm)
            {
                await vm.LoadFilaments();
            }
        }

        private async void HandleEdit(object? sender, DataGridRowEditEndedEventArgs e)
        {
            if (e.Row.DataContext is Filament filament && Configuration.Provider != null)
            {
                await Configuration.Provider.EditFilament(filament);
                await UpdateUi();
            }
        }
    }
}