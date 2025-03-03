using System.IO.Compression;
using ICSharpCode.SharpZipLib.Zip;

namespace Szyfrator.Services;

public class MenuService
{
    public static void Menu()
    {
        var action = AskForAction();
        
        switch (action)
        {
            case "1":
                new ArchiveExplorerService().ArchiveExplorer();
                break;
            case "2":
                CreateArchiveService.CreateArchive();           
                break;
            default:
                Menu();
                break;
        }
    }

    private static string? AskForAction()
    {
        Console.WriteLine("Co chcesz robić?");
        Console.WriteLine("1 - zarządzanie archiwum");
        Console.WriteLine("2 - tworzenie archiwum");

        var option = Console.ReadLine();
        
        return option;
    }
}