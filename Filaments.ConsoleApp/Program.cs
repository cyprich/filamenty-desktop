using Filaments.CommonLibrary;

DatabaseHandler dbhandler = new();
//dbhandler.ChangeCredentials("192.168.88.3", "cyprich", "lepacapaska");
if (!dbhandler.LoadCredentials(new FileInfo(".env")))
{
    return;
}

var x = dbhandler.GetFilaments();

foreach (var f in x.Result)
{
    Console.WriteLine(f);
}

