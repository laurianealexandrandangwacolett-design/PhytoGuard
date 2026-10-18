using Microsoft.Maui.Storage;

namespace PhytoGuard;

public partial class ParcelsPage : ContentPage
{
    public ParcelsPage()
    {
        InitializeComponent();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        string nom = Preferences.Get("SavedParcelName", "");
        string culture = Preferences.Get("SavedCrop", "");
        string coordonnees = Preferences.Get("SavedCoordinates", "");

        bool parcelleEnregistree = !string.IsNullOrWhiteSpace(nom);

        ParcelCard.IsVisible = parcelleEnregistree;
        EmptyMessageLabel.IsVisible = !parcelleEnregistree;

        if (parcelleEnregistree)
        {
            ParcelNameLabel.Text = nom;

            CropLabel.Text = string.IsNullOrWhiteSpace(culture)
                ? "Non renseignée"
                : culture;

            CoordinatesLabel.Text = string.IsNullOrWhiteSpace(coordonnees)
                ? "Non renseignées"
                : coordonnees;
        }
    }

    private async void OnHomeTapped(
        object? sender,
        TappedEventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(HomePage));
    }

    private async void OnHistoryTapped(
        object? sender,
        TappedEventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(HistoryPage));
    }

    private async void OnAdviceTapped(
        object? sender,
        TappedEventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(AdvicePage));
    }

    private async void OnAddParcelClicked(
        object? sender,
        EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(ParcelGpsPage));
    }
}