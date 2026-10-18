using System.Linq;
using Microsoft.Maui.Storage;

namespace PhytoGuard;

public partial class HistoryPage : ContentPage
{

    private async void OnHomeTapped(object? sender, TappedEventArgs e)
{
    await Shell.Current.GoToAsync(nameof(HomePage));
}

    private async void OnAdviceTapped(object sender, TappedEventArgs e)
{
    await Shell.Current.GoToAsync(nameof(AdvicePage));
}
    public HistoryPage()
    {
        InitializeComponent();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        string parcelle = Preferences.Get("SavedParcelName", "");
        string culture = Preferences.Get("SavedCrop", "");

        ParcelLabel.Text = string.IsNullOrWhiteSpace(parcelle)
            ? "Aucune parcelle enregistrée"
            : parcelle;

        CropLabel.Text = string.IsNullOrWhiteSpace(culture)
            ? "Enregistrez une culture pour cette parcelle."
            : culture;

        RecordsLayout.Children.Clear();

        if (string.IsNullOrWhiteSpace(parcelle) ||
            string.IsNullOrWhiteSpace(culture))
        {
            EmptyLabel.Text = "Enregistrez d’abord une parcelle et sa culture.";
            EmptyLabel.IsVisible = true;
            return;
        }

        var observations = HistoryStore.GetAll()
            .Where(o =>
                o.ParcelName.Equals(parcelle, StringComparison.OrdinalIgnoreCase) &&
                o.Crop.Equals(culture, StringComparison.OrdinalIgnoreCase))
            .OrderByDescending(o => o.Date)
            .ToList();

        EmptyLabel.IsVisible = observations.Count == 0;
        EmptyLabel.Text = "Aucune observation enregistrée pour cette parcelle.";

        foreach (var observation in observations)
        {
            var contenu = new VerticalStackLayout { Spacing = 5 };

            contenu.Children.Add(new Label
            {
                Text = observation.DateLabel,
                FontSize = 11,
                TextColor = Color.FromArgb("#A4D6AE")
            });

            contenu.Children.Add(new Label
            {
                Text = observation.Title,
                FontSize = 14,
                FontAttributes = FontAttributes.Bold,
                TextColor = Colors.White
            });

            contenu.Children.Add(new Label
            {
                Text = observation.Details,
                FontSize = 11,
                TextColor = Color.FromArgb("#B8C7BC")
            });

            RecordsLayout.Children.Add(new Border
            {
                BackgroundColor = Color.FromArgb("#20362A"),
                Stroke = Color.FromArgb("#304B38"),
                StrokeThickness = 1,
                StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle
                {
                    CornerRadius = new CornerRadius(14)
                },
                Padding = 12,
                Content = contenu
            });
        }
    }
}