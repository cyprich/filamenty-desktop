using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using Filaments.CommonLibrary;

namespace Filaments.AvaloniaApp.ViewModels
{
    public partial class MainWindowViewModel : ViewModelBase
    {
        public ObservableCollection<Filament> Filaments { get; private set; }

        public MainWindowViewModel()
        {
            Filaments = new ObservableCollection<Filament>();
            LoadFilamentsAsync();
        }

        private async void LoadFilamentsAsync()
        {
            var dbHandler = new DatabaseHandler();
            dbHandler.LoadCredentials(new FileInfo(".env"));
            var result = await Task.Run(() => dbHandler.GetFilaments());

            foreach (var f in result)
            {
                Filaments.Add(f);
            }
        }
    }
}
