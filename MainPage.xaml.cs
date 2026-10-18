namespace PhytoGuard;

public partial class MainPage : ContentPage
{

	private async void OnCreateAccountClicked(object sender, EventArgs e)
{
    await Shell.Current.GoToAsync(nameof(RegisterPage));
}


private async void OnLoginClicked(object sender, EventArgs e)
{
    await Shell.Current.GoToAsync(nameof(LoginPage));
}
    public MainPage()
    {
        InitializeComponent();
    }
}