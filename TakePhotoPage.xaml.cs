using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Media;
using Microsoft.Maui.Storage;

namespace PhytoGuard;

public partial class TakePhotoPage : ContentPage
{
    public TakePhotoPage()
    {
        InitializeComponent();
    }

    private async void OnCultureTapped(object? sender, TappedEventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(ParcelGpsPage));
    }

    private async void OnPhotoTapped(object? sender, TappedEventArgs e)
    {
        // La page photo est déjà affichée.
        await Task.CompletedTask;
    }

    private async void OnSymptomsTapped(object? sender, TappedEventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(GuidedSymptomsPage));
    }

    private async void OnResultTapped(object? sender, TappedEventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(ResultPage));
    }

    private async void OnTakePhotoClicked(object? sender, EventArgs e)
    {
        try
        {
            if (!MediaPicker.Default.IsCaptureSupported)
            {
                await DisplayAlert(
                    "Caméra indisponible",
                    "La prise de photo n’est pas disponible sur cet appareil.",
                    "OK");
                return;
            }

            FileResult? photo = await MediaPicker.Default.CapturePhotoAsync();
            await ShowPhotoAsync(photo);
        }
        catch (Exception)
        {
            await DisplayAlert(
                "Erreur",
                "Impossible d’ouvrir la caméra. Vérifiez son autorisation et réessayez.",
                "OK");
        }
    }

    private async void OnChoosePhotoClicked(object? sender, EventArgs e)
    {
        try
        {
            var photos = await MediaPicker.Default.PickPhotosAsync(
                new MediaPickerOptions
                {
                    SelectionLimit = 1
                });

            if (photos.Count > 0)
                await ShowPhotoAsync(photos[0]);
        }
        catch (Exception)
        {
            await DisplayAlert(
                "Erreur",
                "Impossible d’ouvrir la galerie de photos.",
                "OK");
        }
    }

    private async Task ShowPhotoAsync(FileResult? photo)
    {
        if (photo is null)
            return;

        string extension = Path.GetExtension(photo.FileName);
        if (string.IsNullOrWhiteSpace(extension))
            extension = ".jpg";

        string fileName = $"phytoguard_{Guid.NewGuid():N}{extension}";
        string localPath = Path.Combine(FileSystem.AppDataDirectory, fileName);

        await using (Stream source = await photo.OpenReadAsync())
        await using (FileStream destination = File.Create(localPath))
        {
            await source.CopyToAsync(destination);
        }

        // Affiche l’aperçu avant de demander confirmation.
        PhotoPreview.Source = ImageSource.FromFile(localPath);
        PhotoHintLabel.IsVisible = false;

        bool confirmation = await DisplayAlert(
            "Vérifier la photo",
            "La photo montre-t-elle clairement une plante ou une feuille ?",
            "Oui, continuer",
            "Non, reprendre");

        if (!confirmation)
        {
            Preferences.Remove("SelectedPlantPhotoPath");
            Preferences.Remove("LastPhotoPath");

            PhotoPreview.Source = null;
            PhotoHintLabel.IsVisible = true;
            return;
        }

        // Enregistre la photo confirmée et ouvre l’étape suivante.
        Preferences.Set("SelectedPlantPhotoPath", localPath);
        Preferences.Set("LastPhotoPath", localPath);

        await Shell.Current.GoToAsync(nameof(GuidedSymptomsPage));
    }
}