using Archiver.Exceptions;
using Spectre.Console;

namespace Archiver.Services;

public static class ArchiveExplorerMenuService
{
    public static void ShowMenu()
    {
        var archivePath = GetPathService.GetArchivePath();
        var password = ArchiveHelperService.GetPassword();

        try
        {
            var archiveExplorerService = new ArchiveExplorerService(archivePath, password);
            var exit = false;

            while (!exit)
            {
                var action = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("Wybierz opcję:")
                        .PageSize(10)
                        .AddChoices("1 - Przeglądaj pliki", "2 - Dodaj plik/folder", "3 - Usuń plik/folder",
                            "4 - Wyszukaj plik/folder", "5 - Powrót do menu głównego"));

                switch (action.Split(' ')[0])
                {
                    case "1":
                        DisplayArchiveTree(archiveExplorerService);
                        break;
                    case "2":
                        AddFileToArchive(archiveExplorerService);
                        break;
                    case "3":
                        RemoveFileFromArchive(archiveExplorerService);
                        break;
                    case "4":
                        SearchInArchive(archiveExplorerService);
                        break;
                    case "5":
                        exit = true;
                        break;
                }
            }
        }
        catch (Exception ex)
        {
            AnsiConsole.MarkupLine($"[red]Błąd: {ex.Message}[/]");
        }
    }

    private static void DisplayArchiveTree(ArchiveExplorerService archiveExplorerService)
    {
        archiveExplorerService.DisplayTree();
    }

    private static void SearchInArchive(ArchiveExplorerService archiveExplorerService)
    {
        var searchTerm = AnsiConsole.Prompt(
            new TextPrompt<string>("Podaj nazwę pliku/folderu do wyszukania:")
                .PromptStyle("green"));

        var results = archiveExplorerService.SearchFilesWithIndices(searchTerm);

        if (results.Count == 0)
        {
            AnsiConsole.MarkupLine("[yellow]Nie znaleziono plików/folderów o podanej nazwie.[/]");
            return;
        }

        AnsiConsole.MarkupLine($"[green]Znaleziono {results.Count} wyników:[/]");
        foreach (var result in results)
        {
            var icon = result.Item1.IsDirectory ? "📁" : "📄";
            var color = result.Item1.IsDirectory ? "blue" : "green";
            AnsiConsole.MarkupLine($"{icon} [{color}]{result.Item2})[/] {result.Item1.FullPath}{(result.Item1.IsPassword ? " 🔒" : "")}");
        }
    }

    private static void AddFileToArchive(ArchiveExplorerService archiveExplorerService)
    {
        try
        {
            DisplayArchiveTree(archiveExplorerService);

            var index = AnsiConsole.Prompt(
                new TextPrompt<string>("Podaj index folderu, do którego chcesz dodać plik/folder:")
                    .PromptStyle("green"));

            var path = GetPathService.GetPath();
            var compressionLevel = ArchiveHelperService.GetCompressionLevel();

            archiveExplorerService.AddFileToArchive(path, compressionLevel, index);
            AnsiConsole.MarkupLine("[green]Plik/folder został dodany do archiwum[/]");
        }
        catch (IncorrectIndexException)
        {
            AnsiConsole.MarkupLine("[red]Podany index jest niepoprawny[/]");
        }
        catch (CannotAddFileToFileException)
        {
            AnsiConsole.MarkupLine("[red]Nie można dodać pliku do pliku[/]");
        }
        catch (Exception ex)
        {
            AnsiConsole.MarkupLine($"[red]Błąd: {ex.Message}[/]");
        }
    }

    private static void RemoveFileFromArchive(ArchiveExplorerService archiveExplorerService)
    {
        try
        {
            DisplayArchiveTree(archiveExplorerService);

            var index = AnsiConsole.Prompt(
                new TextPrompt<string>("Podaj index pliku/folderu do usunięcia:")
                    .PromptStyle("green"));

            archiveExplorerService.RemoveFileFromArchive(index);
            AnsiConsole.MarkupLine("[green]Plik/folder został usunięty z archiwum[/]");
        }
        catch (IncorrectIndexException)
        {
            AnsiConsole.MarkupLine("[red]Podany index jest niepoprawny[/]");
        }
        catch (Exception ex)
        {
            AnsiConsole.MarkupLine($"[red]Błąd: {ex.Message}[/]");
        }
    }
}