using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using Szyfrator.Gui.Models;

namespace Szyfrator.Gui.Views;

public partial class MainWindow : Window
{
    private List<FileArchive> _files = [];

    public MainWindow()
    {
        InitializeComponent();
    }

    private void SelectDirectory_OnClick(object? sender, RoutedEventArgs e)
    {
        var options = new FolderPickerOpenOptions()
        {
            Title = "Wybierz folder",
            AllowMultiple = false
        };

        var dialog = new Window().StorageProvider.OpenFolderPickerAsync(options);

        if (dialog.Result.Count <= 0)
        {
            return;
        }

        var directory = dialog.Result[0];
        var files = Directory.GetFiles(directory.Path.AbsolutePath);

        _files.Clear();
        foreach (var file in files)
        {
            if (Path.GetExtension(file) != ".zip") continue;

            _files.Add(new FileArchive
            {
                Name = Path.GetFileName(file),
                Path = file
            });
        }

        Archives.ItemsSource = _files.Select(x => x.Name);
    }

    private void Archives_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (Archives.SelectedItem is string selectedArchive)
        {
            Console.WriteLine($"Wybrano archiwum: {selectedArchive}");
        }
    }
}