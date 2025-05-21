using Filaments.CommonLibrary;

Configuration.Change(new FileInfo(".env"));
var filaments = await DatabaseManager.GetFilaments();

foreach (var f in filaments)
{
    Console.WriteLine(f);
}

