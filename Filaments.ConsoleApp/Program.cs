using Filaments.CommonLibrary;

Configuration.Change("pokus.sqlite");
Console.WriteLine($"Configuration is {(Configuration.IsCorrect ? "correct" : "incorrect")}");

await Configuration.Provider!.PrepareDatabase();


var x = await Configuration.Provider!.GetFilaments();

if (x.Length > 0)
{
    var selected = x[0];
    selected.Vendor = "haleluja";
    await Configuration.Provider.EditFilament(selected);
}


foreach (var f in x)
{
    Console.WriteLine(f);
}

