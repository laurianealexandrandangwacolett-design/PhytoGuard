using PhytoGuard.Models;

namespace PhytoGuard.Services;

/// <summary>État partagé du parcours de diagnostic (Culture → Photo → Symptômes → Résultat).</summary>
public class DiagnosticSession
{
    public int ParcelId { get; set; }
    public string ParcelName { get; set; } = "";
    public string Crop { get; set; } = "";
    public string? PhotoPath { get; set; }
    public List<string> Symptoms { get; set; } = new();
    public string Duration { get; set; } = "";
    public List<DiagnosisResult> Results { get; set; } = new();

    public void Reset()
    {
        ParcelId = 0; ParcelName = ""; Crop = ""; PhotoPath = null;
        Symptoms = new(); Duration = ""; Results = new();
    }
}
