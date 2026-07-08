# Submission Notes

## Part 1: Create a web API with ASP.NET Core controllers

### Existing content (default Pizzas list)

- id: 1, name: "Classic Italian", isGlutenFree: false
- id: 2, name: "Veggie", isGlutenFree: true

### Additional record added

- id: 3, name: "Pepperoni Feast", isGlutenFree: false

### Verified via GET /pizza — 200 OK

```json
[
  { "id": 1, "name": "Classic Italian", "isGlutenFree": false },
  { "id": 2, "name": "Veggie", "isGlutenFree": true },
  { "id": 3, "name": "Pepperoni Feast", "isGlutenFree": false }
]
```

### CRUD verification with status codes

**GET /pizza/3** → 200 OK

```json
{ "id": 3, "name": "Pepperoni Feast", "isGlutenFree": false }
```

**POST /pizza** → 201 Created

```json
{ "id": 4, "name": "Hawaii", "isGlutenFree": false }
```

Location header: http://localhost:5175/Pizza/4

**PUT /pizza/3** → 204 No Content

**DELETE /pizza/3** → 204 No Content

---

## Part 2: Sales Summary Report Function

```csharp
void GenerateSalesSummaryReport(IEnumerable<string> salesFiles, string outputDirectory)
{
    // Dictionary to hold each file's relative path and its sales total
    var fileTotals = new Dictionary<string, double>();
    double grandTotal = 0;

    foreach (var file in salesFiles)
    {
        string salesJson = File.ReadAllText(file);
        SalesData? data = JsonConvert.DeserializeObject<SalesData?>(salesJson);
        double fileTotal = data?.Total ?? 0;

        // Use path relative to the stores directory so files with the
        // same name in different store folders don't overwrite each other
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
```

### Sample output (salesSummary.txt)

```
Sales Summary
----------------------------
 Total Sales: $2,012.20

 Details:
  sales.json: $88.88
  201\sales.json: $501.22
  202\sales.json: $1,234.22
  203\sales.json: $99.00
  204\sales.json: $88.88
```
