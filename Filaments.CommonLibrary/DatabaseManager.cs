using Npgsql;

namespace Filaments.CommonLibrary
{
    public static class DatabaseManager
    {
        public static async Task<Filament[]> GetFilaments()
        {
            if (Configuration.Provider == null)
            {
                return [];
            }

            var filaments = await Configuration.Provider.GetFilaments();
            return filaments;
        }
    }
}
