// See https://aka.ms/new-console-template for more information
using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json;

var currentDirectory = Directory.GetCurrentDirectory();
var storesDirectory = Path.Combine(currentDirectory, "stores");
var salesTotalDir = Path.Combine(currentDirectory, "salesTotalDir");
Directory.CreateDirectory(salesTotalDir);
var salesFiles = FindFiles(storesDirectory);

var salesTotal = CalculateSalesTotal(salesFiles);
File.AppendAllText(Path.Combine(salesTotalDir, "totals.txt"), $"{salesTotal}{Environment.NewLine}");

// Generate the new sales summary report
GenerateSalesSummaryReport(salesFiles, salesTotalDir);

IEnumerable<string> FindFiles(string folderName)
{
    List<string> salesFiles = new List<string>();

    var foundFiles = Directory.EnumerateFiles(folderName, "*", SearchOption.AllDirectories);

    foreach (var file in foundFiles)
    {
        var fileName = Path.GetFileName(file);
        if (fileName == "sales.json")
        {
            salesFiles.Add(file);
        }
    }

    return salesFiles;
}

double CalculateSalesTotal(IEnumerable<string> salesFiles)
{
    double salesTotal = 0;

    // Loop over each file path in salesFiles
    foreach (var file in salesFiles)
    {
        // Read the contents of the file
        string salesJson = File.ReadAllText(file);

        // Parse the contents as JSON
        SalesData? data = JsonConvert.DeserializeObject<SalesData?>(salesJson);

        // Add the amount found in the Total field to the salesTotal variable
        salesTotal += data?.Total ?? 0;
    }

    return salesTotal;
}

void GenerateSalesSummaryReport(IEnumerable<string> salesFiles, string outputDirectory)
{
    // Dictionary to hold each file's name and its sales total
    var fileTotals = new Dictionary<string, double>();
    double grandTotal = 0;

    foreach (var file in salesFiles)
    {
        string salesJson = File.ReadAllText(file);
        SalesData? data = JsonConvert.DeserializeObject<SalesData?>(salesJson);
        double fileTotal = data?.Total ?? 0;

        // Use just the file name (not full path) for readability in the report
        string relativePath = Path.GetRelativePath(storesDirectory, file);
        fileTotals[relativePath] = fileTotal;

        grandTotal += fileTotal;
    }

    // Build the report text
    var report = new System.Text.StringBuilder();
    report.AppendLine("Sales Summary");
    report.AppendLine("----------------------------");
    report.AppendLine($" Total Sales: {grandTotal:C}");
    report.AppendLine();
    report.AppendLine(" Details:");

    foreach (var entry in fileTotals)
    {
        report.AppendLine($"  {entry.Key}: {entry.Value:C}");
    }

    // Write the report to a file
    var reportPath = Path.Combine(outputDirectory, "salesSummary.txt");
    File.WriteAllText(reportPath, report.ToString());
}

record SalesData(double Total);