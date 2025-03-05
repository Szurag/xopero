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
                    new ArchiveExplorerService().ArchiveExplorer();
                    break;
                case "2":
                    CreateArchive();
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

    private static void CreateArchive()
    {
        var path = GetPathService.GetPath();
        
        var archivePath = GetPathService.GetNewArchivePath();
        
        var compressionLevel = AnsiConsole.Prompt(
            new TextPrompt<int>("Podaj poziom kompresji (0-9):")
                .PromptStyle("green")
                .ValidationErrorMessage("[red]Niepoprawny poziom kompresji[/]")
                .Validate(level => level is >= 0 and <= 9));
        var password = AnsiConsole.Prompt(
            new TextPrompt<string?>("Podaj hasło (opcjonalnie):")
                .PromptStyle("green")
                .AllowEmpty());
        
        CreateArchiveService.CreateArchive(path, archivePath, compressionLevel, password);
        
        AnsiConsole.MarkupLine($"[green]Archiwum zostało utworzone pod ścieżką {archivePath}[/]");
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