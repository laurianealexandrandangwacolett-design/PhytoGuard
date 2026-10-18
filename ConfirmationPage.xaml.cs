using Microsoft.Maui.Storage;

namespace PhytoGuard;

public partial class ConfirmationPage : ContentPage
{
    public ConfirmationPage()
    {
        InitializeComponent();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        string expert = Preferences.Get(
            "AppointmentExpert", "Spécialiste non choisi");

        string creneau = Preferences.Get(
            "AppointmentSlot", "Créneau non choisi");

        string paiement = Preferences.Get(
            "SelectedPaymentMethod", "Moyen non choisi");

        string nom = Preferences.Get("UserName", "");

        WelcomeLabel.Text = string.IsNullOrWhiteSpace(nom)
            ? "À bientôt"
            : $"À bientôt avec {nom}";

        SlotConfirmationLabel.Text = creneau;
        ExpertConfirmationLabel.Text = $"{expert} · Consultation";
        PaymentConfirmationLabel.Text =
            $"Moyen choisi : {paiement} (simulation)";
    }

    private async void OnAccountClicked(object? sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(AccountPage));
    }

    private async void OnAccountTapped(object? sender, TappedEventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(AccountPage));
    }
}