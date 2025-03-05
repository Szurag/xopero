using CSMyAdmin.Data;
using Microsoft.EntityFrameworkCore;

namespace CSMyAdmin;

public class App(ApplicationDbContext _context)
{
    public async Task Run()
    {
        Console.WriteLine("Database Console");
        Console.WriteLine("Type SQL queries or 'exit' to quit");

        while (true)
        {
            Console.Write("> ");
            var sql = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(sql)) continue;
            if (sql.Equals("exit", StringComparison.OrdinalIgnoreCase)) break;

            try
            {
                if (IsSelectQuery(sql))
                {
                    await ExecuteQueryAsync(sql);
                }
                else
                {
                    await ExecuteNonQueryAsync(sql);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
    }

    private async Task ExecuteQueryAsync(string sql)
    {
        // Execute raw SQL query that returns results
        var result = await _context.Database.SqlQueryRaw<dynamic>(sql).ToListAsync();
        
        // For older EF Core versions:
        // FormattableString sqlCommand = $"{sql}";
        // var result = await _context.Database.SqlQuery<dynamic>(sqlCommand).ToListAsync();

        if (result.Count == 0)
        {
            Console.WriteLine("Query executed successfully. No results returned.");
            return;
        }

        // Display column names
        var properties = result[0].GetType().GetProperties();
        foreach (var prop in properties)
        {
            Console.Write($"{prop.Name,-20}");
        }
        Console.WriteLine();

        // Display data
        foreach (var row in result)
        {
            foreach (var prop in properties)
            {
                var value = prop.GetValue(row)?.ToString() ?? "NULL";
                Console.Write($"{value,-20}");
            }
            Console.WriteLine();
        }

        Console.WriteLine($"{result.Count} row(s) returned");
    }

    private async Task ExecuteNonQueryAsync(string sql)
    {
        int rowsAffected = await _context.Database.ExecuteSqlRawAsync(sql);
        Console.WriteLine($"Command executed successfully. {rowsAffected} row(s) affected.");
    }

    private bool IsSelectQuery(string sql)
    {
        var trimmedSql = sql.TrimStart().ToLower();
        return trimmedSql.StartsWith("select") || trimmedSql.StartsWith("with");
    }
}