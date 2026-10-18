using System;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;

namespace PhytoGuard;

public partial class ResultPage : ContentPage
{
    public ResultPage()
    {
        InitializeComponent();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        string culture = Preferences.Get("SavedCrop", "");
        string parcelle = Preferences.Get("SavedParcelName", "");

        string cultureTexte = string.IsNullOrWhiteSpace(culture)
            ? "Culture non renseignée"
            : culture;

        string parcelleTexte = string.IsNullOrWhiteSpace(parcelle)
            ? "Parcelle non renseignée"
            : $"parcelle {parcelle}";

        ParcelCropLabel.Text =
            $"{cultureTexte} · {parcelleTexte} · {DateTime.Now:dd/MM/yyyy}";
    }

    private async void OnBookExpertClicked(object? sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(ExpertsPage));
    }

    private async void OnAdviceTapped(object? sender, TappedEventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(AdvicePage));
    }

    private async void OnHistoryTapped(object? sender, TappedEventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(HistoryPage));
    }
}