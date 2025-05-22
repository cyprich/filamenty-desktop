using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Filaments.CommonLibrary
{
    public static class Configuration
    {
        public static string Host { get; set; } = "";
        public static string Port { get; set; } = "";
        public static string Username { get; set; } = "";
        public static string Password { get; set; } = "";
        public static string Schema { get; set; } = "";
        public static string FilePath { get; set; } = "";
        public static IDatabaseProvider? Provider { get; set; }
        public static string[] PostgresFields => ["host", "port", "username", "password", "provider", "schema"];

        public static bool IsCorrect { get; private set; }

        // for PostgreSQL
        public static bool Change(string host, string port, string username, string password, string schema)
        {
            Host = host;
            Port = port;
            Username = username;
            Password = password;
            Schema = schema;
            Provider = new PostgresDatabaseProvider();

            return IsCorrect;
        }

        // for Sqlite

        public static bool Change(string sqliteFilePath)
        {
            if (!File.Exists(sqliteFilePath))
            {
                IsCorrect = false;
            }
            else
            {
                FilePath = sqliteFilePath;
                Provider = new SqliteDatabaseProvider();
                IsCorrect = true;
            }
            return IsCorrect;
        }

        public static bool Change(Dictionary<string, string> configDictionary)
        {
            if (!configDictionary.TryGetValue("provider", out var provider))
            {
                IsCorrect = false;
                return IsCorrect;
            }


            switch (provider.ToLower())
            {
                case "postgre":
                case "postgres":
                case "postgresql":
                    foreach (var f in PostgresFields)
                    {
                        if (!configDictionary.ContainsKey(f))
                        {
                            IsCorrect = false;
                            return IsCorrect;
                        }
                    }
                    var d = configDictionary;
                    return Change(d["host"], d["port"],
                        d["username"], d["password"],
                        d["schema"]);

                case "sqlite":
                    if (!configDictionary.TryGetValue("file", out var filename) || !File.Exists(filename))
                    {
                        IsCorrect = false;
                        return IsCorrect;
                    }

                    FilePath = filename;
                    IsCorrect = true;
                    return IsCorrect;

                default:
                    IsCorrect = false;
                    return IsCorrect;
            }
        }

        public static async Task<bool> Load(FileInfo file)
        {
            if (!File.Exists(file.FullName))
            {
                IsCorrect = false;
                return IsCorrect;
            }

            try
            {
                return Change((await File.ReadAllLinesAsync(file.FullName))
                    .Select(line => line.Split("="))
                    .ToDictionary(part => part[0], part => part[1]));
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                IsCorrect = false;
                return IsCorrect;
            }
        }
        public static async Task<bool> Save(FileInfo file)
        {
            var lines = new List<string>
            {
                $"provider={Provider?.Name}",
                $"username={Username}",
                $"password={Password}",
                $"host={Host}",
                $"port={Port}",
                $"schema={Schema}"
            };

            try
            {
                await File.WriteAllLinesAsync(file.FullName, lines);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }

            return true;
        }
    }
}
