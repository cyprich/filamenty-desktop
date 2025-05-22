using System.Runtime.CompilerServices;
using Filaments.CommonLibrary;
using Spectre.Console;

namespace Filaments.ConsoleApp;

internal class Program
{
    public static Filament[] Filaments { get; set; } = [];
    public static Table MainTable { get; set; } = new();

    public static async Task Main(string[] args)
    {
        // Set up table
        await RecreateTable();

        // Set up other elements
        var options = new Panel(
            new Rows(
                new Text("1. Show filaments"),
                new Text("2. Add filament"),
                new Text("3. Edit filament"),
                new Text("4. Delete filament"),
                new Text("5. Settings"),
                new Text("6. Exit")
            )
        );
        const int numberOfOptions = 6; // WARNING - hard-coded value
        options.Header("\nMain menu");
        options.Padding(2, 1, 2, 1);

        // Set up console
        Console.Clear();
        AnsiConsole.Write(new Markup("[gray]*Please put your console in fullscreen mode*[/]\n"));
        AnsiConsole.Write(new Markup("[gray]*Please begin with Settings*[/]\n\n"));
        AnsiConsole.Write(new FigletText("Filaments").Color(Color.Green));
        Console.WriteLine();

        while (true)
        {
            AnsiConsole.Write(options);

            var choice = AnsiConsole.Prompt(
                new TextPrompt<int>("Your choice: ")
                    .Validate((o) => o switch
                    {
                        < 1 => ValidationResult.Error("Invalid input"),
                        > numberOfOptions => ValidationResult.Error("Invalid input"),
                        _ => ValidationResult.Success()
                    })
            );
            Console.WriteLine();

            // TODO make this into list of functions? 
            switch (choice)
            {
                case 1:
                    ShowTable();
                    break;
                case 2:
                    await AddFilament();
                    break;
                case 3:
                    await EditFilament();
                    break;
                case 4:
                    await DeleteFilament();
                    break;
                case 5:
                    await Settings();
                    break;
                case 6:
                    AnsiConsole.Write(new Markup("Goodbye [yellow]:)[/]"));
                    return;
            }
        }
    }

    public static async Task AddFilament()
    {
        if (!CheckDatabaseConfiguration()) { return; }

        var vendor = AnsiConsole.Prompt(new TextPrompt<string>("Vendor:"));
        var material = AnsiConsole.Prompt(new TextPrompt<string>("Material:"));
        var price = AnsiConsole.Prompt(new TextPrompt<double>("Price:"));
        var colorName = AnsiConsole.Prompt(new TextPrompt<string>("Primary Color (name):"));
        var colorHex = AnsiConsole.Prompt(new TextPrompt<string>("Primary Color (HEX value):"));
        var color2Name = AnsiConsole.Prompt(new TextPrompt<string?>("[[Optional]] Secondary Color (name):").AllowEmpty());
        var color2Hex = AnsiConsole.Prompt(new TextPrompt<string?>("[[Optional]] Secondary Color (HEX value):").AllowEmpty());
        var tempMin = AnsiConsole.Prompt(new TextPrompt<int>("Temperature (minimal):"));
        var tempMax = AnsiConsole.Prompt(new TextPrompt<int?>("[[Optional]] Temperature (maximal):").AllowEmpty().DefaultValue(null));
        var tempBedMin = AnsiConsole.Prompt(new TextPrompt<int>("Bed Temperature (minimal):"));
        var tempBedMax = AnsiConsole.Prompt(new TextPrompt<int?>("[[Optional]] Bed Temperature (maximal):").AllowEmpty().DefaultValue(null));
        var measuredWeight = AnsiConsole.Prompt(new TextPrompt<int>("Measured weight:"));
        var spoolWeight = AnsiConsole.Prompt(new TextPrompt<int>("Spool weight:"));
        var originalWeight = AnsiConsole.Prompt(new TextPrompt<int>("Original weight:"));

        _ = string.IsNullOrEmpty(color2Name ?? "") ? (color2Name = null) : "";
        _ = string.IsNullOrEmpty(color2Hex ?? "") ? (color2Hex = null) : "";
        _ = tempMax <= 0 ? (tempMax = null) : 0;
        _ = tempBedMax <= 0 ? (tempBedMax = null) : 0;

        // NOTE - I used null suppression,
        // because it's already checked by CheckDatabaseConfiguration()
        await Configuration.Provider!.AddFilament(new Filament(-1, vendor, material, price,
            colorHex, colorName, color2Hex, color2Name,
            tempMin, tempMax, tempBedMin, tempBedMax,
            measuredWeight, spoolWeight, originalWeight));

        await RecreateTable();

        AnsiConsole.Write(new Markup("\n[green]Success[/] New Filament added\n\n"));
    }

    public static async Task EditFilament()
    {
        if (!CheckDatabaseConfiguration()) { return; }

        var id = AnsiConsole.Prompt(new TextPrompt<int>("ID of filament to edit:"));

        if (!ValidateFilamentId(id)) { return; }

        var f = Filaments.First(f => f.Id == id);

        f.Vendor = AnsiConsole.Prompt(new TextPrompt<string>("Vendor:").DefaultValue(f.Vendor));
        f.Material = AnsiConsole.Prompt(new TextPrompt<string>("Material:").DefaultValue(f.Material));
        f.Price = AnsiConsole.Prompt(new TextPrompt<double>("Price:").DefaultValue(Math.Round(f.Price, 2)));
        f.ColorName = AnsiConsole.Prompt(new TextPrompt<string>("Primary Color (name):").DefaultValue(f.ColorName));
        f.ColorHex = AnsiConsole.Prompt(new TextPrompt<string>("Primary Color (HEX value):").DefaultValue(f.ColorHex));
        var color2Name = AnsiConsole.Prompt(new TextPrompt<string?>("[[Optional]] Secondary Color (name):").AllowEmpty().DefaultValue(f.Color2Name));
        var color2Hex = AnsiConsole.Prompt(new TextPrompt<string?>("[[Optional]] Secondary Color (HEX value):").AllowEmpty().DefaultValue(f.Color2Hex));
        f.TempMin = AnsiConsole.Prompt(new TextPrompt<int>("Temperature (minimal):").DefaultValue(f.TempMin));
        var tempMax = AnsiConsole.Prompt(new TextPrompt<int?>("[[Optional]] Temperature (maximal):").AllowEmpty().DefaultValue(f.TempMax));
        f.TempMax = AnsiConsole.Prompt(new TextPrompt<int>("Bed Temperature (minimal):").DefaultValue(f.TempBedMin));
        var tempBedMax = AnsiConsole.Prompt(new TextPrompt<int?>("[[Optional]] Bed Temperature (maximal):").AllowEmpty().DefaultValue(f.TempBedMax));
        f.MeasuredWeight = AnsiConsole.Prompt(new TextPrompt<int>("Measured weight:").DefaultValue(f.MeasuredWeight));
        f.SpoolWeight = AnsiConsole.Prompt(new TextPrompt<int>("Spool weight:").DefaultValue(f.SpoolWeight));
        f.OriginalWeight = AnsiConsole.Prompt(new TextPrompt<int>("Original weight:").DefaultValue(f.OriginalWeight));

        f.Color2Name = string.IsNullOrEmpty(color2Name) ? null : color2Name;
        f.Color2Hex = string.IsNullOrEmpty(color2Hex) ? null : color2Hex;
        f.TempMax = tempMax <= 0 ? null : tempMax;
        f.TempBedMax = tempBedMax <= 0 ? null : tempBedMax;

        // NOTE - I used null suppression,
        // because it's already checked by CheckDatabaseConfiguration()
        await Configuration.Provider!.EditFilament(f);

        await RecreateTable();

        AnsiConsole.Write(new Markup("\n[green]Success[/] New Filament added\n\n"));
    }

    public static async Task DeleteFilament()
    {
        if (!CheckDatabaseConfiguration()) { return; }

        var id = AnsiConsole.Prompt(new TextPrompt<int>("Filament ID:"));
        if (!ValidateFilamentId(id)) { return; }

        // NOTE - I used null suppression,
        // because it's already checked by CheckDatabaseConfiguration()
        await Configuration.Provider!.DeleteFilament(id);

        AnsiConsole.Write(new Markup("[Green]Success![/] filament deleted\n\n"));
    }

    public static async Task Settings()
    {
        var options = new Panel(
            new Rows(
                new Text("1. Load settings from file"),
                new Text("2. Connect to PostgreSQL database"),
                new Text("3. Connect to Sqlite database"),
                new Text("4. Exit")
            )
        );
        const int numberOfOptions = 4; // WARNING - hard-coded value
        options.Header("Settings");
        options.Padding(2, 1, 2, 1);

        while (true)
        {
            AnsiConsole.Write(options);

            var choice = AnsiConsole.Prompt(
                new TextPrompt<int>("Your choice: ")
                    .Validate((o) => o switch
                    {
                        < 1 => ValidationResult.Error("Invalid input"),
                        > numberOfOptions => ValidationResult.Error("Invalid input"),
                        _ => ValidationResult.Success()
                    })
            );
            Console.WriteLine();
            var result = false;
            switch (choice)
            {
                case 1:
                    result = await LoadSettingsFromFile();
                    break;
                case 2:
                    result = SetUpPostgres();
                    break;
                case 3:
                    result = SetUpSqlite();
                    break;
                case 4:
                    return;
            }

            if (result)
            {
                await RecreateTable();
                AnsiConsole.Write(new Markup("\n[green]Success![/] Database configured\n\n"));
                return;
            }
            AnsiConsole.Write(new Markup("\n[red]Error![/] Failed to configure database\n\n"));
        }

    }

    public static async Task<bool> LoadSettingsFromFile()
    {
        var filePath = AnsiConsole.Prompt(new TextPrompt<string>("Configuration file path:"));
        return await Configuration.Load(new FileInfo(filePath));
    }

    public static bool SetUpPostgres()
    {
        var username = AnsiConsole.Prompt(new TextPrompt<string>("Username:"));
        var password = AnsiConsole.Prompt(new TextPrompt<string>("Password:").Secret());
        var address = AnsiConsole.Prompt(new TextPrompt<string>("IP address: "));
        var port = AnsiConsole.Prompt(new TextPrompt<string>("Port:").DefaultValue("5432"));
        var schema = AnsiConsole.Prompt(new TextPrompt<string>("Schema:").DefaultValue("filaments"));

        return Configuration.Change(address, port, username, password, schema);
    }

    public static bool SetUpSqlite()
    {
        var file = AnsiConsole.Prompt(new TextPrompt<string>("Sqlite file location:"));
        return Configuration.Change(file);
    }

    public static bool CheckDatabaseConfiguration()
    {
        if (Configuration.Provider == null)
        {
            AnsiConsole.Write(new Markup(
                "[red]Error![/]" +
                "\nIt seems like you are not connected to database" +
                "\nPlease set up database in [yellow]Settings[/]" +
                "\n\n"));
            return false;
        }

        return true;
    }

    public static void ShowTable()
    {
        AnsiConsole.Write(MainTable);
        Console.WriteLine();
    }

    public static async Task RecreateTable()
    {
        var p = Configuration.Provider;

        if (p != null)
        {
            Filaments = await p.GetFilaments();
        }

        MainTable = new Table();
        MainTable.Title($"[Green]Filaments[/] (Database: " +
            $"{(Configuration.Provider?.Name ?? "[red]None[/]")}, " +
            $"[yellow]{Filaments.Length}[/] rows)");

        MainTable.AddColumn("ID");
        MainTable.AddColumn("Vendor");
        MainTable.AddColumn("Material");
        MainTable.AddColumn("Price");
        MainTable.AddColumn("Primary Color");
        MainTable.AddColumn("Secondary Color");
        MainTable.AddColumn("Temperature");
        MainTable.AddColumn("Bed Temperature");
        MainTable.AddColumn("Weight");
        MainTable.AddColumn("Spool Weight");
        MainTable.AddColumn("Weight (Full)");

        MainTable.ShowRowSeparators();

        if (p != null)
        {
            foreach (var f in Filaments)
            {
                string color2Text = "";
                if (f.Color2Name != null && f.Color2Hex != null)
                {
                    color2Text = $"{f.Color2Name} [gray]({f.Color2Hex})[/]";
                }

                MainTable.AddRow(
                    new Text($"{f.Id}"),
                    new Text($"{f.Vendor}"),
                    new Text($"{f.Material}"),
                    new Text($"{f.PriceFormatted}"),
                    new Markup($"{f.ColorName} [gray]({f.ColorHex})[/]"),
                    new Markup(color2Text),
                    new Text($"{f.TempMin} {(f.TempMax != null ? $"- {f.TempMax!}" : "")}"),
                    new Text($"{f.TempBedMin} {(f.TempBedMax != null ? $"- {f.TempBedMax!}" : "")}"),
                    new Text($"{f.MeasuredWeight}"),
                    new Text($"{f.SpoolWeight}"),
                    new Text($"{f.OriginalWeight}")
                );
            }
        }
    }

    public static bool ValidateFilamentId(int id)
    {
        var exists = Filaments.Any(f => f.Id == id);

        if (!exists)
        {
            AnsiConsole.Write(new Markup("[red]Error![/] Filament index invalid\n\n"));
            return false;
        }

        return true;
    } 
}