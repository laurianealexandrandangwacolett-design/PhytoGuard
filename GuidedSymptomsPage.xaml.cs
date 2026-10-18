using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;

namespace PhytoGuard;

public partial class GuidedSymptomsPage : ContentPage
{
    public GuidedSymptomsPage()
    {
        InitializeComponent();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        // Afficher la culture sauvegardée.
        string culture = Preferences.Get("SavedCrop", "");

        SavedCropLabel.Text = string.IsNullOrWhiteSpace(culture)
            ? "Aucune culture enregistrée"
            : $"Culture de la parcelle : {culture}";

        // Afficher la photo choisie sur TakePhotoPage.
        string cheminPhoto = Preferences.Get("SelectedPlantPhotoPath", "");

        bool photoDisponible =
            !string.IsNullOrWhiteSpace(cheminPhoto) &&
            File.Exists(cheminPhoto);

        SymptomPhotoPreview.IsVisible = photoDisponible;
        NoPhotoLabel.IsVisible = !photoDisponible;

        if (photoDisponible)
        {
            SymptomPhotoPreview.Source = ImageSource.FromFile(cheminPhoto);
        }
        else
        {
            SymptomPhotoPreview.Source = null;
        }

        // Restaurer les sélections si l’utilisateur revient sur cette page.
        string symptomesEnregistres = Preferences.Get("Symptoms", "");

        YellowSpotsCheck.IsChecked =
            symptomesEnregistres.Contains("Taches jaunes", StringComparison.OrdinalIgnoreCase);

        DryLeavesCheck.IsChecked =
            symptomesEnregistres.Contains("Feuilles sèches", StringComparison.OrdinalIgnoreCase);

        WhitePowderCheck.IsChecked =
            symptomesEnregistres.Contains("Poudre blanche", StringComparison.OrdinalIgnoreCase);

        string periodeEnregistree = Preferences.Get("SymptomsDuration", "");
        if (!string.IsNullOrWhiteSpace(periodeEnregistree))
        {
            DurationPicker.SelectedItem = periodeEnregistree;
        }
    }

    private async void OnContinueClicked(object? sender, EventArgs e)
    {
        List<string> symptomes = new();

        if (YellowSpotsCheck.IsChecked)
            symptomes.Add("Taches jaunes");

        if (DryLeavesCheck.IsChecked)
            symptomes.Add("Feuilles sèches");

        if (WhitePowderCheck.IsChecked)
            symptomes.Add("Poudre blanche");

        if (symptomes.Count == 0)
        {
            await DisplayAlert(
                "Choisissez un symptôme",
                "Cochez au moins un symptôme pour continuer.",
                "OK");
            return;
        }

        string periode = DurationPicker.SelectedItem?.ToString() ?? "Non précisé";

        // Enregistrer les valeurs avant de naviguer vers le résultat.
        Preferences.Set("Symptoms", string.Join(", ", symptomes));
        Preferences.Set("SymptomsDuration", periode);

        await Shell.Current.GoToAsync(nameof(ResultPage));
    }
}