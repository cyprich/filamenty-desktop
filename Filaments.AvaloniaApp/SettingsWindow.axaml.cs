using System.Collections.ObjectModel;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Filaments.CommonLibrary;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;
using Tmds.DBus.Protocol;

namespace Filaments.AvaloniaApp;

public partial class SettingsWindow : Window
{
    private readonly IDatabaseProvider[] _providers = [new PostgresDatabaseProvider(), new SqliteDatabaseProvider()];
    public SettingsWindow()
    {
        InitializeComponent();
        DatabaseComboBox.ItemsSource = _providers.Select(p => p.Name);

        switch (Configuration.Provider?.Name.ToLower())
        {
            case "postgre":
            case "postgres":
            case "postgresql":
                DatabaseComboBox.SelectedIndex = 0;
                break;
            case "sqlite":
                DatabaseComboBox.SelectedIndex = 1;
                break;
        }

        PostgresUsername.Text = Configuration.Username;
        PostgresPassword.Text = Configuration.Password;
        PostgresAddress.Text = Configuration.Host;
        PostgresPort.Text = Configuration.Port;
        PostgresName.Text = Configuration.Database;

        DatabaseComboBox_OnSelectionChanged(null, null);
    }

    private void HandleSubmit(object? sender, RoutedEventArgs e)
    {

        if (!ValidateParameters())
        {
            var box = MessageBoxManager.GetMessageBoxStandard("Invalid fields", 
                    "Couldn't submit\nPlease make sure that all fields are filled in correctly",
                    ButtonEnum.Ok, MsBox.Avalonia.Enums.Icon.Error);
            _ = box.ShowAsPopupAsync(this);
            return;
        }

        Configuration.Username = PostgresUsername.Text!;
        Configuration.Password = PostgresPassword.Text!;
        Configuration.Host = PostgresAddress.Text!;
        Configuration.Port = PostgresPort.Text!;
        Configuration.Database = PostgresName.Text!;

        var selected = DatabaseComboBox.SelectedItem?.ToString()?.ToLower();

        switch (selected)
        {
            case "postgresql":
                Configuration.Provider = new PostgresDatabaseProvider();
                Configuration.Username = PostgresUsername.Text!;
                Configuration.Password = PostgresPassword.Text!;
                Configuration.Host = PostgresAddress.Text!;
                Configuration.Port = PostgresPort.Text!;
                Configuration.Database = PostgresName.Text!;
                break;
            case "sqlite":
                break;
            default:
                return;
        }

        Close(true);
    }


    private void HandleCancel(object? sender, RoutedEventArgs e)
    {
        Close(false);
    }

    private void DatabaseComboBox_OnSelectionChanged(object? sender, SelectionChangedEventArgs? e)
    {
        var selected = DatabaseComboBox.SelectedItem?.ToString();
        PostgresGrid.IsVisible = selected == "PostgreSQL";
        SqliteGrid.IsVisible = selected == "Sqlite";
    }

    private bool ValidateParameters()
    {
        var selected = DatabaseComboBox.SelectedItem?.ToString()?.ToLower();

        switch (DatabaseComboBox.SelectedItem?.ToString()?.ToLower())
        {
            case "postgresql":
                if (string.IsNullOrEmpty(PostgresUsername.Text) ||
                    string.IsNullOrEmpty(PostgresPassword.Text) ||
                    string.IsNullOrEmpty(PostgresAddress.Text) ||
                    string.IsNullOrEmpty(PostgresPort.Text) ||
                    string.IsNullOrEmpty(PostgresName.Text))
                {
                    return false;
                }
                return true;

            case "sqlite":
                return false;
            default:
                return false;
        }
    }
}