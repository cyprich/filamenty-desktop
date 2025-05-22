using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Avalonia.Data.Converters;
using Avalonia.Media;

namespace Filaments.AvaloniaApp
{
    public static class Utils
    {
        // https://stackoverflow.com/questions/13354892/converting-from-rgb-ints-to-hex
        public static String ColorToHex(Color color)
        {
            return $"#{color.R:X2}{color.G:X2}{color.B:X2}";
        }
    }

    // NOTE - begin of AI-generated code
    public class HexToColorConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is string hex && !string.IsNullOrWhiteSpace(hex))
            {
                return Color.Parse(hex);
            }
            return Colors.White;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is Color color)
            {
                return color.ToString();
            }
            return "#FFFFFFFF"; 
        }
    }
    // NOTE - end of AI-generated code
}
