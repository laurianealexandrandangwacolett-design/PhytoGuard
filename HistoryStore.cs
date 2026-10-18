using System.Globalization;
using System.Text.Json;
using Microsoft.Maui.Storage;

namespace PhytoGuard;

public class DiagnosticRecord
{
    public DateTime Date { get; set; }
    public string ParcelName { get; set; } = "";
    public string Crop { get; set; } = "";
    public string Title { get; set; } = "";
    public string Details { get; set; } = "";
    public string PhotoPath { get; set; } = "";

    public string DateLabel =>
        Date.ToString("dd MMMM yyyy 'à' HH:mm",
            CultureInfo.GetCultureInfo("fr-FR"));
}

public static class HistoryStore
{
    private const string StorageKey = "DiagnosticHistory";

    public static List<DiagnosticRecord> GetAll()
    {
        string json = Preferences.Get(StorageKey, "");

        if (string.IsNullOrWhiteSpace(json))
            return new List<DiagnosticRecord>();

        try
        {
            return JsonSerializer.Deserialize<List<DiagnosticRecord>>(json)
                   ?? new List<DiagnosticRecord>();
        }
        catch
        {
            return new List<DiagnosticRecord>();
        }
    }

    public static void Add(DiagnosticRecord record)
    {
        List<DiagnosticRecord> records = GetAll();
        records.Insert(0, record);

        // Garde les 100 observations les plus récentes.
        if (records.Count > 100)
            records = records.Take(100).ToList();

        Preferences.Set(StorageKey, JsonSerializer.Serialize(records));
    }
}