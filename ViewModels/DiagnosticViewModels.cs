using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PhytoGuard.Models;
using PhytoGuard.Services;

namespace PhytoGuard.ViewModels;

public class BookingSession
{
    public string ExpertName { get; set; } = "";
    public string Slot { get; set; } = "";
}

// ---------- Étape 1 : culture ----------
public partial class DiagCultureViewModel : BaseViewModel
{
    private readonly AuthService _auth;
    private readonly DatabaseService _db;
    private readonly DiagnosticSession _session;
    public DiagCultureViewModel(AuthService auth, DatabaseService db, DiagnosticSession session)
    { _auth = auth; _db = db; _session = session; }

    public string[] Crops => DiagnosisService.Crops;
    public ObservableCollection<Parcel> Parcels { get; } = new();
    [ObservableProperty] private Parcel? selectedParcel;
    [ObservableProperty] private string? crop;

    partial void OnSelectedParcelChanged(Parcel? value)
    {
        if (value != null && DiagnosisService.Crops.Contains(value.Crop)) Crop = value.Crop;
    }

    [RelayCommand]
    public async Task Load()
    {
        if (_auth.CurrentPhone is not { } phone) return;
        Parcels.Clear();
        foreach (var p in await _db.GetParcelsAsync(phone)) Parcels.Add(p);
    }

    [RelayCommand]
    private async Task Next()
    {
        ErrorMessage = "";
        if (string.IsNullOrEmpty(Crop)) { ErrorMessage = "Choisissez la culture à diagnostiquer."; return; }
        _session.Crop = Crop!;
        _session.ParcelId = SelectedParcel?.Id ?? 0;
        _session.ParcelName = SelectedParcel?.Name ?? "";
        await Shell.Current.GoToAsync("diagphoto");
    }
}

// ---------- Étape 2 : photo ----------
public partial class DiagPhotoViewModel : BaseViewModel
{
    private readonly PhotoService _photos;
    private readonly DiagnosticSession _session;
    public DiagPhotoViewModel(PhotoService photos, DiagnosticSession session) { _photos = photos; _session = session; }

    [ObservableProperty] private ImageSource? photo;
    [ObservableProperty] private bool hasPhoto;

    private void SetPhoto(string? path)
    {
        if (path == null) return;
        _session.PhotoPath = path;
        Photo = ImageSource.FromFile(path);
        HasPhoto = true;
    }

    [RelayCommand]
    private async Task Take()
    {
        ErrorMessage = "";
        try
        {
            if (!_photos.CanCapture) { ErrorMessage = "Appareil photo indisponible : choisissez dans la galerie."; return; }
            SetPhoto(await _photos.CaptureAsync());
        }
        catch (Exception ex) { ErrorMessage = "Impossible d'ouvrir l'appareil photo (" + ex.Message + ")"; }
    }

    [RelayCommand]
    private async Task Pick()
    {
        ErrorMessage = "";
        try { SetPhoto(await _photos.PickAsync()); }
        catch (Exception ex) { ErrorMessage = "Impossible d'ouvrir la galerie (" + ex.Message + ")"; }
    }

    [RelayCommand]
    private async Task Next()
    {
        if (!HasPhoto)
        {
            var go = await Shell.Current.DisplayAlert("Photo", "Continuer sans photo ?", "Continuer", "Annuler");
            if (!go) return;
        }
        await Shell.Current.GoToAsync("diagsymptoms");
    }
}

// ---------- Étape 3 : symptômes ----------
public partial class SymptomItem : ObservableObject
{
    public string Key { get; init; } = "";
    public string Label { get; init; } = "";
    [ObservableProperty] private bool isChecked;
}

public partial class DiagSymptomsViewModel : BaseViewModel
{
    private readonly DiagnosticSession _session;
    private readonly DiagnosisService _diag;
    private readonly DatabaseService _db;
    private readonly AuthService _auth;
    private readonly SyncService _sync;

    public DiagSymptomsViewModel(DiagnosticSession session, DiagnosisService diag, DatabaseService db, AuthService auth, SyncService sync)
    { _session = session; _diag = diag; _db = db; _auth = auth; _sync = sync; }

    public ObservableCollection<SymptomItem> Items { get; } = new();
    public string[] Durations => DiagnosisService.Durations;
    [ObservableProperty] private string cultureText = "";
    [ObservableProperty] private string? selectedDuration;

    [RelayCommand]
    public void Load()
    {
        CultureText = $"Culture : {_session.Crop}";
        SelectedDuration = null;
        Items.Clear();
        foreach (var s in DiagnosisService.Symptoms) Items.Add(new SymptomItem { Key = s.Key, Label = s.Label });
    }

    [RelayCommand]
    private async Task Analyze()
    {
        ErrorMessage = "";
        var keys = Items.Where(i => i.IsChecked).Select(i => i.Key).ToList();
        if (keys.Count == 0) { ErrorMessage = "Cochez au moins un symptôme visible."; return; }
        if (string.IsNullOrEmpty(SelectedDuration)) { ErrorMessage = "Indiquez depuis quand vous voyez ces symptômes."; return; }
        if (_auth.CurrentPhone is not { } phone) return;

        IsBusy = true;
        try
        {
            _session.Symptoms = keys;
            _session.Duration = SelectedDuration!;
            _session.Results = await _diag.DiagnoseAsync(_session.Crop, keys, _session.PhotoPath);
            var top = _session.Results.FirstOrDefault();

            await _db.AddDiagnosticAsync(new DiagnosticRecord
            {
                OwnerPhone = phone, ParcelId = _session.ParcelId, ParcelName = _session.ParcelName,
                Crop = _session.Crop, PhotoPath = _session.PhotoPath ?? "",
                Symptoms = string.Join(", ", keys.Select(DiagnosisService.SymptomLabel)),
                Duration = _session.Duration,
                DiseaseId = top?.Disease.Id ?? "", DiseaseName = top?.Disease.Name ?? "Non identifiée",
                Confidence = top?.Confidence ?? 0, Reviewed = top == null
            });
            _ = _sync.SyncAsync();
        }
        finally { IsBusy = false; }
        await Shell.Current.GoToAsync("diagresult");
    }
}

// ---------- Étape 4 : résultat ----------
public partial class DiagResultViewModel : BaseViewModel
{
    private readonly DiagnosticSession _session;
    public DiagResultViewModel(DiagnosticSession session) => _session = session;

    public ObservableCollection<DiagnosisResult> Results { get; } = new();
    [ObservableProperty] private bool hasResults;
    [ObservableProperty] private string headline = "";
    [ObservableProperty] private string summary = "";
    [ObservableProperty] private ImageSource? photo;
    [ObservableProperty] private bool hasPhoto;

    [RelayCommand]
    public void Load()
    {
        Results.Clear();
        foreach (var r in _session.Results) Results.Add(r);
        HasResults = Results.Count > 0;
        HasPhoto = !string.IsNullOrEmpty(_session.PhotoPath);
        Photo = HasPhoto ? ImageSource.FromFile(_session.PhotoPath!) : null;
        Headline = HasResults ? $"Suspicion : {Results[0].Disease.Name}" : "Aucune maladie identifiée";
        Summary = HasResults
            ? "Résultat indicatif basé sur vos observations. Consultez la fiche pour les conseils de traitement."
            : "Les symptômes indiqués ne correspondent à aucune maladie connue pour cette culture. Prenez rendez-vous avec un spécialiste.";
    }

    [RelayCommand] private Task OpenDisease(DiagnosisResult r) => Shell.Current.GoToAsync($"diseasedetail?id={r.Disease.Id}");
    [RelayCommand] private Task Experts() => Shell.Current.GoToAsync("experts");
    [RelayCommand] private Task Done() => Shell.Current.GoToAsync("//home");
}

// ---------- Fiche de maladie ----------
[QueryProperty(nameof(DiseaseId), "id")]
public partial class DiseaseDetailViewModel : BaseViewModel
{
    private readonly DiagnosisService _diag;
    public DiseaseDetailViewModel(DiagnosisService diag) => _diag = diag;

    [ObservableProperty] private string diseaseId = "";
    [ObservableProperty] private Disease? disease;
    [ObservableProperty] private string symptomsText = "";

    async partial void OnDiseaseIdChanged(string value)
    {
        Disease = await _diag.GetByIdAsync(value);
        SymptomsText = Disease == null ? "" :
            string.Join("\n", Disease.Symptoms.Select(s => "• " + DiagnosisService.SymptomLabel(s)));
    }

    [RelayCommand] private Task Experts() => Shell.Current.GoToAsync("experts");
}

// ---------- Rendez-vous & paiement ----------
public partial class ExpertItem : ObservableObject
{
    public ExpertDef Def { get; init; } = null!;
    public string Title => Def.Title;
    public string Subtitle => Def.Subtitle;
    [ObservableProperty, NotifyPropertyChangedFor(nameof(Marker))] private bool isSelected;
    public string Marker => IsSelected ? "◉" : "○";
}

public partial class ExpertsViewModel : BaseViewModel
{
    private readonly BookingSession _booking;
    public ExpertsViewModel(BookingSession booking)
    {
        _booking = booking;
        foreach (var e in AppData.Experts) Experts.Add(new ExpertItem { Def = e });
        Experts[^1].IsSelected = true;
    }

    public ObservableCollection<ExpertItem> Experts { get; } = new();
    public string[] Slots => AppData.Slots;
    [ObservableProperty] private string? selectedSlot;

    [RelayCommand]
    private void Select(ExpertItem item)
    {
        foreach (var e in Experts) e.IsSelected = e == item;
    }

    [RelayCommand]
    private async Task Continue()
    {
        ErrorMessage = "";
        var chosen = Experts.FirstOrDefault(e => e.IsSelected);
        if (chosen == null) { ErrorMessage = "Choisissez un spécialiste."; return; }
        if (string.IsNullOrEmpty(SelectedSlot)) { ErrorMessage = "Choisissez un créneau."; return; }
        _booking.ExpertName = chosen.Def.Name;
        _booking.Slot = SelectedSlot!;
        await Shell.Current.GoToAsync("payment");
    }
}

public partial class PaymentViewModel : BaseViewModel
{
    private readonly BookingSession _booking;
    private readonly DatabaseService _db;
    private readonly AuthService _auth;
    public PaymentViewModel(BookingSession booking, DatabaseService db, AuthService auth)
    { _booking = booking; _db = db; _auth = auth; }

    public string[] Methods => AppData.PaymentMethods;
    [ObservableProperty] private string expertName = "";
    [ObservableProperty] private string slot = "";
    [ObservableProperty] private string priceText = "Tarif de démonstration : 3 000 FCFA (à remplacer)";
    [ObservableProperty, NotifyPropertyChangedFor(nameof(PaymentLabel)), NotifyPropertyChangedFor(nameof(NeedsPhone))] private string method = "Mobile Money";
    [ObservableProperty] private string phoneNumber = "";

    public string PaymentLabel => $"Paiement avec {Method}";
    public bool NeedsPhone => Method != "Carte bancaire";

    [RelayCommand]
    public void Load()
    {
        ExpertName = _booking.ExpertName;
        Slot = _booking.Slot;
    }

    [RelayCommand]
    private async Task Confirm()
    {
        ErrorMessage = "";
        if (_auth.CurrentPhone is not { } phone) return;
        if (NeedsPhone && AuthService.NormalizePhone(PhoneNumber).Length < 8)
        { ErrorMessage = "Entrez un numéro de téléphone valide."; return; }

        await _db.AddAppointmentAsync(new Appointment
        {
            OwnerPhone = phone, ExpertName = ExpertName, Slot = Slot,
            PaymentMethod = Method, PriceFcfa = AppData.DemoPriceFcfa
        });
        await Helpers.Alert("Rendez-vous (simulation)",
            $"Rendez-vous réservé avec {ExpertName}, {Slot}.\nAucun paiement réel n'a été effectué.");
        await Shell.Current.GoToAsync("//home");
    }
}

