using System.Security.Cryptography;
using PhytoGuard.Models;

namespace PhytoGuard.Services;

public class AuthService
{
    private const string SessionKey = "session_phone";
    private readonly DatabaseService _db;
    private readonly Dictionary<string, string> _resetCodes = new();

    public AuthService(DatabaseService db) => _db = db;

    public string? CurrentPhone => Preferences.Get(SessionKey, "") is { Length: > 0 } p ? p : null;
    public string CurrentName => Preferences.Get("session_name", "");
    public bool IsLoggedIn => CurrentPhone != null;

    public static string NormalizePhone(string? phone)
    {
        phone ??= "";
        var digits = new string(phone.Where(char.IsDigit).ToArray());
        return phone.TrimStart().StartsWith('+') ? "+" + digits : digits;
    }

    private static string Hash(string password, byte[] salt) =>
        Convert.ToBase64String(Rfc2898DeriveBytes.Pbkdf2(password, salt, 100_000, HashAlgorithmName.SHA256, 32));

    public async Task<(bool Ok, string Message)> RegisterAsync(string name, string phone, string password)
    {
        phone = NormalizePhone(phone);
        if (string.IsNullOrWhiteSpace(name)) return (false, "Entrez votre nom complet.");
        if (phone.Length < 8) return (false, "Numéro de téléphone invalide.");
        if (password.Length < 6) return (false, "Le mot de passe doit contenir au moins 6 caractères.");
        if (await _db.GetUserAsync(phone) != null) return (false, "Un compte existe déjà avec ce numéro.");

        var salt = RandomNumberGenerator.GetBytes(16);
        await _db.SaveUserAsync(new UserAccount
        {
            FullName = name.Trim(), Phone = phone,
            Salt = Convert.ToBase64String(salt), PasswordHash = Hash(password, salt)
        });
        StartSession(phone, name.Trim());
        return (true, "");
    }

    public async Task<(bool Ok, string Message)> LoginAsync(string phone, string password)
    {
        phone = NormalizePhone(phone);
        var user = await _db.GetUserAsync(phone);
        if (user == null || user.PasswordHash != Hash(password ?? "", Convert.FromBase64String(user.Salt)))
            return (false, "Téléphone ou mot de passe incorrect.");
        StartSession(user.Phone, user.FullName);
        return (true, "");
    }

    /// <summary>Génère un code à 6 chiffres. Un vrai SMS nécessite une passerelle (voir README) :
    /// ici le code est renvoyé pour être affiché en mode simulation.</summary>
    public async Task<string?> SendResetCodeAsync(string phone)
    {
        phone = NormalizePhone(phone);
        if (await _db.GetUserAsync(phone) == null) return null;
        var code = RandomNumberGenerator.GetInt32(100000, 999999).ToString();
        _resetCodes[phone] = code;
        return code;
    }

    public async Task<(bool Ok, string Message)> ResetPasswordAsync(string phone, string code, string newPassword)
    {
        phone = NormalizePhone(phone);
        if (!_resetCodes.TryGetValue(phone, out var expected) || expected != code?.Trim())
            return (false, "Code de vérification incorrect.");
        if (newPassword.Length < 6) return (false, "Le mot de passe doit contenir au moins 6 caractères.");
        var user = await _db.GetUserAsync(phone);
        if (user == null) return (false, "Compte introuvable.");
        var salt = RandomNumberGenerator.GetBytes(16);
        user.Salt = Convert.ToBase64String(salt);
        user.PasswordHash = Hash(newPassword, salt);
        await _db.SaveUserAsync(user);
        _resetCodes.Remove(phone);
        return (true, "");
    }

    private static void StartSession(string phone, string name)
    {
        Preferences.Set(SessionKey, phone);
        Preferences.Set("session_name", name);
    }

    public void Logout()
    {
        Preferences.Remove(SessionKey);
        Preferences.Remove("session_name");
    }
}
