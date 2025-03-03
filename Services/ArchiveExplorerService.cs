using System.IO.Compression;
using ICSharpCode.SharpZipLib.Zip;
using Szyfrator.Models;

namespace Szyfrator.Services;

public class ArchiveExplorerService
{
    private List<ZipItem> _zipItems = [];

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

        var fileStream = File.OpenRead(archivePath);
        
        using var zipInputStream = new ZipInputStream(fileStream);

        ZipEntry entry;

        while ((entry = zipInputStream.GetNextEntry()) != null)
        {
            var zipItem = new ZipItem
            {
                Name = entry.Name,
                IsDirectory = entry.IsDirectory,
                IsPassword = entry.IsCrypted
            };

            AddEntryToModel(_zipItems, zipItem);
        }
    }
    
    private static void AddEntryToModel(List<ZipItem> items, ZipItem entry)
    {
        var parts = entry.Name?.Split('/');
        var current = items;

        if (parts == null) return;
        for (var i = 0; i < parts.Length; i++)
        {
            var part = parts[i];
            var existing = current.FirstOrDefault(x => x.Name == part);

            if (existing == null)
            {
                var newItem = new ZipItem
                {
                    Name = part,
                    IsDirectory = (i < parts.Length - 1) || entry.IsDirectory
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