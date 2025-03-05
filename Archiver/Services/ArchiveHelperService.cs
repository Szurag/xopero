using Spectre.Console;

namespace Archiver.Services;

public static class ArchiveHelperService
{
    public static int GetCompressionLevel()
    {
        return AnsiConsole.Prompt(
            new TextPrompt<int>("Podaj poziom kompresji (0-9):")
                .PromptStyle("green")
                .ValidationErrorMessage("[red]Niepoprawny poziom kompresji[/]")
                .Validate(level => level is >= 0 and <= 9));
    }
    
    public static string? GetPassword()
    {
        return AnsiConsole.Prompt(
            new TextPrompt<string?>("Podaj hasło (opcjonalnie):")
                .PromptStyle("green")
                .AllowEmpty());
    }
}