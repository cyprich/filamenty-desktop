using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;

namespace Filaments.CommonLibrary
{
    public class SqliteDatabaseProvider : IDatabaseProvider
    {
        public string Name { get; } = "Sqlite";

        private SqliteConnection GetConn()
        {
            return new SqliteConnection($"Data Source={Configuration.FilePath}");
        }

        public async Task<Filament[]> GetFilaments()
        {
            await using var conn = GetConn();
            conn.Open();

            var cmd = conn.CreateCommand();
            cmd.CommandText =
                """
                select * 
                from filament
                order by id
                """;
            cmd.ExecuteNonQuery();

            var result = new List<Filament>();
            await using (var reader = await cmd.ExecuteReaderAsync())
            {
                while (reader.Read())
                {
                    var filament = GetFilamentFromReader(reader);
                    if (filament != null)
                    {
                        result.Add(filament);
                    }
                }
            }

            return result.ToArray();
        }

        public async Task EditFilament(Filament filament)
        {
            await using var conn = GetConn();
            conn.Open();

            await using var cmd = new SqliteCommand(
                $"""
                update filament set 
                vendor = $1, 
                material = $2, 
                price = $3, 
                color_hex = $4, 
                color_name = $5, 
                color2_hex = $6, 
                color2_name = $7, 
                temp_min = $8, 
                temp_max = $9, 
                temp_bed_min = $10, 
                temp_bed_max = $11, 
                measured_weight = $12, 
                spool_weight = $13, 
                original_weight = $14 
                where id = {filament.Id};
                """, conn
            );

            // TODO this is duplicity
            cmd.Parameters.AddWithValue("$1", filament.Vendor);
            cmd.Parameters.AddWithValue("$2", filament.Material);
            cmd.Parameters.AddWithValue("$3", filament.Price);
            cmd.Parameters.AddWithValue("$4", filament.ColorHex);
            cmd.Parameters.AddWithValue("$5", filament.ColorName);
            cmd.Parameters.AddWithValue("$6", (object?)filament.Color2Hex ?? DBNull.Value);
            cmd.Parameters.AddWithValue("$7", (object?)filament.Color2Name ?? DBNull.Value);
            cmd.Parameters.AddWithValue("$8", filament.TempMin);
            cmd.Parameters.AddWithValue("$9", (object?)filament.TempMax ?? DBNull.Value);
            cmd.Parameters.AddWithValue("$10", filament.TempBedMin);
            cmd.Parameters.AddWithValue("$11", (object?)filament.TempBedMax ?? DBNull.Value);
            cmd.Parameters.AddWithValue("$12", filament.MeasuredWeight);
            cmd.Parameters.AddWithValue("$13", filament.SpoolWeight);
            cmd.Parameters.AddWithValue("$14", filament.OriginalWeight);

            await cmd.ExecuteReaderAsync();
        }

        public async Task AddFilament(Filament filament)
        {
            await using var conn = GetConn();
            conn.Open();

            await using var cmd = new SqliteCommand(
                """
                insert into filament(
                    vendor, material, price, 
                    color_hex, color_name, color2_hex, color2_name, 
                    temp_min, temp_max, temp_bed_min, temp_bed_max, 
                    measured_weight, spool_weight, original_weight)
                values ( $1, $2, $3, $4, $5, $6, $7, $8, $9, $10, $11, $12, $13, $14 )
                """, conn
            );
            cmd.Parameters.AddWithValue("$1", filament.Vendor);
            cmd.Parameters.AddWithValue("$2", filament.Material);
            cmd.Parameters.AddWithValue("$3", filament.Price);
            cmd.Parameters.AddWithValue("$4", filament.ColorHex);
            cmd.Parameters.AddWithValue("$5", filament.ColorName);
            cmd.Parameters.AddWithValue("$6", (object?)filament.Color2Hex ?? DBNull.Value);
            cmd.Parameters.AddWithValue("$7", (object?)filament.Color2Name ?? DBNull.Value);
            cmd.Parameters.AddWithValue("$8", filament.TempMin);
            cmd.Parameters.AddWithValue("$9", (object?)filament.TempMax ?? DBNull.Value);
            cmd.Parameters.AddWithValue("$10", filament.TempBedMin);
            cmd.Parameters.AddWithValue("$11", (object?)filament.TempBedMax ?? DBNull.Value);
            cmd.Parameters.AddWithValue("$12", filament.MeasuredWeight);
            cmd.Parameters.AddWithValue("$13", filament.SpoolWeight);
            cmd.Parameters.AddWithValue("$14", filament.OriginalWeight);

            await cmd.ExecuteReaderAsync();
        }

        public async Task DeleteFilament(Filament filament)
        {
            await using var conn = GetConn();
            conn.Open();

            await using var cmd = new SqliteCommand($"delete from filament where id = {filament.Id}", conn);
            await cmd.ExecuteReaderAsync();
        }

        public async Task DeleteFilament(int id)
        {
            await using var conn = GetConn();
            conn.Open();

            await using var cmd = new SqliteCommand($"delete from filament where id = {id}", conn);
            await cmd.ExecuteReaderAsync();
        }

        public Task<bool> TestConnection()
        {
            return Task.FromResult(false);  // TODO
        }

        public Filament? GetFilamentFromReader<T>(T reader)
        {
            if (reader is not SqliteDataReader sqliteReader)
            {
                return null;
            }

            // columns that can be null
            var o = sqliteReader.GetOrdinal("color2_hex");
            var color2Hex = sqliteReader.IsDBNull(o)
                ? null
                : sqliteReader.GetString(o);

            o = sqliteReader.GetOrdinal("color2_name");
            var color2Name = sqliteReader.IsDBNull(o)
                ? null
                : sqliteReader.GetString(o);

            o = sqliteReader.GetOrdinal("temp_max");
            int? tempMax = sqliteReader.IsDBNull(o)
                ? null
                : sqliteReader.GetInt32(o);

            o = sqliteReader.GetOrdinal("temp_bed_max");
            int? tempBedMax = sqliteReader.IsDBNull(o)
                ? null
                : sqliteReader.GetInt32(o);

            return new Filament(
                sqliteReader.GetInt32(sqliteReader.GetOrdinal("id")),
                sqliteReader.GetString(sqliteReader.GetOrdinal("vendor")),
                sqliteReader.GetString(sqliteReader.GetOrdinal("material")),
                sqliteReader.GetDouble(sqliteReader.GetOrdinal("price")),
                sqliteReader.GetString(sqliteReader.GetOrdinal("color_hex")),
                sqliteReader.GetString(sqliteReader.GetOrdinal("color_name")),
                color2Hex,
                color2Name,
                sqliteReader.GetInt32(sqliteReader.GetOrdinal("temp_min")),
                tempMax,
                sqliteReader.GetInt32(sqliteReader.GetOrdinal("temp_bed_min")),
                tempBedMax,
                sqliteReader.GetInt32(sqliteReader.GetOrdinal("measured_weight")),
                sqliteReader.GetInt32(sqliteReader.GetOrdinal("spool_weight")),
                sqliteReader.GetInt32(sqliteReader.GetOrdinal("original_weight"))
            );
        }

        public Task PrepareDatabase()
        {
            try
            {
                using var conn = GetConn();
                conn.Open();

                var cmd = new SqliteCommand("select * from filament", conn);
                cmd.ExecuteNonQuery();
            }
            catch (SqliteException e)
            {
                Console.WriteLine(e);
                using var conn = GetConn();
                conn.Open();

                var cmd = new SqliteCommand(
                    """
                    drop table if exists filament;
                    create table filament (
                        id integer not null primary key autoincrement,
                        vendor text not null,
                        material text not null,
                        price real not null,
                        color_hex text not null,
                        color_name text not null,
                        color2_hex text,
                        color2_name text,
                        temp_min integer not null,
                        temp_max integer,
                        temp_bed_min integer not null,
                        temp_bed_max integer,
                        measured_weight integer not null,
                        spool_weight integer not null,
                        original_weight integer not null
                    )
                    """, conn
                );
                cmd.ExecuteNonQuery();
                SqliteConnection.ClearAllPools();
            }

            return Task.CompletedTask;


        }
    }
}
