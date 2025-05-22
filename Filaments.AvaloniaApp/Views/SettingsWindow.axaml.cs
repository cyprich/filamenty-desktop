using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
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

        UpdateUi();
        //DatabaseComboBox_OnSelectionChanged(null, null);
    }

    private void UpdateUi()
    {
        PostgresUsername.Text = Configuration.Username;
        PostgresPassword.Text = Configuration.Password;
        PostgresAddress.Text = Configuration.Host;
        PostgresPort.Text = Configuration.Port;
        PostgresSchema.Text = Configuration.Schema;

        SqliteTextBox.Text = Configuration.FilePath;

        DatabaseComboBox.SelectedItem = Configuration.Provider?.Name;
        DatabaseComboBox_OnSelectionChanged(null, null);
    }

    private async void HandleExit(object? sender, RoutedEventArgs? e)
    {
        if (await ValidateParametersMessageBox() == false) { return; }
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
                    PostgresSchema.Text!
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
        bool result;
        switch (DatabaseComboBox.SelectedItem?.ToString()?.ToLower())
        {
            case "postgresql":
                result = !(string.IsNullOrEmpty(PostgresUsername.Text) ||
                    string.IsNullOrEmpty(PostgresPassword.Text) ||
                    string.IsNullOrEmpty(PostgresAddress.Text) ||
                    string.IsNullOrEmpty(PostgresPort.Text) ||
                    string.IsNullOrEmpty(PostgresSchema.Text));
                break;

            case "sqlite":
                result = File.Exists(SqliteTextBox.Text);
                break;
            default:
                return false;
        }

        Configuration.Provider?.PrepareDatabase();
        return result;
    }

    private async Task<bool> ValidateParametersMessageBox()
    {
        if (!ValidateParameters())
        {
            var box = MessageBoxManager.GetMessageBoxStandard("Invalid fields",
                    "Couldn't submit\nPlease make sure that all fields are filled in correctly",
                    ButtonEnum.Ok, MsBox.Avalonia.Enums.Icon.Error);
            await box.ShowAsPopupAsync(this);
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
                var result = Configuration.Load(new FileInfo(path));
                if (!await result)
                {
                    var box = MessageBoxManager.GetMessageBoxStandard("Error",
                        "Failed to load configuration file" +
                        "\nMake sure the file is correct",
                        ButtonEnum.Ok,
                        MsBox.Avalonia.Enums.Icon.Error);
                    _ = box.ShowAsync();
                }
                else
                {
                    UpdateUi();
                }
            }
        }
    }

    private async void HandleSave(object? sender, RoutedEventArgs e)
    {
        var topLevel = TryGetTopLevel();

        if (topLevel != null)
        {
            var file = await topLevel.StorageProvider.SaveFilePickerAsync(
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
                if (await result)
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

    private void HandlePostgresDefaultsWindow(object? sender, RoutedEventArgs e)
    {
        var window = new PostgresDefaultsWindow();
        window.ShowDialog(this);
    }

    private async void HandleSqliteFileSelect(object? sender, RoutedEventArgs e)
    {
        var topLevel = TryGetTopLevel();

        if (topLevel == null)
        {
            return;
        }

        var dbFileFilter = new FilePickerFileType("Sqlite database file") { Patterns = ["*.sqlite", "*.db"] };
        var file = await topLevel.StorageProvider.SaveFilePickerAsync(
            new FilePickerSaveOptions()
            {
                Title = "Open or Create Sqlite file",
                ShowOverwritePrompt = false,
                FileTypeChoices = [dbFileFilter, FilePickerFileTypes.All]
            }
        );

        if (file != null)
        {
            var path = file.Path.LocalPath;

            // NOTE - begin of AI-generated code
            if (!File.Exists(path))
            {
                await using (File.Create(path)) { }  // creates a file if it doesn't exit
            }
            // NOTE - end of AI-generated code

            var result = Configuration.Change(path);
            if (!result)
            {
                var box = MessageBoxManager.GetMessageBoxStandard(
                    "Error", 
                    "Couldn't open file", 
                    ButtonEnum.Ok,
                    MsBox.Avalonia.Enums.Icon.Error);
                _ = box.ShowAsync();
            }
            else
            {
                UpdateUi();
            }
        }
    }

    private async void Window_OnClosing(object? sender, WindowClosingEventArgs e)
    {
        // TODO remember me
        e.Cancel = !(await ValidateParametersMessageBox());
    }

}