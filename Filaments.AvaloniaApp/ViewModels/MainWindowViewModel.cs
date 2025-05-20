using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Filaments.CommonLibrary;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;

namespace Filaments.AvaloniaApp.ViewModels
{
    public partial class MainWindowViewModel : ViewModelBase
    {
        public ObservableCollection<Filament> Filaments { get; private set; }

        public MainWindowViewModel()
        {
            Filaments = [];
            LoadFilamentsAsync();
        }

        private async void LoadFilamentsAsync()
        {
            Credentials.Change(new FileInfo(".env"));
            DatabaseHandler.Provider = new PostgresDatabaseProvider();
            var filaments = await DatabaseHandler.GetFilaments();

            //var filaments = await Task.Run(() => dbHandler.GetFilaments());

            foreach (var f in filaments)
            {
                Filaments.Add(f);
            }
        }
    }
}
