using System;
using System.IO;

namespace TODO.Configuration;

public class AppSettings
{
    public const string DefaultTodoCsvFileName = "todo.csv";
    public const string DefaultArchiveCsvFileName = "archive.csv";

    public static readonly string DefaultTodoCsvPath = Path.Combine(Environment.CurrentDirectory, DefaultTodoCsvFileName);
    public static readonly string DefaultArchiveCsvPath = Path.Combine(Environment.CurrentDirectory, DefaultArchiveCsvFileName);

    public string TodoCsvPath { get; set; } = DefaultTodoCsvPath;
    public string ArchiveCsvPath { get; set; } = DefaultArchiveCsvPath;

    public bool DarkModeEnabled { get; set; }
}