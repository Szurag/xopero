using Archiver.Exceptions;
using Spectre.Console;

namespace Archiver.Services;

public static class MenuService
{
    public static void Menu()
    {
        var exit = false;

        do
        {
            var action = AskForAction();

            switch (action)
            {
                case "1":
                    ArchiveExplorerMenuService.ShowMenu();
                    break;
                case "2":
                    ArchiveCreatorService.CreateArchive();
                    break;
                case "3":
                    exit = true;
                    break;
                default:
                    AnsiConsole.MarkupLine("[red]Niepoprawna opcja, spróbuj ponownie.[/]");
                    break;
            }
        } while (!exit);
    }

    private static string? AskForAction()
    {
        var option = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("Co chcesz robić?")
                .PageSize(10)
                .AddChoices("1 - zarządzanie archiwum", "2 - tworzenie archiwum", "3 - Wyjdź"));

        return option.Split(' ')[0];
    }
}