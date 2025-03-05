namespace Archiver.Models;

public class ZipItem
{
    public string? Name { get; set; } = null;
    public bool IsDirectory { get; set; }
    public bool IsPassword { get; set; } = false;
    public List<ZipItem> Children { get; set; } = [];
    public string? FullPath { get; set; } = null;
}