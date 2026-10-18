using System;
using System.Globalization;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;

namespace PhytoGuard;

public partial class HomePage : ContentPage
{
    public HomePage()
    {
        InitializeComponent();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        string prenom = Preferences.Get("UserName", "");

        GreetingLabel.Text = string.IsNullOrWhiteSpace(prenom)
            ? "Bonjour"
            : $"Bonjour, {prenom}";

        RefreshParcelCard();
    }

    private void RefreshParcelCard()
    {
        string nom = Preferences.Get("SavedParcelName", "");
        string culture = Preferences.Get("SavedCrop", "");
        string dateEnregistree = Preferences.Get("SavedParcelDate", "");

        ParcelNameLabel.Text = string.IsNullOrWhiteSpace(nom)
            ? "Aucune parcelle enregistrée"
            : nom;

        ParcelCropLabel.Text = string.IsNullOrWhiteSpace(culture)
            ? "Culture non renseignée"
            : culture;

        if (DateTime.TryParse(
                dateEnregistree,
                CultureInfo.InvariantCulture,
                DateTimeStyles.RoundtripKind,
                out DateTime date))
        {
            ParcelDateLabel.Text =
                $"Enregistrée le {date.ToLocalTime():dd/MM/yyyy à HH:mm}";
        }
        else
        {
            ParcelDateLabel.Text = "Date d’enregistrement indisponible";
        }
    }

    private async void OnParcelsTapped(
        object? sender,
        TappedEventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(ParcelsPage));
    }

    private async void OnAdviceTapped(
        object? sender,
        TappedEventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(AdvicePage));
    }

    private async void OnHistoryTapped(
        object? sender,
        TappedEventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(HistoryPage));
    }

    private async void OnAddParcelClicked(
        object? sender,
        EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(ParcelGpsPage));
    }

    private async void OnNewDiagnosticClicked(
        object? sender,
        EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(TakePhotoPage));
    }
}