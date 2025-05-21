using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Filaments.CommonLibrary
{
    public class Filament : INotifyPropertyChanged
    {

        private int _id;
        public int Id
        {
            get => _id;
            set
            {
                _id = value;
                OnPropertyChanged(nameof(Id));
            }
        }

        private string _vendor = "";
        public string Vendor
        {
            get => _vendor;
            set
            {
                _vendor = value;
                OnPropertyChanged(nameof(Vendor));
            }
        }

        private string _material = "";
        public string Material
        {
            get => _material;
            set
            {
                _material = value;
                OnPropertyChanged(nameof(Material));
            }
        }

        private double _price;
        public double Price
        {
            get => _price;
            set
            {
                _price = value;
                OnPropertyChanged(nameof(Price));
            }
        }

        private string _colorHex = "";
        public string ColorHex
        {
            get => _colorHex;
            set
            {
                _colorHex = value;
                OnPropertyChanged(nameof(ColorHex));
            }
        }

        private string _colorName = "";
        public string ColorName
        {
            get => _colorName;
            set
            {
                _colorName = value;
                OnPropertyChanged(nameof(ColorName));
            }
        }

        private string? _color2Hex;
        public string? Color2Hex
        {
            get => _color2Hex;
            set
            {
                _color2Hex = value;
                OnPropertyChanged(nameof(Color2Hex));
            }
        }

        private string? _color2Name;
        public string? Color2Name
        {
            get => _color2Name;
            set
            {
                _color2Name = value;
                OnPropertyChanged(nameof(Color2Name));
            }
        }

        private int _tempMin;
        public int TempMin
        {
            get => _tempMin;
            set
            {
                _tempMin = value;
                OnPropertyChanged(nameof(TempMin));
            }
        }

        private int? _tempMax;
        public int? TempMax
        {
            get => _tempMax;
            set
            {
                _tempMax = value;
                OnPropertyChanged(nameof(TempMax));
            }
        }

        private int _tempBedMin;
        public int TempBedMin
        {
            get => _tempBedMin;
            set
            {
                _tempBedMin = value;
                OnPropertyChanged(nameof(TempBedMin));
            }
        }

        private int? _tempBedMax;
        public int? TempBedMax
        {
            get => _tempBedMax;
            set
            {
                _tempBedMax = value;
                OnPropertyChanged(nameof(TempBedMax));
            }
        }

        private int _measuredWeight;
        public int MeasuredWeight
        {
            get => _measuredWeight;
            set
            {
                _measuredWeight = value;
                OnPropertyChanged(nameof(MeasuredWeight));
            }
        }

        private int _spoolWeight;

        public int SpoolWeight
        {
            get => _spoolWeight;

            set
            {
                _spoolWeight = value;
                OnPropertyChanged(nameof(SpoolWeight));
            }
        }

        private int _originalWeight;

        public int OriginalWeight
        {
            get => _originalWeight;
            set
            {
                _originalWeight = value;
                OnPropertyChanged(nameof(OriginalWeight));
            }
        }

        public string TempSecondValue => TempMax != null ? $" - {TempMax}" : "";
        public string TempBedSecondValue => TempBedMax != null ? $" - {TempBedMax}" : "";
        public int WeightLeft => Math.Max(MeasuredWeight - SpoolWeight, 0);
        public string PriceFormatted => $"{Price:F2}";
        public string CentsPerGram => $"{(Price / OriginalWeight * 100):F2}";


        public Filament(int id, string vendor, string material, double price, string colorHex, string colorName, string? color2Hex, string? color2Name, int tempMin, int? tempMax, int tempBedMin, int? tempBedMax, int measuredWeight, int spoolWeight, int originalWeight)
        {
            Id = id;
            Vendor = vendor;
            Material = material;
            Price = price;
            ColorHex = colorHex;
            ColorName = colorName;
            Color2Hex = color2Hex;
            Color2Name = color2Name;
            TempMin = tempMin;
            TempMax = tempMax;
            TempBedMin = tempBedMin;
            TempBedMax = tempBedMax;
            MeasuredWeight = measuredWeight;
            SpoolWeight = spoolWeight;
            OriginalWeight = originalWeight;
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        //protected bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
        //{
        //    if (EqualityComparer<T>.Default.Equals(field, value)) return false;
        //    field = value;
        //    OnPropertyChanged(propertyName);
        //    return true;
        //}

        public override string ToString()
        {
            var result = $"Filament #{Id}, ";
            result += $"By {Vendor}, ";
            result += $"Material {Material}, ";
            result += $"{Price} EUR, ";
            result += $"Color {ColorName} ({ColorHex}), ";
            result += !string.IsNullOrEmpty(Color2Name)
                ? $"Secondary Color {Color2Name} ({Color2Hex}), "
                : "";
            result += $"Temperature {TempMin}" + 
            (TempMax != null 
                ? $"-{TempMax}"
                : "") +
            "°C, ";
            result += $"Bed Temperature {TempBedMin}" + 
            (TempBedMax != null 
                ? $"-{TempBedMax}"
                : "") +
            "°C, ";
            result += $"Weight {MeasuredWeight}g";

            return result;
        }
    }
}
