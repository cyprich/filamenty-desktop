using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Npgsql;

namespace Filaments.CommonLibrary
{
    public class PostgresDatabaseProvider : IDatabaseProvider
    {
        public string Name => "PostgreSQL";

        public string ConnString { get; set; } = "";

        public NpgsqlDataSource? DataSource { get; private set; }

        private void Update()
        {
            ConnString = $"Host={Configuration.Host};Port={Configuration.Port};" +
                         $"Username={Configuration.Username};Password={Configuration.Password};" +
                         $"Database={Configuration.Database}";
            DataSource = new NpgsqlDataSourceBuilder(ConnString).Build();
        }

        public async Task<Filament[]> GetFilaments()
        {
            var result = new List<Filament>();

            Update();
            if (DataSource != null)
            {
                Console.WriteLine("Loading data from database...");

                var conn = await DataSource.OpenConnectionAsync();
                await using var cmd = new NpgsqlCommand($"select * from {Configuration.Schema}.filament;", conn);

                NpgsqlDataReader? reader = null;
                try
                {
                    reader = await cmd.ExecuteReaderAsync();
                }
                catch (NpgsqlException e)
                {
                    Console.WriteLine(e);
                    Create();
                    reader = await cmd.ExecuteReaderAsync();
                }

                while (await reader.ReadAsync())
                {
                    result.Add(ParseFilament(reader));
                }
            }

            return result.ToArray();
        }

        // NOTE - Begin of AI-generated code
        public Filament ParseFilament(object? reader)
        {
            if (reader is not NpgsqlDataReader npgsqlReader)
            {
                throw new ArgumentException("Expected NpgsqlDataReader");
            }

            return ParseFilament(npgsqlReader);
        }
        // NOTE - End of AI-generated code

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
                reader.GetInt32(reader.GetOrdinal("id")),
                reader.GetString(reader.GetOrdinal("vendor")),
                reader.GetString(reader.GetOrdinal("material")),
                reader.GetFloat(reader.GetOrdinal("price")),
                reader.GetString(reader.GetOrdinal("color_hex")),
                reader.GetString(reader.GetOrdinal("color_name")),
                color2Hex,
                color2Name,
                reader.GetInt32(reader.GetOrdinal("temp_min")),
                tempMax,
                reader.GetInt32(reader.GetOrdinal("temp_bed_min")),
                tempBedMax,
                reader.GetInt32(reader.GetOrdinal("measured_weight")),
                reader.GetInt32(reader.GetOrdinal("spool_weight")),
                reader.GetInt32(reader.GetOrdinal("original_weight"))
            );
        }

        public async void Create()
        {
            Update();
            await using var command = DataSource?.CreateCommand(
                $"drop schema if exists {Configuration.Schema};" +
                $"create schema {Configuration.Schema}" +
                $"create table {Configuration.Schema}.filament (" +
                $"id serial primary key," +
                $"vendor varchar(50) not null," +
                $"material varchar(50) not null," +
                $"price numeric(5, 2) not null," +
                $"color_hex char(7) not null," +
                $"color_name varchar(50) not null," +
                $"color2_hex char(7)," +
                $"color2_name varchar(50)," +
                $"temp_min numeric(3, 0) not null," +
                $"temp_max numeric(3, 0)," +
                $"temp_bed_min numeric(3, 0) not null," +
                $"temp_bed_max numeric(3, 0)," +
                $"measured_weight numeric(5, 0)," +
                $"spool_weight numeric(5, 0)," +
                $"original_weight numeric(5, 0)" +
                $")"
            );
        }
    }
}
