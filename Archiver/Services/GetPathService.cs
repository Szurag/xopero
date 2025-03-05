namespace Archiver.Services;

public static class GetPathService
{
    public static string GetPath(string message)
    {
        Console.WriteLine(message);

        var path = Console.ReadLine();

        if (path != null)
        {
            return path;
        }
        
        Console.WriteLine("Path jest niepoprawny");
        return GetPath(message);
    }
}