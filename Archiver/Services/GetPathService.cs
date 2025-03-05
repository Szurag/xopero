using Spectre.Console;

namespace Archiver.Services;

public static class GetPathService
{
    public static string GetPath()
    {
        return AnsiConsole.Prompt(
            new TextPrompt<string>("Podaj ścieżkę do folderu lub pliku:")
                .PromptStyle("green")
                .ValidationErrorMessage("[red]Path jest niepoprawny[/]")
                .Validate(ValidatePath));
    }
    
    public static string GetArchivePath()
    {
        return AnsiConsole.Prompt(
            new TextPrompt<string>("Podaj ścieżkę do odczytu archiwum:")
                .PromptStyle("green")
                .ValidationErrorMessage("[red]{0}[/]")
                .Validate(ValidatePathArchivePath));
    }

    private static ValidationResult ValidatePathArchivePath(string arg)
    {
        if (string.IsNullOrEmpty(arg))
        {
            return ValidationResult.Error("Ścieżka do archiwum nie istnieje");
        }

        if (!arg.Contains(".zip"))
        {
            return ValidationResult.Error("Ścieżka do archiwum musi zawierać rozszerzenie .zip");
        }

        return File.Exists(arg) ? ValidationResult.Success() : ValidationResult.Error("Ściezka do archiwum nie istnieje");
    }

    public static string GetNewArchivePath()
    {
        return AnsiConsole.Prompt(
            new TextPrompt<string>("Podaj ścieżkę do zapisu archiwum:")
                .PromptStyle("green")
                .ValidationErrorMessage("[red]{0}[/]")
                .Validate(ValidatePathNewArchivePath));
    }

    private static ValidationResult ValidatePathNewArchivePath(string path)
    {
        if (string.IsNullOrEmpty(path))
        {
            return ValidationResult.Error("Ścieżka do archiwum nie istnieje");
        }

        if (!path.Contains(".zip"))
        {
            return ValidationResult.Error("Ścieżka do archiwum musi zawierać rozszerzenie .zip");
        }

        return File.Exists(path) ? ValidationResult.Error("Ściezka do archiwum już istnieje") : ValidationResult.Success();
    }

    private static ValidationResult ValidatePath(string path)
    {
        if (string.IsNullOrEmpty(path))
        {
            return ValidationResult.Error("Ściezka jest pusta");
        }
        
        if (!(Directory.Exists(path) || File.Exists(path)))
        {
            return ValidationResult.Error("Ściezka nie istnieje");
        }
        
        return ValidationResult.Success();
    }
}