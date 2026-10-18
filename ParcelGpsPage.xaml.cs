using System;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Maps;
using Microsoft.Maui.Devices.Sensors;
using Microsoft.Maui.Maps;
using Microsoft.Maui.Storage;

namespace PhytoGuard;

public partial class ParcelGpsPage : ContentPage
{
    public ParcelGpsPage()
    {
        InitializeComponent();
    }

    private async void OnAdviceTapped(object? sender, TappedEventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(AdvicePage));
    }

    private async void OnHistoryTapped(object? sender, TappedEventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(HistoryPage));
    }

    private async void OnSaveParcelClicked(object? sender, EventArgs e)
    {
        string nom = ParcelNameEntry.Text?.Trim() ?? "";
        string culture = CropEntry.Text?.Trim() ?? "";

        if (string.IsNullOrWhiteSpace(nom) ||
            string.IsNullOrWhiteSpace(culture))
        {
            await DisplayAlertAsync(
                "Informations manquantes",
                "Saisissez le nom de la parcelle et la culture.",
                "OK");
            return;
        }

        Preferences.Set("SavedParcelName", nom);
        Preferences.Set("SavedCrop", culture);
        Preferences.Set("SavedCoordinates", GpsCoordinatesLabel.Text ?? "");
        Preferences.Set(
    "SavedParcelDate",
    DateTime.Now.ToString("O", System.Globalization.CultureInfo.InvariantCulture));

        await DisplayAlertAsync(
            "Parcelle enregistrée",
            $"{nom} · {culture} est enregistrée sur cet appareil.",
            "OK");

        await Shell.Current.GoToAsync(nameof(HomePage));
    }

    private async void OnGetLocationClicked(object? sender, EventArgs e)
    {
        try
        {
            var request = new GeolocationRequest(
                GeolocationAccuracy.Medium,
                TimeSpan.FromSeconds(10));

            Location? location =
                await Geolocation.Default.GetLocationAsync(request);

            if (location is null)
            {
                await DisplayAlertAsync(
                    "Position introuvable",
                    "Le téléphone n’a pas réussi à obtenir votre position. Réessayez dehors.",
                    "OK");
                return;
            }

            GpsCoordinatesLabel.Text =
                $"{location.Latitude:F6}°, {location.Longitude:F6}°";

            GpsAccuracyLabel.Text = location.Accuracy.HasValue
                ? $"Précision estimée : ± {location.Accuracy.Value:F0} m"
                : "Position obtenue";

            // Centre la carte sur la position et ajoute un repère.
            ParcelMap.MoveToRegion(
                MapSpan.FromCenterAndRadius(
                    location,
                    Distance.FromKilometers(0.5)));

            ParcelMap.Pins.Clear();
            ParcelMap.Pins.Add(new Pin
            {
                Label = "Ma parcelle",
                Type = PinType.Place,
                Location = location
            });
        }
        catch (FeatureNotEnabledException)
        {
            await DisplayAlertAsync(
                "GPS désactivé",
                "Activez la localisation dans les réglages du téléphone, puis réessayez.",
                "OK");
        }
        catch (PermissionException)
        {
            await DisplayAlertAsync(
                "Autorisation nécessaire",
                "Autorisez PhytoGuard à accéder à la position dans les réglages du téléphone.",
                "OK");
        }
        catch (Exception)
        {
            await DisplayAlertAsync(
                "Erreur GPS",
                "La position n’a pas pu être récupérée. Vérifiez que la localisation est activée et réessayez.",
                "OK");
        }
    }
}