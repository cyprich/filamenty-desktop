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

        public Filament ParseFilament(object? reader);

        protected void Create();
    }
}
