using Microsoft.Maui.Storage;

namespace PhytoGuard;

public partial class AdvicePage : ContentPage
{

private async void OnParcelsTapped(object? sender, TappedEventArgs e)
{
    await Shell.Current.GoToAsync(nameof(ParcelsPage));
}
    private async void OnHomeTapped(object? sender, TappedEventArgs e)
{
    await Shell.Current.GoToAsync(nameof(HomePage));
}
    public AdvicePage()
    {
        InitializeComponent();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        string culture = Preferences.Get("SavedCrop", "");

        if (string.IsNullOrWhiteSpace(culture))
        {
            CropSubtitle.Text = "Enregistrez une parcelle pour obtenir des conseils liés à sa culture.";
            PreventionTitle.Text = "Surveillez régulièrement vos plants";
            PreventionText.Text = "Observez les feuilles et les tiges. Arrosez au pied pour éviter de mouiller le feuillage.";
            PracticalText.Text = "Notez les changements observés et prenez une photo nette. Demandez confirmation avant d’appliquer un traitement.";
            return;
        }

        CropSubtitle.Text = $"Conseils pour la culture : {culture}";
        PreventionTitle.Text = $"Surveillez vos plants de {culture}";
        PreventionText.Text = "Inspectez régulièrement les feuilles, surtout celles du bas. Retirez les feuilles très abîmées et évitez de mouiller le feuillage.";
        PracticalText.Text = "Notez la date et les symptômes observés dans l’historique. Utilisez uniquement des traitements adaptés à cette culture et suivez leur étiquette.";
    }

    private async void OnHistoryClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(HistoryPage));
    }
}