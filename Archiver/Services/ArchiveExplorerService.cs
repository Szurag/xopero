using Archiver.Models;
using ICSharpCode.SharpZipLib.Zip;
using Spectre.Console;

namespace Archiver.Services;

public class ArchiveExplorerService
{
    private readonly List<ZipItem> _zipItems = [];
    private string? _password = null;

    public void ArchiveExplorer()
    {
        var archivePath = GetPathService.GetPath();

        if (string.IsNullOrEmpty(archivePath) || !File.Exists(archivePath))
        {
            AnsiConsole.MarkupLine("[red]Nieprawidłowa ścieżka do archiwum[/]");
            return;
        }

        try
        {
            AnsiConsole.Status().Start("Otwieranie archiwum...", ctx => MapArchive(archivePath));
            AnsiConsole.MarkupLine("[green]Archiwum zostało otwarte pomyślnie[/]");
        }
        catch (Exception ex)
        {
            AnsiConsole.MarkupLine($"[red]Błąd podczas otwierania archiwum: {ex.Message}[/]");
            return;
        }

        var exit = false;

        do
        {
            AnsiConsole.Clear();
            AnsiConsole.Write(new FigletText("Archiver").Centered().Color(Color.Green));
            AnsiConsole.MarkupLine($"[grey]Otwarte archiwum: [/][green]{Path.GetFileName(archivePath)}[/]");

            var option = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("Wybierz akcję:")
                    .PageSize(10)
                    .AddChoices(
                        "1 - Przeglądaj archiwum",
                        "2 - Dodaj plik do archiwum",
                        "3 - Usuń plik z archiwum",
                        "4 - Wyszukaj po nazwie",
                        "5 - Wyjdź"));

            switch (option)
            {
                case "1 - Przeglądaj archiwum":
                    Display();
                    break;
                case "2 - Dodaj plik do archiwum":
                    AddFileToArchive(archivePath);
                    break;
                case "3 - Usuń plik z archiwum":
                    RemoveFileFromArchive(archivePath);
                    break;
                case "4 - Wyszukaj po nazwie":
                    SearchByName();
                    break;
                case "5 - Wyjdź":
                    exit = true;
                    break;
            }
        } while (!exit);
    }

    private void SearchByName()
    {
        if (_zipItems.Count == 0)
        {
            AnsiConsole.MarkupLine("[yellow]Archiwum jest puste.[/]");
            return;
        }

        var name = AnsiConsole.Ask<string>("[green]Podaj nazwę pliku/folderu, który chcesz wyszukać:[/]");

        if (string.IsNullOrEmpty(name))
        {
            AnsiConsole.MarkupLine("[red]Nazwa nie może być pusta[/]");
            return;
        }

        var results = SearchByNameLinq(_zipItems, name);

        if (results.Count == 0)
        {
            AnsiConsole.MarkupLine("[yellow]Nie znaleziono pliku/folderu o podanej nazwie[/]");
            return;
        }

        AnsiConsole.MarkupLine($"[green]Znaleziono {results.Count} plików/folderów:[/]");

        var table = new Table();
        table.AddColumn("Typ");
        table.AddColumn("Nazwa");
        table.AddColumn("Ścieżka");

        foreach (var result in results)
        {
            table.AddRow(
                result.IsDirectory ? "[blue]Folder[/]" : "[yellow]Plik[/]",
                result.Name ?? "",
                result.FullPath ?? ""
            );
        }

        AnsiConsole.Write(table);

        Console.WriteLine("\nNaciśnij dowolny klawisz, aby kontynuować...");
        Console.ReadKey();
    }

    private static List<ZipItem> SearchByNameLinq(List<ZipItem> items, string name)
    {
        return items
            .Where(item => item.Name != null && item.Name.Contains(name, StringComparison.OrdinalIgnoreCase))
            .Concat(items.Where(item => item.IsDirectory)
                .SelectMany(item => SearchByNameLinq(item.Children, name)))
            .ToList();
    }

    private void RemoveFileFromArchive(string archivePath)
    {
        if (_zipItems.Count == 0)
        {
            AnsiConsole.MarkupLine("[yellow]Archiwum jest puste.[/]");
            return;
        }

        Display();

        string index = AnsiConsole.Ask<string>("[green]Podaj index pliku/folderu, który chcesz usunąć:[/]");

        var selectedZipItem = GetZipItemByIndex(_zipItems, index);

        if (selectedZipItem == null)
        {
            AnsiConsole.MarkupLine("[red]Niepoprawny index[/]");
            return;
        }

        if (!AnsiConsole.Confirm(
                $"Czy na pewno chcesz usunąć {(selectedZipItem.IsDirectory ? "folder" : "plik")} '{selectedZipItem.Name}'?"))
        {
            return;
        }

        AnsiConsole.Status()
            .Start("Usuwanie z archiwum...", ctx =>
            {
                var tempArchivePath = Path.GetTempFileName();

                using (var fs = new FileStream(archivePath, FileMode.Open, FileAccess.Read))
                using (var zipInputStream = new ZipInputStream(fs))
                using (var tempFs = new FileStream(tempArchivePath, FileMode.Create, FileAccess.Write))
                using (var zipOutputStream = new ZipOutputStream(tempFs))
                {
                    if (!string.IsNullOrEmpty(_password))
                    {
                        zipInputStream.Password = _password;
                        zipOutputStream.Password = _password;
                    }

                    ZipEntry entry;
                    while ((entry = zipInputStream.GetNextEntry()) != null)
                    {
                        if (selectedZipItem.FullPath != null &&
                            (entry.Name.Equals(selectedZipItem.FullPath, StringComparison.OrdinalIgnoreCase) ||
                             (selectedZipItem.IsDirectory && entry.Name.StartsWith(selectedZipItem.FullPath + "/",
                                 StringComparison.OrdinalIgnoreCase))))
                        {
                            continue;
                        }

                        zipOutputStream.PutNextEntry(new ZipEntry(entry.Name));
                        zipInputStream.CopyTo(zipOutputStream);
                        zipOutputStream.CloseEntry();
                    }
                }

                File.Delete(archivePath);
                File.Move(tempArchivePath, archivePath);
                
                _zipItems.Clear();
                MapArchive(archivePath);
            });

        AnsiConsole.MarkupLine("[green]Plik/folder został usunięty pomyślnie[/]");
    }

    private void AddFileToArchive(string archivePath)
    {
        if (_zipItems.Count == 0)
        {
            AnsiConsole.MarkupLine("[yellow]Najpierw musisz utworzyć lub otworzyć archiwum.[/]");
            return;
        }
        
        Display();

        string index =
            AnsiConsole.Ask<string>(
                "[green]Podaj index folderu, do którego chcesz dodać plik/folder[/] ([grey]pusty dla katalogu głównego[/]):");

        ZipItem? selectedZipItem = null;
        if (!string.IsNullOrEmpty(index))
        {
            selectedZipItem = GetZipItemByIndex(_zipItems, index);
            if (selectedZipItem == null)
            {
                AnsiConsole.MarkupLine("[red]Niepoprawny index[/]");
                return;
            }

            if (!selectedZipItem.IsDirectory)
            {
                AnsiConsole.MarkupLine("[red]Nie można dodać pliku do pliku[/]");
                return;
            }
        }

        var path = GetPathService.GetPath();
        if (string.IsNullOrEmpty(path) || (!Directory.Exists(path) && !File.Exists(path)))
        {
            AnsiConsole.MarkupLine("[red]Niepoprawna ścieżka pliku lub folderu[/]");
            return;
        }

        int compressionLevel = AnsiConsole.Prompt(
            new SelectionPrompt<int>()
                .Title("Wybierz poziom kompresji:")
                .AddChoices(new[] { 0, 1, 3, 5, 7, 9 })
                .UseConverter(level => level switch
                {
                    0 => "0 - Bez kompresji",
                    1 => "1 - Najszybsza (słaba kompresja)",
                    9 => "9 - Najlepsza (wolna)",
                    _ => $"{level} - Średnia"
                }));

        string? password = AnsiConsole.Prompt(
            new TextPrompt<string>("Wprowadź hasło dla dodawanego pliku (pozostaw puste, aby nie używać hasła):")
                .AllowEmpty());

        AnsiConsole.Status()
            .Start("Dodawanie pliku/folderu do archiwum...", ctx =>
            {
                var tempArchivePath = Path.GetTempFileName();

                using (var fs = new FileStream(archivePath, FileMode.Open, FileAccess.Read))
                using (var zipInputStream = new ZipInputStream(fs))
                using (var tempFs = new FileStream(tempArchivePath, FileMode.Create, FileAccess.Write))
                using (var zipOutputStream = new ZipOutputStream(tempFs))
                {
                    if (!string.IsNullOrEmpty(_password))
                    {
                        zipInputStream.Password = _password;
                    }

                    zipOutputStream.SetLevel(compressionLevel);

                    if (!string.IsNullOrEmpty(password))
                    {
                        zipOutputStream.Password = password;
                    }

                    ZipEntry entry;
                    while ((entry = zipInputStream.GetNextEntry()) != null)
                    {
                        zipOutputStream.PutNextEntry(new ZipEntry(entry.Name));
                        zipInputStream.CopyTo(zipOutputStream);
                        zipOutputStream.CloseEntry();
                    }

                    string targetPath = selectedZipItem?.FullPath ?? "";

                    if (Directory.Exists(path))
                    {
                        CreateArchiveService.AddDirectoryToArchive(zipOutputStream, path, targetPath);
                    }
                    else if (File.Exists(path))
                    {
                        CreateArchiveService.AddFileToArchive(zipOutputStream, path, targetPath);
                    }
                }

                File.Delete(archivePath);
                File.Move(tempArchivePath, archivePath);

                _zipItems.Clear();
                MapArchive(archivePath);
            });

        AnsiConsole.MarkupLine("[green]Plik/folder został dodany pomyślnie[/]");
    }

    private static ZipItem? GetZipItemByIndex(List<ZipItem> zipItems, string? index)
    {
        var parts = index?.Split('.');
        var current = zipItems;

        if (parts == null) return null;
        for (var i = 0; i < parts.Length; i++)
        {
            var part = parts[i];
            var partIndex = Convert.ToInt32(part);

            if (partIndex < 0 || partIndex >= current.Count) return null;

            var item = current[partIndex];

            if (i == parts.Length - 1)
            {
                return item;
            }

            if (!item.IsDirectory) return null;

            current = item.Children;
        }

        return null;
    }

    private void Display()
    {
        if (_zipItems.Count == 0)
        {
            AnsiConsole.MarkupLine("[yellow]Archiwum jest puste.[/]");
            return;
        }

        AnsiConsole.MarkupLine("[green]Zawartość archiwum:[/]");
        DisplayRecursive(_zipItems, 0, "");
        Console.WriteLine("\nNaciśnij dowolny klawisz, aby kontynuować...");
        Console.ReadKey();
    }

    private static void DisplayRecursive(List<ZipItem> items, int level, string parentIndex)
    {
        for (int index = 0; index < items.Count; index++)
        {
            var item = items[index];
            string currentIndex = string.IsNullOrEmpty(parentIndex) ? index.ToString() : $"{parentIndex}.{index}";
            string itemType = item.IsDirectory ? "[blue]📁[/]" : "[yellow]📄[/]";
            string lockIcon = item.IsPassword ? "🔒" : "";

            AnsiConsole.MarkupLine(
                $"{new string(' ', level * 2)}[grey]{currentIndex})[/] {itemType} {item.Name} {lockIcon}");

            if (item.IsDirectory && item.Children.Count > 0)
            {
                DisplayRecursive(item.Children, level + 1, currentIndex);
            }
        }
    }


    private void MapArchive(string archivePath)
    {
        var fileStream = File.OpenRead(archivePath);

        using var zipInputStream = new ZipInputStream(fileStream);

        Console.WriteLine("Podaj hasło do archiwum (jeśli jest wymagane):");
        var password = Console.ReadLine();

        if (!string.IsNullOrEmpty(password))
        {
            _password = password;
            zipInputStream.Password = password;
        }

        ZipEntry entry;

        while ((entry = zipInputStream.GetNextEntry()) != null)
        {
            var zipItem = new ZipItem
            {
                Name = entry.Name,
                IsDirectory = entry.IsDirectory,
                IsPassword = entry.IsCrypted,
                FullPath = entry.Name
            };

            AddEntryToModel(_zipItems, zipItem);
        }

        fileStream.Close();
    }

    private static void AddEntryToModel(List<ZipItem> items, ZipItem entry)
    {
        var parts = entry.Name?.Split('/');
        var current = items;

        if (parts == null) return;
        for (var i = 0; i < parts.Length; i++)
        {
            var part = parts[i];

            if (part == "") continue;

            var existing = current.FirstOrDefault(x => x.Name == part);

            if (existing == null)
            {
                var newItem = new ZipItem
                {
                    Name = part,
                    IsDirectory = (i < parts.Length - 1) || entry.IsDirectory,
                    FullPath = entry.FullPath
                };

                current.Add(newItem);
                current = newItem.Children;
            }
            else
            {
                current = existing.Children;
            }
        }
    }
}