using Npgsql;

namespace Filaments.CommonLibrary
{
    public class DatabaseHandler
    {
        private string _host = "";
        private string _port = "";
        private string _username = "";
        private string _password = "";
        private string _database = "";
        private string _connString = "";

        public DatabaseHandler() { }

        public DatabaseHandler(string host, string port, string username, string password, string database)
        {
            _host = host;
            _port = port;
            _username = username;
            _password = password;
            _database = database;
            UpdateConnString();
        }

        public void ChangeCredentials(string host, string port, string username, string password, string database)
        {
            _host = host;
            _port = port;
            _username = username;
            _password = password;
            _database = database;
            UpdateConnString();
        }

        public bool LoadCredentials(FileInfo file)
        {
            try
            {
                var config = File.ReadAllLines(file.FullName)
                    .Select(line => line.Split("="))
                    .ToDictionary(part => part[0], part => part[1]);

                //if (!(
                //        config.ContainsKey("host") &&
                //        config.ContainsKey("username") &&
                //        config.ContainsKey("password") &&
                //        config.ContainsKey("database") &&
                //        config.ContainsKey("port")
                //))
                //{
                //    throw new InvalidDataException("Configuration file does not contain one of required fields " +
                //        "(host, port, username, password, database)");
                //}

                ChangeCredentials(
                    config["host"],
                    config["port"],
                    config["username"],
                    config["password"],
                    config["database"]
                );
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }

            return true;
        }

        private void UpdateConnString()
        {
            _connString = $"Host={_host};Port={_port};Username={_username};Password={_password};Database={_database}";
        }

        public async Task<Filament[]> GetFilaments()
        {
            var result = new List<Filament>();

            var dataSource = new NpgsqlDataSourceBuilder(_connString).Build();
            var conn = await dataSource.OpenConnectionAsync();
            await using var cmd = new NpgsqlCommand("select * from f.full_view", conn);
            await using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                result.Add(ParseFilament(reader));
            }

            return result.ToArray();
        }

        private static Filament ParseFilament(NpgsqlDataReader reader)
        {
            // columns that can be null
            string? color2Hex = reader.IsDBNull(reader.GetOrdinal("color2_hex"))
                ? null
                : reader.GetString(reader.GetOrdinal("color2_hex"));
            string? color2Name = reader.IsDBNull(reader.GetOrdinal("color2_name"))
                ? null
                : reader.GetString(reader.GetOrdinal("color2_name"));
            int? tempMax = reader.IsDBNull(reader.GetOrdinal("temp_max"))
                ? null
                : reader.GetInt32(reader.GetOrdinal("temp_max"));
            int? tempBedMax = reader.IsDBNull(reader.GetOrdinal("temp_bed_max"))
                ? null
                : reader.GetInt32(reader.GetOrdinal("temp_bed_max"));


            return new Filament(
                reader.GetInt32(reader.GetOrdinal("id_filament")),
                new Vendor(
                    reader.GetInt32(reader.GetOrdinal("id_vendor")),
                    reader.GetString(reader.GetOrdinal("name_vendor"))
                ),
                new Material(
                    reader.GetInt32(reader.GetOrdinal("id_material")),
                    reader.GetString(reader.GetOrdinal("name_material"))
                ),
                new Spool(
                    reader.GetInt32(reader.GetOrdinal("id_spool")),
                    reader.GetString(reader.GetOrdinal("spool_material")),
                    reader.GetInt32(reader.GetOrdinal("spool_weight")),
                    reader.GetInt32(reader.GetOrdinal("capacity"))
                ),
                reader.GetFloat(reader.GetOrdinal("price")),
                reader.GetString(reader.GetOrdinal("color_hex")),
                reader.GetString(reader.GetOrdinal("color_name")),
                color2Hex,
                color2Name,
                reader.GetInt32(reader.GetOrdinal("temp_min")),
                tempMax,
                reader.GetInt32(reader.GetOrdinal("temp_bed_min")),
                tempBedMax,
                reader.GetInt32(reader.GetOrdinal("weight"))
            );
        }
    }
}
