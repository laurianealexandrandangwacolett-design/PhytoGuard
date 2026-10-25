namespace PhytoGuard.Services;

public class LocationService
{
    public async Task<Location?> GetCurrentAsync()
    {
        var status = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();
        if (status != PermissionStatus.Granted)
            status = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
        if (status != PermissionStatus.Granted) return null;
        try
        {
            var req = new GeolocationRequest(GeolocationAccuracy.Best, TimeSpan.FromSeconds(15));
            return await Geolocation.Default.GetLocationAsync(req) ?? await Geolocation.Default.GetLastKnownLocationAsync();
        }
        catch { return await Geolocation.Default.GetLastKnownLocationAsync(); }
    }
}

public class PhotoService
{
    public bool CanCapture => MediaPicker.Default.IsCaptureSupported;

    public async Task<string?> CaptureAsync()
    {
        if (!CanCapture) return null;
        return await SaveAsync(await MediaPicker.Default.CapturePhotoAsync());
    }

    public async Task<string?> PickAsync() =>
        await SaveAsync(await MediaPicker.Default.PickPhotoAsync());

    /// <summary>La photo est gardée sur le téléphone (dossier de l'application).</summary>
    private static async Task<string?> SaveAsync(FileResult? file)
    {
        if (file == null) return null;
        var dir = Path.Combine(FileSystem.AppDataDirectory, "photos");
        Directory.CreateDirectory(dir);
        var path = Path.Combine(dir, $"{Guid.NewGuid():N}.jpg");
        using var src = await file.OpenReadAsync();
        using var dst = File.Create(path);
        await src.CopyToAsync(dst);
        return path;
    }
}

public class SyncService
{
    private readonly DatabaseService _db;
    private readonly AuthService _auth;
    public event Action? StatusChanged;

    public SyncService(DatabaseService db, AuthService auth)
    {
        _db = db; _auth = auth;
        Connectivity.Current.ConnectivityChanged += async (_, _) =>
        {
            if (IsOnline) await SyncAsync();
            StatusChanged?.Invoke();
        };
    }

    public bool IsOnline => Connectivity.Current.NetworkAccess == NetworkAccess.Internet;

    public async Task<int> PendingAsync() =>
        _auth.CurrentPhone is { } p ? await _db.PendingCountAsync(p) : 0;

    /// <summary>Envoie les données en attente. Ici : simulation (à remplacer par un appel à une API REST).</summary>
    public async Task SyncAsync()
    {
        if (!IsOnline || _auth.CurrentPhone is not { } phone) return;
        // TODO API : POST des parcelles / diagnostics / rendez-vous non synchronisés.
        await Task.Delay(300);
        await _db.MarkAllSyncedAsync(phone);
        StatusChanged?.Invoke();
    }
}

public static class AppData
{
    public static readonly string[] Slots =
        { "Lundi · 08:30", "Lundi · 10:00", "Mardi · 09:00", "Mercredi · 14:00", "Jeudi · 11:00" };
    public const int DemoPriceFcfa = 3000;
    public static readonly string[] PaymentMethods = { "Mobile Money", "Orange Money", "Carte bancaire" };

    public static readonly List<Models.ExpertDef> Experts = new()
    {
        new("Amina N.", "Phytopathologie", "Appel vidéo", "Français", 20),
        new("Jean M.", "Agronomie", "Appel audio", "Français", 30),
        new("Sarah K.", "Protection des cultures", "Appel vidéo", "Français", 20)
    };

    public static readonly string[] GeneralTips =
    {
        "Photographiez la feuille à la lumière du jour, sans flou.",
        "Inspectez vos parcelles 2 fois par semaine, surtout après la pluie.",
        "Retirez et brûlez les plants très atteints pour protéger les autres.",
        "Respectez toujours la dose et le délai avant récolte indiqués sur l'étiquette d'un produit.",
        "Faites tourner les cultures d'une saison à l'autre.",
        "Désinfectez vos outils après avoir travaillé sur des plants malades."
    };
}
