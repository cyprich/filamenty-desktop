using Npgsql;

namespace Filaments.CommonLibrary
{
    public static class DatabaseManager
    {
        public static async Task<Filament[]> GetFilaments()
        {
            if (Configuration.Provider == null)
            {
                throw new InvalidOperationException("Database provider is not set.");
            }

            var filaments = await Configuration.Provider.GetFilaments();
            return filaments;
        }

    }
}
