using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

class TraderValidator
{
    static void Main(string[] args)
    {
        Console.WriteLine("=== DrJones Trader File Validator ===\n");

        if (args.Length != 1)
        {
            Console.WriteLine("Usage: Drag a .txt file onto this .exe to validate it.");
            Pause();
            return;
        }

        string filePath = args[0];

        if (!File.Exists(filePath))
        {
            Console.WriteLine($"Error: File '{filePath}' not found.");
            Pause();
            return;
        }

        var lines = File.ReadAllLines(filePath);
        var seenItems = new HashSet<string>();
        var duplicates = new List<(int lineNumber, string item)>();
        var invalidLines = new List<(int lineNumber, string content, int commaCount)>();

        for (int i = 0; i < lines.Length; i++)
        {
            string line = lines[i].Trim();

            if (string.IsNullOrWhiteSpace(line) || line.StartsWith("//"))
                continue;

            if (line.StartsWith("<Trader>") || line.StartsWith("<Category>") ||
                line.StartsWith("<Currency>") || (line.StartsWith("<") && line.EndsWith(">")))
                continue;

            string codeOnly = line.Split(new[] { "//" }, StringSplitOptions.None)[0].Trim();

            if (codeOnly.Contains(","))
            {
                int commaCount = codeOnly.Count(c => c == ',');
                if (commaCount != 3)
                {
                    invalidLines.Add((i + 1, lines[i], commaCount));
                }

                string classname = codeOnly.Split(',')[0].Trim();
                if (seenItems.Contains(classname))
                {
                    duplicates.Add((i + 1, classname));
                }
                else
                {
                    seenItems.Add(classname);
                }
            }
        }

        Console.WriteLine();

        if (invalidLines.Any() || duplicates.Any())
        {
            Console.WriteLine("❌ Validation Failed.\n");

            if (invalidLines.Any())
            {
                Console.WriteLine("⚠️ Lines with incorrect number of commas (should be 3):");
                foreach (var (lineNumber, content, count) in invalidLines)
                {
                    Console.WriteLine($"  Line {lineNumber}: {content} (commas: {count})");
                }
                Console.WriteLine();
            }

            if (duplicates.Any())
            {
                Console.WriteLine("⚠️ Duplicate item classnames found:");
                foreach (var (lineNumber, item) in duplicates)
                {
                    Console.WriteLine($"  Line {lineNumber}: {item}");
                }
            }
        }
        else
        {
            Console.WriteLine("✅ Validation passed: No issues found.");
        }

        Pause();
    }

    static void Pause()
    {
        Console.WriteLine("\nPress any key to exit...");
        Console.ReadKey();
    }
}
