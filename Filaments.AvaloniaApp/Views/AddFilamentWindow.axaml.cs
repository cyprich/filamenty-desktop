using System;
using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Filaments.CommonLibrary;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;

namespace Filaments.AvaloniaApp;

public partial class AddFilamentWindow : Window
{
    public AddFilamentWindow()
    {
        InitializeComponent();
        Opened += CheckDatabaseConnection;
    }

    private async void CheckDatabaseConnection(object? sender, EventArgs e)
    {
        if (!Configuration.IsCorrect)
        {
            var box = MessageBoxManager.GetMessageBoxStandard(
                "Warning",
                "It seems like you are not connected to database" +
                "\nYou can open Settings menu to connect to database" +
                "\nAdding filament will have no effect" +
                "\nDo you wish to continue?",
                ButtonEnum.YesNo,
                MsBox.Avalonia.Enums.Icon.Warning);
            var result = await box.ShowAsync();

            if (result == ButtonResult.No)
            {
                Close(false);
            }
        }
    }

    private async void HandleAdd(object? sender, RoutedEventArgs e)
    {
        if (!ValidateFields())
        {
            var box = MessageBoxManager.GetMessageBoxStandard(
                "Error",
                "Couldn't add new filament" +
                "\nPlease make sure that all fields are filled",
                ButtonEnum.Ok,
                MsBox.Avalonia.Enums.Icon.Error);
            _ = box.ShowAsync();
            return;
        }


        var filament = new Filament(
            -1,
            VendorTextBox.Text ?? "",
            MaterialTextBox.Text ?? "",
            (double)(PriceNumberBox?.Value ?? 0),
            Utils.ColorToHex(ColorPicker.Color),
            ColorTextBox.Text ?? "",
            Utils.ColorToHex(Color2Picker.Color),
            Color2TextBox.Text,
            (int)(TempMinNumberBox.Value ?? 0),
            (int)(TempMaxNumberBox.Value ?? 0),
            (int)(TempBedMinNumberBox.Value ?? 0),
            (int)(TempBedMaxNumberBox.Value ?? 0),
            (int)(MeasuredWeightNumberBox.Value ?? 0),
            (int)(SpoolWeightNumberBox.Value ?? 0),
            (int)(OriginalWeightNumberBox.Value ?? 0)
        );

        if (Color2CheckBox != null && Color2CheckBox.IsChecked != true)
        {
            filament.Color2Hex = null;
            filament.Color2Name = null;
        }

        if (TempCheckBox != null && TempCheckBox.IsChecked != true)
        {
            filament.TempMax = null;
        }

        if (TempBedCheckBox != null && TempBedCheckBox.IsChecked != true)
        {
            filament.TempBedMax = null;
        }

        if (Configuration.Provider != null)
        {
            await Configuration.Provider.AddFilament(filament);
            Close(true);
        }
        else
        {
            Close(false);
        }
    }

    private bool ValidateFields()
    {
        // validating text boxes
        var requiredTextBoxes = new List<TextBox>() {VendorTextBox, MaterialTextBox, ColorTextBox};

        if (Color2CheckBox != null && Color2CheckBox.IsChecked == true)
        {
            requiredTextBoxes.Add(Color2TextBox);
        }

        foreach (var b in requiredTextBoxes)
        {
            if (string.IsNullOrEmpty(b.Text))
            {
                return false;
            }
        }

        // validating number boxes
        var requiredNumberBoxes = new List<NumericUpDown>()
        {
            PriceNumberBox, TempMinNumberBox, TempBedMinNumberBox,
            MeasuredWeightNumberBox, SpoolWeightNumberBox, OriginalWeightNumberBox
        };

        if (TempCheckBox != null && TempCheckBox.IsChecked == true)
        {
            requiredNumberBoxes.Add(TempMaxNumberBox);
        }

        if (TempBedCheckBox != null && TempBedCheckBox.IsChecked == true)
        {
            requiredNumberBoxes.Add(TempBedMaxNumberBox);
        }

        foreach (var b in requiredNumberBoxes)
        {
            if (b.Value == null || b.Value <= 0)
            {
                return false;
            }
        }

        // color pickers don't need to be validated,
        // because they always have a color (white by default)

        return true;
    }

    private void HandleCancel(object? sender, RoutedEventArgs e)
    {
        Close(false);
    }

}