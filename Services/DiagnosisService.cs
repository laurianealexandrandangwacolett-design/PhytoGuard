using System.Text.Json;
using PhytoGuard.Models;

namespace PhytoGuard.Services;

/// <summary>Hook pour un modèle d'IA (ONNX). Retourne une probabilité par identifiant de maladie.</summary>
public interface IImageClassifier
{
    bool IsAvailable { get; }
    Task<IReadOnlyDictionary<string, float>> ClassifyAsync(string photoPath);
}

public class NullImageClassifier : IImageClassifier
{
    public bool IsAvailable => false;
    public Task<IReadOnlyDictionary<string, float>> ClassifyAsync(string photoPath) =>
        Task.FromResult<IReadOnlyDictionary<string, float>>(new Dictionary<string, float>());
}

public class DiagnosisService
{
    public static readonly string[] Crops = { "Tomate", "Maïs", "Manioc", "Cacao", "Banane plantain" };
    public static readonly string[] Durations = { "Aujourd'hui", "Depuis quelques jours", "Depuis une semaine ou plus" };

    public static readonly List<SymptomDef> Symptoms = new()
    {
        new("taches_jaunes", "Taches jaunes"),
        new("feuilles_seches", "Feuilles sèches"),
        new("poudre_blanche", "Poudre blanche"),
        new("taches_brunes", "Taches brunes"),
        new("fletrissement", "Plante qui se fane"),
        new("pourriture_fruit", "Fruits qui pourrissent"),
        new("feuilles_enroulees", "Feuilles déformées / enroulées"),
        new("trous_feuilles", "Trous dans les feuilles")
    };

    private readonly IImageClassifier _classifier;
    private List<Disease>? _diseases;

    public DiagnosisService(IImageClassifier classifier) => _classifier = classifier;

    public async Task<List<Disease>> GetAllAsync()
    {
        if (_diseases != null) return _diseases;
        using var stream = await FileSystem.OpenAppPackageFileAsync("diseases.json");
        _diseases = await JsonSerializer.DeserializeAsync<List<Disease>>(stream,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new();
        return _diseases;
    }

    public async Task<Disease?> GetByIdAsync(string id) =>
        (await GetAllAsync()).FirstOrDefault(d => d.Id == id);

    public static string SymptomLabel(string key) =>
        Symptoms.FirstOrDefault(s => s.Key == key)?.Label ?? key;

    /// <summary>Diagnostic hors ligne : score selon les symptômes cochés (+ IA ONNX si disponible).</summary>
    public async Task<List<DiagnosisResult>> DiagnoseAsync(string crop, IReadOnlyCollection<string> symptoms, string? photoPath)
    {
        var diseases = (await GetAllAsync()).Where(d => d.Crop == crop).ToList();
        IReadOnlyDictionary<string, float> ai = new Dictionary<string, float>();
        if (_classifier.IsAvailable && !string.IsNullOrEmpty(photoPath))
        {
            try { ai = await _classifier.ClassifyAsync(photoPath); } catch { /* repli sur les règles */ }
        }

        var results = new List<DiagnosisResult>();
        foreach (var d in diseases)
        {
            var matched = d.Symptoms.Count(symptoms.Contains);
            double rule = 0;
            if (matched > 0 && symptoms.Count > 0)
                rule = 0.6 * matched / d.Symptoms.Count + 0.4 * matched / symptoms.Count;
            double score = rule;
            if (ai.TryGetValue(d.Id, out var p)) score = rule > 0 ? 0.5 * rule + 0.5 * p : 0.5 * p;
            if (score > 0.05) results.Add(new DiagnosisResult { Disease = d, Confidence = Math.Round(Math.Min(score, 0.98) * 100) });
        }
        return results.OrderByDescending(r => r.Confidence).Take(3).ToList();
    }
}
