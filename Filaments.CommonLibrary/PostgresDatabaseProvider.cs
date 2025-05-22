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

        private void UpdateConnString()
        {
            ConnString = $"Host={Configuration.Host};Port={Configuration.Port};" +
                $"Username={Configuration.Username};Password={Configuration.Password};" +
                $"Database=postgres"; 
        }

        private async Task<NpgsqlConnection> GetConnection()
        {
            UpdateConnString();

            var dataSource = new NpgsqlDataSourceBuilder(ConnString).Build();
            var conn = await dataSource.OpenConnectionAsync();

            return conn;
        }

        public async Task<Filament[]> GetFilaments()
        {
            var result = new List<Filament>();

            var conn = await GetConnection();
            await using var cmd = new NpgsqlCommand($"select * from {Configuration.Schema}.filament order by id;", conn);

            var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                result.Add(GetFilamentFromReader(reader));
            }

            return result.ToArray();
        }

        public async Task AddFilament(Filament filament)
        {
            var conn = await GetConnection();

            await using var cmd = new NpgsqlCommand($"insert into {Configuration.Schema}.filament " +
                $"(vendor, material, price, color_hex, color_name, color2_hex, color2_name, " +
                $"temp_min, temp_max, temp_bed_min, temp_bed_max, measured_weight, spool_weight, original_weight) " +
                $"values ( ($1), ($2), ($3), ($4), ($5), ($6), ($7), ($8), ($9), ($10), ($11), ($12), ($13), ($14) )", conn)
            {
                Parameters =
                {
                    new NpgsqlParameter {Value = filament.Vendor},
                    new NpgsqlParameter {Value = filament.Material},
                    new NpgsqlParameter {Value = filament.Price},
                    new NpgsqlParameter {Value = filament.ColorHex},
                    new NpgsqlParameter {Value = filament.ColorName},
                    new NpgsqlParameter {Value = (object?)filament.Color2Hex ?? DBNull.Value},
                    new NpgsqlParameter {Value = (object?)filament.Color2Name ?? DBNull.Value},
                    new NpgsqlParameter {Value = filament.TempMin},
                    new NpgsqlParameter {Value = (object?)filament.TempMax ?? DBNull.Value},
                    new NpgsqlParameter {Value = filament.TempBedMin},
                    new NpgsqlParameter {Value = (object?)filament.TempBedMax ?? DBNull.Value},
                    new NpgsqlParameter {Value = filament.MeasuredWeight},
                    new NpgsqlParameter {Value = filament.SpoolWeight},
                    new NpgsqlParameter {Value = filament.OriginalWeight},
                }
            };

            await cmd.ExecuteNonQueryAsync();
        }

        public async Task DeleteFilament(Filament filament)
        {
            var conn = await GetConnection();

            await using var cmd = new NpgsqlCommand(
                $"delete from {Configuration.Schema}.filament where id = {filament.Id};", conn);

            await cmd.ExecuteNonQueryAsync();
        }

        public async Task EditFilament(Filament filament)
        {
            var conn = await GetConnection();

            await using var cmd = new NpgsqlCommand(
                $"update {Configuration.Schema}.filament set " +
                $"vendor = ($1), " +
                $"material = ($2), " +
                $"price = ($3), " +
                $"color_hex = ($4), " +
                $"color_name = ($5), " +
                $"color2_hex = ($6), " +
                $"color2_name = ($7), " +
                $"temp_min = ($8), " +
                $"temp_max = ($9), " +
                $"temp_bed_min = ($10), " +
                $"temp_bed_max = ($11), " +
                $"measured_weight = ($12), " +
                $"spool_weight = ($13), " +
                $"original_weight = ($14) " +
                $"where id = {filament.Id}",
                conn
            )
            {
                Parameters =
                {
                    new NpgsqlParameter {Value = filament.Vendor},
                    new NpgsqlParameter {Value = filament.Material},
                    new NpgsqlParameter {Value = filament.Price},
                    new NpgsqlParameter {Value = filament.ColorHex},
                    new NpgsqlParameter {Value = filament.ColorName},
                    new NpgsqlParameter {Value = (object?)filament.Color2Hex ?? DBNull.Value},
                    new NpgsqlParameter {Value = (object?)filament.Color2Name ?? DBNull.Value},
                    new NpgsqlParameter {Value = filament.TempMin},
                    new NpgsqlParameter {Value = (object?)filament.TempMax ?? DBNull.Value},
                    new NpgsqlParameter {Value = filament.TempBedMin},
                    new NpgsqlParameter {Value = (object?)filament.TempBedMax ?? DBNull.Value},
                    new NpgsqlParameter {Value = filament.MeasuredWeight},
                    new NpgsqlParameter {Value = filament.SpoolWeight},
                    new NpgsqlParameter {Value = filament.OriginalWeight},
                    new NpgsqlParameter {Value = filament.Id}
                }
            };

            await cmd.ExecuteNonQueryAsync();
        }

        public Task<bool> TestConnection()
        {
            throw new NotImplementedException();
        }

        // NOTE - Begin of AI-generated code
        public Filament GetFilamentFromReader(object? reader)
        {
            if (reader is not NpgsqlDataReader npgsqlReader)
            {
                throw new ArgumentException("Expected NpgsqlDataReader");
            }

            return GetFilamentFromReader(npgsqlReader);
        }
        // NOTE - End of AI-generated code

        private static Filament GetFilamentFromReader(NpgsqlDataReader reader)
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

        public async Task PrepareDatabase()
        {
            Console.WriteLine($"Preparing Database... ConnString: {ConnString}");
            await HandleMissingSchema();
            await HandleMissingTable();
        }

        public async Task HandleMissingSchema()
        {
            var conn = await GetConnection();
            await using var cmd = new NpgsqlCommand(
                $"create schema if not exists {Configuration.Schema};", conn);
            await cmd.ExecuteNonQueryAsync();
        }

        public async Task HandleMissingTable()
        {
            try
            {
                var conn = await GetConnection();
                await using var cmd = new NpgsqlCommand($"select * from {Configuration.Schema}.filament;", conn);
                await cmd.ExecuteNonQueryAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                var conn = await GetConnection();
                await using var cmd = new NpgsqlCommand(
                    $"drop table if exists {Configuration.Schema}.filament cascade;" +
                    $"create table {Configuration.Schema}.filament (" +
                    "id serial primary key," +
                    "vendor varchar(50) not null," +
                    "material varchar(50) not null," +
                    "price numeric(5, 2) not null," +
                    "color_hex char(7) not null," +
                    "color_name varchar(50) not null," +
                    "color2_hex char(7)," +
                    "color2_name varchar(50)," +
                    "temp_min numeric(3, 0) not null," +
                    "temp_max numeric(3, 0)," +
                    "temp_bed_min numeric(3, 0) not null," +
                    "temp_bed_max numeric(3, 0)," +
                    "measured_weight numeric(5, 0)," +
                    "spool_weight numeric(5, 0)," +
                    "original_weight numeric(5, 0)" +
                    ")", conn);

                await cmd.ExecuteNonQueryAsync();
            }
        }
    }
}
