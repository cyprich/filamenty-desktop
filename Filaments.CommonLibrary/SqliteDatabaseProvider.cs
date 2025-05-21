using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Filaments.CommonLibrary
{
    public class SqliteDatabaseProvider : IDatabaseProvider
    {
        public string Name { get; } = "Sqlite";

        public Task<Filament[]> GetFilaments()
        {
            throw new NotImplementedException();
        }

        public Task EditFilament(Filament filament)
        {
            throw new NotImplementedException();
        }

        public Task AddFilament(Filament filament)
        {
            throw new NotImplementedException();
        }

        public Task DeleteFilament(Filament filament)
        {
            throw new NotImplementedException();
        }

        public Task<bool> TestConnection()
        {
            throw new NotImplementedException();
        }

        public Filament GetFilamentFromReader(object? reader)
        {
            throw new NotImplementedException();
        }

        public Task HandleMissingSchema()
        {
            throw new NotImplementedException();
        }

        public Task HandleMissingTable()
        {
            throw new NotImplementedException();
        }
    }
}
