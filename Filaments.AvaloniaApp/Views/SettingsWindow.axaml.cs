using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Platform.Storage;
using Filaments.CommonLibrary;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;
using Tmds.DBus.Protocol;

namespace Filaments.AvaloniaApp;

public partial class SettingsWindow : Window
{
    private readonly IDatabaseProvider[] _providers = [new PostgresDatabaseProvider(), new SqliteDatabaseProvider()];

    private bool AbleToClose { get; set; }

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

        UpdateUiFromConfiguration();
        //DatabaseComboBox_OnSelectionChanged(null, null);
    }

    private void UpdateUiFromConfiguration()
    {
        PostgresUsername.Text = Configuration.Username;
        PostgresPassword.Text = Configuration.Password;
        PostgresAddress.Text = Configuration.Host;
        PostgresPort.Text = Configuration.Port;
        PostgresName.Text = Configuration.Database;
        PostgresSchema.Text = Configuration.Schema;
        DatabaseComboBox.SelectedItem = Configuration.Provider?.Name;
        DatabaseComboBox_OnSelectionChanged(null, null);
    }

    private void HandleExit(object? sender, RoutedEventArgs? e)
    {
        if (!ValidateParametersMessageBox()) { return; }
        var selected = DatabaseComboBox.SelectedItem?.ToString()?.ToLower();

        switch (selected)
        {
            case "postgresql":
                // NOTE - I used null suppression, because the values are validated in ValidateParameters() function
                var result = Configuration.Change(
                    PostgresAddress.Text!,
                    PostgresPort.Text!,
                    PostgresUsername.Text!,
                    PostgresPassword.Text!,
                    PostgresName.Text!,
                    PostgresSchema.Text!,
                    new PostgresDatabaseProvider()
                );

                if (!result)
                {
                    var box = MessageBoxManager.GetMessageBoxStandard("Error",
                        "Failed to save configuration" +
                        "\nMake sure that all fields are filled in and valid.",
                        ButtonEnum.Ok, MsBox.Avalonia.Enums.Icon.Error);
                    _ = box.ShowAsync();
                    return;
                }
                break;
            case "sqlite":
                break;
            default:
                return;
        }

        Close(true);
    }

    private void DatabaseComboBox_OnSelectionChanged(object? sender, SelectionChangedEventArgs? e)
    {
        var selected = DatabaseComboBox.SelectedItem?.ToString();
        PostgresGrid.IsVisible = selected == "PostgreSQL";
        SqliteGrid.IsVisible = selected == "Sqlite";
    }

    private bool ValidateParameters()
    {
        switch (DatabaseComboBox.SelectedItem?.ToString()?.ToLower())
        {
            case "postgresql":
                if (string.IsNullOrEmpty(PostgresUsername.Text) ||
                    string.IsNullOrEmpty(PostgresPassword.Text) ||
                    string.IsNullOrEmpty(PostgresAddress.Text) ||
                    string.IsNullOrEmpty(PostgresPort.Text) ||
                    string.IsNullOrEmpty(PostgresName.Text) ||
                    string.IsNullOrEmpty(PostgresSchema.Text))
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

    private bool ValidateParametersMessageBox()
    {
        if (!ValidateParameters())
        {
            var box = MessageBoxManager.GetMessageBoxStandard("Invalid fields",
                    "Couldn't submit\nPlease make sure that all fields are filled in correctly",
                    ButtonEnum.Ok, MsBox.Avalonia.Enums.Icon.Error);
            _ = box.ShowAsPopupAsync(this);
            return false;
        }

        return true;
    }

    private TopLevel? TryGetTopLevel()
    {
        var topLevel = TopLevel.GetTopLevel(this);

        if (topLevel?.StorageProvider == null)
        {
            var box = MessageBoxManager.GetMessageBoxStandard("Error",
                "StorageProvider is not available.",
                ButtonEnum.Ok,
                MsBox.Avalonia.Enums.Icon.Error);
            _ = box.ShowAsync();
            return null;
        }

        return topLevel;
    }

    private async void HandleLoad(object? sender, RoutedEventArgs e)
    {
        var topLevel = TryGetTopLevel();

        if (topLevel != null)
        {
            var envFileFilter = new FilePickerFileType("Environment file") { Patterns = ["*.env"] };
            var txtFileFilter = new FilePickerFileType("Text file") { Patterns = ["*.txt"] };

            var files = await topLevel.StorageProvider.OpenFilePickerAsync(
                new FilePickerOpenOptions()
                {
                    Title = "Open file",
                    AllowMultiple = false,
                    FileTypeFilter = [envFileFilter, txtFileFilter, FilePickerFileTypes.All]
                }
            );
            if (files.Count >= 1)
            {
                var path = files[0].Path.LocalPath;
                var result = Configuration.Change(new FileInfo(path));
                if (result)
                {
                    UpdateUiFromConfiguration();
                    var box = MessageBoxManager.GetMessageBoxStandard("Success",
                        "Successfully loaded file",
                        ButtonEnum.Ok,
                        MsBox.Avalonia.Enums.Icon.Success);
                    _ = box.ShowAsync();
                }
                else
                {
                    var box = MessageBoxManager.GetMessageBoxStandard("Error",
                        "Failed to load configuration file" +
                        "\nMake sure the file is correct",
                        ButtonEnum.Ok,
                        MsBox.Avalonia.Enums.Icon.Error);
                    _ = box.ShowAsync();
                }
            }
        }
    }
    private async void HandleSave(object? sender, RoutedEventArgs e)
    {
        var toplevel = TryGetTopLevel();

        if (toplevel != null)
        {
            var file = await toplevel.StorageProvider.SaveFilePickerAsync(
                new FilePickerSaveOptions
                {
                    DefaultExtension = ".env",
                    ShowOverwritePrompt = true,
                    SuggestedFileName = ".env"
                }
            );

            if (file != null)
            {
                var path = file.Path.LocalPath;
                var result = Configuration.Save(new FileInfo(path));
                if (result)
                {
                    var box = MessageBoxManager.GetMessageBoxStandard(
                        "Success", 
                        "Settings successfully saved",
                        ButtonEnum.Ok, 
                        MsBox.Avalonia.Enums.Icon.Info
                    );
                    _ = box.ShowAsync();
                }
                else
                {
                    var box = MessageBoxManager.GetMessageBoxStandard(
                        "Error", 
                        "Could not save settings",
                        ButtonEnum.Ok, 
                        MsBox.Avalonia.Enums.Icon.Warning
                    );
                    _ = box.ShowAsync();
                }
            }
        }
    }

    private void Window_OnClosing(object? sender, WindowClosingEventArgs e)
    {
        e.Cancel = !ValidateParametersMessageBox();
    }

    private void HandlePostgresDefaults(object? sender, RoutedEventArgs e)
    {
        Configuration.Change("192.168.1.1", "5432", 
            "admin", "admin", 
            "postgres", "filaments", "PostgreSQL");
        UpdateUiFromConfiguration();
    }

    private void HandlePostgresDefaultsWindow(object? sender, RoutedEventArgs e)
    {
        var window = new PostgresDefaultsWindow();
        window.ShowDialog(this);
    }
}