using System;
using System.Collections.Generic;
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
        public static string Database { get; set; } = "";
        public static IDatabaseProvider? Provider { get; set; }
        public static string[] RequiredFields => ["host", "port", "username", "password", "database", "provider"];

        public static bool Change(string host, string port, string username, string password, string database, IDatabaseProvider provider)
        {
            Host = host;
            Port = port;
            Username = username;
            Password = password;
            Database = database;
            Provider = provider;

            return true;
        }

        public static bool Change(string host, string port, string username, string password, string database, string provider)
        {
            Host = host;
            Port = port;
            Username = username;
            Password = password;
            Database = database;

            return ChangeProvider(provider);
        }


        public static bool Change(Dictionary<string, string> configDictionary)
        {
            foreach (var f in RequiredFields)
            {
                if (!configDictionary.ContainsKey(f))
                {
                    return false;
                }
            }

            var d = configDictionary;
            return Change(d["host"], d["port"], d["username"], d["password"], d["database"], d["provider"]);
        }

        public static bool Change(FileInfo file)
        {
            if (!File.Exists(file.FullName))
            {
                return false;
            }

            return Change(File.ReadAllLines(file.FullName)
                .Select(line => line.Split("="))
                .ToDictionary(part => part[0], part => part[1]));
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
    }
}
