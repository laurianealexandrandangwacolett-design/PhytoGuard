using Microsoft.Maui.Storage;

namespace PhytoGuard;

public partial class PaymentPage : ContentPage
{
    public PaymentPage()
    {
        InitializeComponent();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        ExpertLabel.Text =
            Preferences.Get("AppointmentExpert", "Spécialiste non choisi");

        SlotLabel.Text =
            Preferences.Get("AppointmentSlot", "Créneau non choisi");

        UpdatePaymentFields();
    }

    private void OnPaymentMethodChanged(
        object? sender,
        CheckedChangedEventArgs e)
    {
        if (!e.Value)
            return;

        UpdatePaymentFields();
    }

    private void UpdatePaymentFields()
    {
        bool isCard = CardOption.IsChecked;
        bool isOrangeMoney = OrangeMoneyOption.IsChecked;

        PaymentPhoneEntry.IsVisible = !isCard;
        CardEntry.IsVisible = isCard;

        if (isCard)
            PaymentInfoLabel.Text = "Paiement par carte (simulation)";
        else if (isOrangeMoney)
            PaymentInfoLabel.Text = "Paiement avec Orange Money";
        else
            PaymentInfoLabel.Text = "Paiement avec Mobile Money";
    }

    private async void OnPayClicked(object? sender, EventArgs e)
    {
        if (CardOption.IsChecked &&
            string.IsNullOrWhiteSpace(CardEntry.Text))
        {
           Preferences.Set("AppointmentStatus", "Confirmé (simulation)");

await Shell.Current.GoToAsync(nameof(ConfirmationPage));
        }

        if (!CardOption.IsChecked &&
            string.IsNullOrWhiteSpace(PaymentPhoneEntry.Text))
        {
            await DisplayAlertAsync(
                "Numéro manquant",
                "Saisissez le numéro associé au moyen de paiement choisi.",
                "OK");
            return;
        }

        string moyenPaiement =
            CardOption.IsChecked ? "Carte bancaire" :
            OrangeMoneyOption.IsChecked ? "Orange Money" :
            "Mobile Money";

        Preferences.Set("SelectedPaymentMethod", moyenPaiement);

        PaymentStatusLabel.Text =
            $"Demande enregistrée avec {moyenPaiement} (simulation).";
        PaymentStatusLabel.IsVisible = true;

        await DisplayAlertAsync(
            "Simulation terminée",
            "Aucun argent n’a été débité. Un vrai paiement nécessite l’intégration d’un prestataire.",
            "OK");
    }
}