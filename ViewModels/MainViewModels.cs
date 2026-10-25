using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PhytoGuard.Models;
using PhytoGuard.Services;

namespace PhytoGuard.ViewModels;

public partial class HomeViewModel : BaseViewModel
{
    private readonly AuthService _auth;
    private readonly DatabaseService _db;
    private readonly SyncService _sync;
    private readonly DiagnosticSession _session;

    public HomeViewModel(AuthService auth, DatabaseService db, SyncService sync, DiagnosticSession session)
    {
        _auth = auth; _db = db; _sync = sync; _session = session;
        _sync.StatusChanged += () => MainThread.BeginInvokeOnMainThread(async () => await Load());
    }

    public ObservableCollection<Parcel> Parcels { get; } = new();

    [ObservableProperty] private string greeting = "Bonjour 👋";
    [ObservableProperty] private string syncBadge = "À JOUR";
    [ObservableProperty] private string syncText = "";
    [ObservableProperty] private string networkBadge = "";
    [ObservableProperty] private string gpsText = "Aucune position enregistrée pour le moment.";
    [ObservableProperty] private bool hasParcels;

    [RelayCommand]
    public async Task Load()
    {
        if (_auth.CurrentPhone is not { } phone) return;
        var name = _auth.CurrentName.Split(' ', StringSplitOptions.RemoveEmptyEntries).FirstOrDefault();
        Greeting = string.IsNullOrEmpty(name) ? "Bonjour 👋" : $"Bonjour, {name} 👋";

        var parcels = await _db.GetParcelsAsync(phone);
        var diags = await _db.GetDiagnosticsAsync(phone);
        foreach (var p in parcels)
        {
            var mine = diags.Where(d => d.ParcelId == p.Id).ToList();
            p.LastCheckText = mine.Count == 0 ? "Aucun contrôle" : $"Dernier contrôle : {Helpers.Ago(mine[0].CreatedAt)}";
            var toReview = mine.Count(d => !d.Reviewed);
            p.ReviewText = toReview == 0 ? "" : $"{toReview} observation{(toReview > 1 ? "s" : "")} à revoir";
        }
        Parcels.Clear();
        foreach (var p in parcels) Parcels.Add(p);
        HasParcels = parcels.Count > 0;

        var pending = await _sync.PendingAsync();
        SyncBadge = pending == 0 ? "À JOUR" : $"{pending} EN ATTENTE";
        NetworkBadge = _sync.IsOnline ? "En ligne" : "Hors ligne";
        SyncText = _sync.IsOnline
            ? (pending == 0 ? "Toutes vos données sont envoyées." : "Envoi des données en cours…")
            : "Les données seront envoyées au retour du réseau.";
        GpsText = parcels.Count == 0
            ? "Ajoutez une parcelle pour enregistrer sa position GPS."
            : "Coordonnées gardées sur le téléphone. Elles seront synchronisées au retour du réseau.";
    }

    [RelayCommand] private Task AddParcel() => Shell.Current.GoToAsync("newparcel");

    [RelayCommand]
    private Task NewDiagnostic()
    {
        _session.Reset();
        return Shell.Current.GoToAsync("diagculture");
    }

    [RelayCommand]
    private async Task Logout()
    {
        var ok = await Shell.Current.DisplayAlert("Déconnexion", "Voulez-vous vous déconnecter ?", "Se déconnecter", "Annuler");
        if (!ok) return;
        _auth.Logout();
        Parcels.Clear();
        _session.Reset();
        await Shell.Current.GoToAsync("//welcome");
    }
}

public partial class ParcelsViewModel : BaseViewModel
{
    private readonly AuthService _auth;
    private readonly DatabaseService _db;
    public ParcelsViewModel(AuthService auth, DatabaseService db) { _auth = auth; _db = db; }

    public ObservableCollection<Parcel> Parcels { get; } = new();
    [ObservableProperty] private bool isEmpty;

    [RelayCommand]
    public async Task Load()
    {
        if (_auth.CurrentPhone is not { } phone) return;
        var list = await _db.GetParcelsAsync(phone);
        Parcels.Clear();
        foreach (var p in list) Parcels.Add(p);
        IsEmpty = list.Count == 0;
    }

    [RelayCommand] private Task Add() => Shell.Current.GoToAsync("newparcel");
}

public partial class NewParcelViewModel : BaseViewModel
{
    private readonly AuthService _auth;
    private readonly DatabaseService _db;
    private readonly LocationService _location;
    private readonly SyncService _sync;
    private double? _lat, _lon;

    public NewParcelViewModel(AuthService auth, DatabaseService db, LocationService location, SyncService sync)
    { _auth = auth; _db = db; _location = location; _sync = sync; }

    public string[] Crops => DiagnosisService.Crops;
    [ObservableProperty] private string coordsText = "Position non définie";
    [ObservableProperty] private string parcelName = "";
    [ObservableProperty] private string? crop;

    [RelayCommand]
    private async Task Locate()
    {
        ErrorMessage = "";
        IsBusy = true;
        var loc = await _location.GetCurrentAsync();
        IsBusy = false;
        if (loc == null)
        {
            ErrorMessage = "Position introuvable. Activez le GPS et autorisez la localisation.";
            return;
        }
        _lat = loc.Latitude; _lon = loc.Longitude;
        CoordsText = $"{loc.Latitude:0.000000}°, {loc.Longitude:0.000000}°";
    }

    [RelayCommand]
    private async Task Save()
    {
        ErrorMessage = "";
        if (_auth.CurrentPhone is not { } phone) return;
        if (_lat == null || _lon == null) { ErrorMessage = "Localisez d'abord la parcelle."; return; }
        if (string.IsNullOrWhiteSpace(ParcelName)) { ErrorMessage = "Donnez un nom à la parcelle."; return; }
        if (string.IsNullOrEmpty(Crop)) { ErrorMessage = "Choisissez la culture."; return; }

        await _db.AddParcelAsync(new Parcel
        {
            OwnerPhone = phone, Name = ParcelName.Trim(), Crop = Crop!,
            Latitude = _lat.Value, Longitude = _lon.Value
        });
        _ = _sync.SyncAsync();
        await Helpers.Alert("Parcelle", "Parcelle enregistrée sur le téléphone.");
        await Shell.Current.GoToAsync("..");
    }
}

public partial class HistoryViewModel : BaseViewModel
{
    private readonly AuthService _auth;
    private readonly DatabaseService _db;
    public HistoryViewModel(AuthService auth, DatabaseService db) { _auth = auth; _db = db; }

    public ObservableCollection<Parcel> Parcels { get; } = new();
    public ObservableCollection<DiagnosticRecord> Records { get; } = new();

    [ObservableProperty] private string parcelsTitle = "";
    [ObservableProperty] private string parcelsHint = "";
    [ObservableProperty] private string recordsHint = "";
    [ObservableProperty] private bool hasParcels;
    [ObservableProperty] private bool hasRecords;

    [RelayCommand]
    public async Task Load()
    {
        if (_auth.CurrentPhone is not { } phone) return;
        var parcels = await _db.GetParcelsAsync(phone);
        var recs = await _db.GetDiagnosticsAsync(phone);
        foreach (var p in parcels)
        {
            var last = recs.FirstOrDefault(r => r.ParcelId == p.Id);
            p.LastCheckText = last == null ? "Aucun contrôle" : $"Dernier contrôle : {Helpers.Ago(last.CreatedAt)}";
        }
        Parcels.Clear(); foreach (var p in parcels) Parcels.Add(p);
        Records.Clear(); foreach (var r in recs) Records.Add(r);
        HasParcels = parcels.Count > 0;
        HasRecords = recs.Count > 0;
        ParcelsTitle = HasParcels ? $"Mes parcelles ({parcels.Count})" : "Aucune parcelle enregistrée";
        ParcelsHint = HasParcels ? "" : "Enregistrez une culture pour cette parcelle.";
        RecordsHint = HasParcels
            ? (HasRecords ? "" : "Aucune observation. Lancez un nouveau diagnostic.")
            : "Enregistrez d'abord une parcelle et sa culture.";
    }

    [RelayCommand]
    private async Task Open(DiagnosticRecord rec)
    {
        if (!rec.Reviewed) { rec.Reviewed = true; await _db.UpdateDiagnosticAsync(rec); }
        if (!string.IsNullOrEmpty(rec.DiseaseId))
            await Shell.Current.GoToAsync($"diseasedetail?id={rec.DiseaseId}");
        else
            await Helpers.Alert("Observation", "Aucune maladie identifiée pour cette observation. Consultez un expert.");
    }
}

public partial class AdviceViewModel : BaseViewModel
{
    private readonly DiagnosisService _diag;
    private readonly AuthService _auth;
    public AdviceViewModel(DiagnosisService diag, AuthService auth) { _diag = diag; _auth = auth; }

    public ObservableCollection<Disease> Diseases { get; } = new();
    public string[] Tips => AppData.GeneralTips;

    [RelayCommand]
    public async Task Load()
    {
        if (Diseases.Count > 0) return;
        foreach (var d in await _diag.GetAllAsync()) Diseases.Add(d);
    }

    [RelayCommand] private Task OpenDisease(Disease d) => Shell.Current.GoToAsync($"diseasedetail?id={d.Id}");
    [RelayCommand] private Task Experts() => Shell.Current.GoToAsync("experts");

    [RelayCommand]
    private async Task Logout()
    {
        _auth.Logout();
        await Shell.Current.GoToAsync("//welcome");
    }
}