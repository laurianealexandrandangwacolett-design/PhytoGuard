using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PhytoGuard.Services;

namespace PhytoGuard.ViewModels;

public abstract partial class BaseViewModel : ObservableObject
{
    [ObservableProperty] private bool isBusy;
    [ObservableProperty] private string errorMessage = "";
}

public partial class WelcomeViewModel : BaseViewModel
{
    private readonly AuthService _auth;
    public WelcomeViewModel(AuthService auth) => _auth = auth;

    public bool IsLoggedIn => _auth.IsLoggedIn;

    [RelayCommand] private Task GoLogin() => Shell.Current.GoToAsync("login");
    [RelayCommand] private Task GoRegister() => Shell.Current.GoToAsync("register");
}

public partial class LoginViewModel : BaseViewModel
{
    private readonly AuthService _auth;
    public LoginViewModel(AuthService auth) => _auth = auth;

    [ObservableProperty] private string phone = "";
    [ObservableProperty] private string password = "";

    [RelayCommand]
    private async Task Login()
    {
        ErrorMessage = "";
        IsBusy = true;
        var (ok, msg) = await _auth.LoginAsync(Phone, Password);
        IsBusy = false;
        if (!ok) { ErrorMessage = msg; return; }
        Password = "";
        await Shell.Current.GoToAsync("//home");
    }

    [RelayCommand] private Task GoRegister() => Shell.Current.GoToAsync("register");
    [RelayCommand] private Task Forgot() => Shell.Current.GoToAsync("forgot");
}

public partial class RegisterViewModel : BaseViewModel
{
    private readonly AuthService _auth;
    public RegisterViewModel(AuthService auth) => _auth = auth;

    [ObservableProperty] private string fullName = "";
    [ObservableProperty] private string phone = "";
    [ObservableProperty] private string password = "";

    [RelayCommand]
    private async Task Register()
    {
        ErrorMessage = "";
        IsBusy = true;
        var (ok, msg) = await _auth.RegisterAsync(FullName, Phone, Password);
        IsBusy = false;
        if (!ok) { ErrorMessage = msg; return; }
        await Shell.Current.GoToAsync("//home");
    }
}

public partial class ForgotPasswordViewModel : BaseViewModel
{
    private readonly AuthService _auth;
    public ForgotPasswordViewModel(AuthService auth) => _auth = auth;

    [ObservableProperty] private string phone = "";
    [ObservableProperty] private string code = "";
    [ObservableProperty] private string newPassword = "";
    [ObservableProperty] private string confirmPassword = "";

    [RelayCommand]
    private async Task SendCode()
    {
        ErrorMessage = "";
        var code = await _auth.SendResetCodeAsync(Phone);
        if (code == null) { ErrorMessage = "Aucun compte avec ce numéro."; return; }
        // MODE SIMULATION : aucune passerelle SMS n'est branchée, le code est affiché ici.
        await Helpers.Alert("Code SMS (simulation)", $"Votre code de vérification : {code}");
    }

    [RelayCommand]
    private async Task Save()
    {
        ErrorMessage = "";
        if (NewPassword != ConfirmPassword) { ErrorMessage = "Les mots de passe ne correspondent pas."; return; }
        var (ok, msg) = await _auth.ResetPasswordAsync(Phone, Code, NewPassword);
        if (!ok) { ErrorMessage = msg; return; }
        await Helpers.Alert("Mot de passe", "Mot de passe modifié. Connectez-vous.");
        await Shell.Current.GoToAsync("..");
    }
}

