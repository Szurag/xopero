using System.IO.Compression;
using ICSharpCode.SharpZipLib.Zip;
using Szyfrator.Models;

namespace Szyfrator.Services;

public class ArchiveExplorerService
{
    private readonly List<ZipItem> _zipItems = [];
    private string? _password = null;

    public void ArchiveExplorer()
    {
        var archivePath = GetPathService.GetPath("Podaj path archiwum, którym chcesz zarządzać");

        if (!File.Exists(archivePath))
        {
            Console.WriteLine("Archiwum nie istnieje");
            return;
        }

        if (Path.GetExtension(archivePath) != ".zip")
        {
            Console.WriteLine("To nie jest archiwum zip");
            return;
        }

        MapArchive(archivePath);

        Display();

        Console.WriteLine("Wybierz akcję");
        Console.WriteLine("1 - Dodaj plik/folder do archiwum");
        Console.WriteLine("2 - Usuń plik z archiwum");
        Console.WriteLine("3 - Wyszukaj po nazwie");

        var action = Console.ReadLine();

        switch (action)
        {
            case "1":
                AddFileToArchive(archivePath);
                break;
            case "2":
                RemoveFileFromArchive(archivePath);
                break;
            case "3":
                SearchByName();
                break;
        }
    }

    private void SearchByName()
    {
        Console.WriteLine("Podaj nazwę pliku / folderu, który chcesz wyszukać");
        var name = Console.ReadLine();

        if (string.IsNullOrEmpty(name))
        {
            Console.WriteLine("Nazwa nie może być pusta");
            return;
        }

        var results = SearchByNameLinq(_zipItems, name);

        if (results.Count == 0)
        {
            Console.WriteLine("Nie znaleziono pliku / folderu o podanej nazwie");
            return;
        }

        Console.WriteLine("Znalezione pliki / foldery:");
        foreach (var result in results)
        {
            Console.WriteLine(result.FullPath);
        }
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
        Console.WriteLine("Podaj index pliku / folderu, który chcesz usunąć");
        var index = Console.ReadLine();

        var selectedZipItem = GetZipItemByIndex(_zipItems, index);

        if (selectedZipItem == null)
        {
            Console.WriteLine("Niepoprawny index");
            return;
        }

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

            ZipEntry entry;
            while ((entry = zipInputStream.GetNextEntry()) != null)
            {
                if (selectedZipItem.FullPath != null && entry.Name.StartsWith(selectedZipItem.FullPath, StringComparison.OrdinalIgnoreCase))
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
    }

    private void AddFileToArchive(string archivePath)
    {
        Console.WriteLine("Podaj index folderu, na którego poziomie chcesz dodać plik / folder");
        var index = Console.ReadLine();

        var selectedZipItem = GetZipItemByIndex(_zipItems, index);

        if (selectedZipItem == null)
        {
            Console.WriteLine("Niepoprawny index");
            return;
        }

        if (!selectedZipItem.IsDirectory)
        {
            Console.WriteLine("Nie można dodać pliku do pliku");
            return;
        }

        var path = GetPathService.GetPath("Podaj path pliku / folderu, który chcesz dodać");
        var compressionLevel = CreateArchiveService.AskForCompressionLevel();
        var password = CreateArchiveService.AskForPassword();

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
            
            zipOutputStream.SetLevel(Convert.ToInt32(compressionLevel));

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

            if (Directory.Exists(path))
            {
                Console.WriteLine(selectedZipItem.FullPath);
                CreateArchiveService.AddDirectoryToArchive(zipOutputStream, path, selectedZipItem.FullPath ?? "");
            }
            else if (File.Exists(path))
            {
                Console.WriteLine(selectedZipItem.FullPath);

                CreateArchiveService.AddFileToArchive(zipOutputStream, path, selectedZipItem.FullPath ?? "");
            }
        }

        File.Delete(archivePath);
        File.Move(tempArchivePath, archivePath);
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
        DisplayRecursive(_zipItems, 0);
    }

    private static void DisplayRecursive(List<ZipItem> items, int level)
    {
        var index = 0;
        foreach (var item in items)
        {
            Console.WriteLine($"{new string(' ', level * 2)}{index}) {item.Name}");
            index++;
            if (item.IsDirectory)
            {
                DisplayRecursive(item.Children, level + 1);
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