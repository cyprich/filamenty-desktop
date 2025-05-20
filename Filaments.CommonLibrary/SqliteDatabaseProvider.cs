using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Filaments.CommonLibrary
{
    public class SqliteDatabaseProvider : IDatabaseProvider
    {
        public Task<Filament[]> GetFilaments()
        {
            throw new NotImplementedException();
        }

        public Filament ParseFilament(object? reader)
        {
            throw new NotImplementedException();
        }
    }
}
