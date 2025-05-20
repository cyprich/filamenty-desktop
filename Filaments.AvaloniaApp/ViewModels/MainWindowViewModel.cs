using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Controls;
using Filaments.CommonLibrary;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;

namespace Filaments.AvaloniaApp.ViewModels
{
    public partial class MainWindowViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<Filament> Filaments { get; } = [];
        public int FilamentCount => Filaments.Count;

        public MainWindowViewModel()
        {
            _ = LoadFilamentsAsync();
            Filaments.CollectionChanged += Filaments_CollectionChanged;
        }

        private void Filaments_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            OnPropertyChanged(nameof(FilamentCount));
        }


        public async Task LoadFilamentsAsync()
        {
            Filaments.Clear();
            var filaments = await DatabaseHandler.GetFilaments();
            foreach (var f in filaments)
            {
                Filaments.Add(f);
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
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
    }
}
