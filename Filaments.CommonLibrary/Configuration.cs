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
        public static IDatabaseProvider? Provider { get; set; }
        public static string[] Fields => ["host", "port", "username", "password", "provider", "schema"];

        public static bool IsCorrect { get; private set; }

        public static async Task<bool> Change(string host, string port, string username, string password, string schema, IDatabaseProvider provider)
        {
            Host = host;
            Port = port;
            Username = username;
            Password = password;
            Schema = schema;
            Provider = provider;

            await Provider.PrepareDatabase();

            IsCorrect = true;
            return IsCorrect;
        }

        public static async Task<bool> Change(string host, string port, string username, string password, string schema, string provider)
        {
            Host = host;
            Port = port;
            Username = username;
            Password = password;
            Schema = schema;

            IsCorrect = ChangeProvider(provider);

            if (Provider != null)
            {
                await Provider.PrepareDatabase();
            }

            return IsCorrect;
        }

        public static async Task <bool> Change(Dictionary<string, string> configDictionary)
        {
            foreach (var f in Fields)
            {
                if (!configDictionary.ContainsKey(f))
                {
                    IsCorrect = false;
                    return IsCorrect;
                }
            }

            var d = configDictionary;
            return await Change(d["host"], d["port"],
                d["username"], d["password"],
                d["schema"], d["provider"]
            );
        }

        public static async Task<bool> Change(FileInfo file)
        {
            if (!File.Exists(file.FullName))
            {
                IsCorrect = false;
                return IsCorrect;
            }

            try
            {
                return await Change((await File.ReadAllLinesAsync(file.FullName))
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
        public static bool ChangeProvider(string provider)
        {
            switch (provider.ToLower())
            {
                case "postgre":
                case "postgres":
                case "postgresql":
                    Provider = new PostgresDatabaseProvider();
                    return true;
                case "sqlite":
                    Provider = new SqliteDatabaseProvider();
                    return true;
                default:
                    return false;
            }
        }

        public static bool Save(FileInfo file)
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
                File.WriteAllLines(file.FullName, lines);
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
