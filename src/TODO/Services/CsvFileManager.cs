using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using CsvHelper;
using CsvHelper.Configuration;
using TODO.Models;

namespace TODO.Services;

public class CsvFileManager
{
    public void SaveToFile(string filePath, List<TodoItem> list)
    {
        using (var writer = new StreamWriter(filePath))
        using (var csv = new CsvWriter(writer,
                   new CsvConfiguration(CultureInfo.InvariantCulture) { Delimiter = "," }))
        {
            csv.WriteRecords(list);
        }
    }

    public List<TodoItem> LoadFromFile(string filePath)
    {
        List<TodoItem> list;

        using (var reader = new StreamReader(filePath))
        using (var csv = new CsvReader(reader,
                   new CsvConfiguration(CultureInfo.InvariantCulture) { Delimiter = "," }))
        {
            list = csv.GetRecords<TodoItem>().ToList();
        }

        return list;
    }
}