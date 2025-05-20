using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Controls;
using Filaments.CommonLibrary;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;

namespace Filaments.AvaloniaApp.ViewModels
{
    public partial class MainWindowViewModel : ViewModelBase
    {
        public ObservableCollection<Filament> Filaments { get; private set; } = [];

        public MainWindowViewModel()
        {
            _ = LoadFilamentsAsync();
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
    }
}
