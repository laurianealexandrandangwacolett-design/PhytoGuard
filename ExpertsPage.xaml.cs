namespace PhytoGuard;

public partial class ExpertsPage : ContentPage
{
    private string expertChoisi = "";

    private readonly Dictionary<string, List<string>> creneaux =
        new()
        {
            ["Amina N."] = new()
            {
                "Vendredi · 14:30",
                "Samedi · 09:00",
                "Lundi · 11:30"
            },
            ["Jean M."] = new()
            {
                "Vendredi · 16:00",
                "Samedi · 10:30"
            },
            ["Sarah K."] = new()
            {
                "Lundi · 08:30",
                "Mardi · 13:00"
            }
        };

    public ExpertsPage()
    {
        InitializeComponent();
        AminaOption.IsChecked = true;
    }

    private void OnExpertChanged(object sender, CheckedChangedEventArgs e)
    {
        if (!e.Value || sender is not RadioButton choix)
            return;

        expertChoisi = choix.Value?.ToString() ?? "";

        if (creneaux.TryGetValue(expertChoisi, out var horaires))
        {
            SlotPicker.ItemsSource = horaires;
            SlotPicker.SelectedIndex = 0;
        }
    }

    private async void OnContinueClicked(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(expertChoisi) ||
            SlotPicker.SelectedItem is null)
        {
            await DisplayAlert(
                "Choix incomplet",
                "Choisissez un spécialiste et un créneau.",
                "OK");
            return;
        }

        Preferences.Set("AppointmentExpert", expertChoisi);
        Preferences.Set(
            "AppointmentSlot",
            SlotPicker.SelectedItem.ToString() ?? "");

        await Shell.Current.GoToAsync(nameof(PaymentPage));
    }
}