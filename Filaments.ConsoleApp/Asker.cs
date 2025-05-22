using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Spectre.Console;

namespace Filaments.ConsoleApp
{

    internal static class Asker
    {
        public static int? AskValues(string? title, string[] options)
        {
            if (options.Length <= 0)
            {
                Console.WriteLine("There is nothing to do...");
                return null;
            }

            if (title != null)
            {
                Console.WriteLine(title);
            }

            for (int i = 0; i < options.Length; i++)
            {
                Console.WriteLine($"\t{i+1}. {options[i]}");
            }

            int max = options.Length;
            while (true)
            {
                Console.WriteLine("\tYour option: ");
                string? input = Console.ReadLine();

                if (int.TryParse(input, out int parsed) && parsed >= 1 && parsed <= max)
                {
                    return parsed;
                }

                new Markup("[red]Invalid input...[/}");
            }
        }
    }
}
