using Npgsql;

namespace Filaments.CommonLibrary
{
    public static class DatabaseHandler
    {
        public static IDatabaseProvider? Provider { get; set; }

        public static async Task<Filament[]> GetFilaments()
        {
            if (Provider == null)
            {
                throw new InvalidOperationException("Database provider is not set.");
            }

            var filaments = await Provider.GetFilaments();
            return filaments;
        }

    }
}
