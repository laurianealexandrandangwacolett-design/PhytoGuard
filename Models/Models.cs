using SQLite;

namespace PhytoGuard.Models;

public class UserAccount
{
    [PrimaryKey, AutoIncrement] public int Id { get; set; }
    public string FullName { get; set; } = "";
    [Indexed(Unique = true)] public string Phone { get; set; } = "";
    public string PasswordHash { get; set; } = "";
    public string Salt { get; set; } = "";
}

public class Parcel
{
    [PrimaryKey, AutoIncrement] public int Id { get; set; }
    [Indexed] public string OwnerPhone { get; set; } = "";
    public string Name { get; set; } = "";
    public string Crop { get; set; } = "";
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public bool Synced { get; set; }

    [Ignore] public string LastCheckText { get; set; } = "Aucun contrôle";
    [Ignore] public string ReviewText { get; set; } = "";
    [Ignore] public bool HasReview => !string.IsNullOrEmpty(ReviewText);
    [Ignore] public string CoordsText => $"{Latitude:0.000000}°, {Longitude:0.000000}°";
}

public class DiagnosticRecord
{
    [PrimaryKey, AutoIncrement] public int Id { get; set; }
    [Indexed] public string OwnerPhone { get; set; } = "";
    public int ParcelId { get; set; }
    public string ParcelName { get; set; } = "";
    public string Crop { get; set; } = "";
    public string PhotoPath { get; set; } = "";
    public string Symptoms { get; set; } = "";
    public string Duration { get; set; } = "";
    public string DiseaseId { get; set; } = "";
    public string DiseaseName { get; set; } = "";
    public double Confidence { get; set; }
    public bool Reviewed { get; set; }
    public bool Synced { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    [Ignore] public string DateText => CreatedAt.ToString("dd/MM/yyyy HH:mm");
    [Ignore] public string Summary => $"{Crop} · {(string.IsNullOrEmpty(ParcelName) ? "Sans parcelle" : ParcelName)}";
    [Ignore] public string ConfidenceText => Confidence > 0 ? $"{Confidence:0} %" : "—";
    [Ignore] public bool HasPhoto => !string.IsNullOrEmpty(PhotoPath) && File.Exists(PhotoPath);
}

public class Appointment
{
    [PrimaryKey, AutoIncrement] public int Id { get; set; }
    [Indexed] public string OwnerPhone { get; set; } = "";
    public string ExpertName { get; set; } = "";
    public string Slot { get; set; } = "";
    public string PaymentMethod { get; set; } = "";
    public int PriceFcfa { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public bool Synced { get; set; }
}

public class Disease
{
    public string Id { get; set; } = "";
    public string Name { get; set; } = "";
    public string Crop { get; set; } = "";
    public string Severity { get; set; } = "";
    public string Description { get; set; } = "";
    public List<string> Symptoms { get; set; } = new();
    public string Causes { get; set; } = "";
    public List<string> Treatments { get; set; } = new();
    public List<string> Prevention { get; set; } = new();

    public string TreatmentsText => string.Join("\n", Treatments.Select(t => "• " + t));
    public string PreventionText => string.Join("\n", Prevention.Select(t => "• " + t));
    public string SubTitle => $"{Crop} · Gravité {Severity.ToLowerInvariant()}";
}

public class DiagnosisResult
{
    public Disease Disease { get; set; } = new();
    public double Confidence { get; set; }
    public string ConfidenceText => $"{Confidence:0} %";
}

public record SymptomDef(string Key, string Label);

public record ExpertDef(string Name, string Speciality, string Method, string Language, int Minutes)
{
    public string Subtitle => $"Vérifié · {Method} · {Language} · {Minutes} min";
    public string Title => $"{Name} · {Speciality}";
}
