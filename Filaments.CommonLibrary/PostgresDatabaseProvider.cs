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
        public string ConnString { get; set; } = "";

        public PostgresDatabaseProvider() { }

        private void UpdateConnString()
        {
            ConnString = $"Host={Credentials.Host};Port={Credentials.Port};" +
                         $"Username={Credentials.Username};Password={Credentials.Password};" +
                         $"Database={Credentials.Database}";
        }


        public async Task<Filament[]> GetFilaments()
        {
            var result = new List<Filament>();

            UpdateConnString();
            var dataSource = new NpgsqlDataSourceBuilder(ConnString).Build();
            var conn = await dataSource.OpenConnectionAsync();
            await using var cmd = new NpgsqlCommand("select * from f.full_view", conn);
            await using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                result.Add(ParseFilament(reader));
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
