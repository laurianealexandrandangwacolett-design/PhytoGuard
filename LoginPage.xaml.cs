namespace PhytoGuard;

public partial class LoginPage : ContentPage
{

private async void OnCreateAccountClicked(object? sender, EventArgs e)
{
    await Shell.Current.GoToAsync(nameof(RegisterPage));
}
    private async void OnAccountTapped(object? sender, TappedEventArgs e)
{
    await Shell.Current.GoToAsync(nameof(AccountPage));
}
    private async void OnForgotPasswordClicked(object sender, EventArgs e)
{
    await Shell.Current.GoToAsync(nameof(ResetPasswordPage));
}

private async void OnLoginClicked(object sender, EventArgs e)
{
    await Shell.Current.GoToAsync(nameof(HomePage));
}
    public LoginPage()
    {
        InitializeComponent();
    }
}