using Filaments.CommonLibrary;

Configuration.Change(new FileInfo(".env"));
var filaments = await DatabaseManager.GetFilaments();

//foreach (var f in filaments)
//{
//    Console.WriteLine(f);
//}

filaments[0].Vendor = "Bambu Lab";

if (Configuration.Provider != null)
{
    await Configuration.Provider.EditFilament(filaments[0]);
    Filament f = new Filament(50, "nikto", "PLA", 19.99, "#123123", "Gray", null, null, 200, 230, 50, 60, 1000, 200,
        1000);

    await Configuration.Provider.AddFilament(f);
}
else
{
    Console.WriteLine("Provider is not set");
}




