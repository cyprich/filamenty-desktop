using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
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

        private Vendor _vendor = new();
        public Vendor Vendor
        {
            get => _vendor;
            set
            {
                _vendor = value;
                OnPropertyChanged(nameof(Vendor));
            }
        }

        private Material _material = new();
        public Material Material
        {
            get => _material;
            set
            {
                _material = value;
                OnPropertyChanged(nameof(Material));
            }
        }

        private Spool _spool = new();
        public Spool Spool
        {
            get => _spool;
            set
            {
                _spool = value;
                OnPropertyChanged(nameof(Spool));
            }
        }

        private float _price;
        public float Price
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

        private int _weight;
        public int Weight
        {
            get => _weight;
            set
            {
                _weight = value;
                OnPropertyChanged(nameof(Weight));
            }
        }

        public string TempSecondValue => TempMax != null ? $" - {TempMax}" : "";
        public string TempBedSecondValue => TempBedMax != null ? $" - {TempBedMax}" : "";
        public int WeightLeft => Math.Max(Weight - Spool.Weight, 0);

        public string PriceFormatted => $"{Price:F2}";

        // private ? image


        public Filament(int id, Vendor vendor, Material material, Spool spool, float price, string colorHex, string colorName, string? color2Hex, string? color2Name, int tempMin, int? tempMax, int tempBedMin, int? tempBedMax, int weight)
        {
            Id = id;
            Vendor = vendor;
            Material = material;
            Spool = spool;
            Price = price;
            ColorHex = colorHex;
            ColorName = colorName;
            Color2Hex = color2Hex;
            Color2Name = color2Name;
            TempMin = tempMin;
            TempMax = tempMax;
            TempBedMin = tempBedMin;
            TempBedMax = tempBedMax;
            Weight = weight;
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
            result += $"By {Vendor.Name}, ";
            result += $"Material {Material.Name}, ";
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
            result += $"Weight {Weight}g";

            return result;
        }
    }
}
