using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Filaments.CommonLibrary
{
    public interface IDatabaseProvider
    {
        public string Name { get; }

        public Task<Filament[]> GetFilaments();

        public Task EditFilament(Filament filament);

        public Task AddFilament(Filament filament);

        public Task DeleteFilament(Filament filament);

        public Task<bool> TestConnection();

        public Filament GetFilamentFromReader(object? reader);


        public Task PrepareDatabase();

        public Task HandleMissingSchema();

        public Task HandleMissingTable();
    }
}
