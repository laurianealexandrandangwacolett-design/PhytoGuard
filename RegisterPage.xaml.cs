using System;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;

namespace PhytoGuard
{
    public partial class RegisterPage : ContentPage
    {
        public RegisterPage()
        {
            InitializeComponent();
        }

        private async void OnCreateAccountClicked(object sender, EventArgs e)
        {
            string nomComplet = NameEntry.Text?.Trim() ?? "";

            if (string.IsNullOrWhiteSpace(nomComplet))
            {
                await DisplayAlert(
                    "Information manquante",
                    "Saisissez votre nom complet.",
                    "OK");
                return;
            }

            string prenom = nomComplet.Split(
                ' ',
                StringSplitOptions.RemoveEmptyEntries)[0];

            Preferences.Set("UserName", prenom);

            await Shell.Current.GoToAsync(nameof(HomePage));
        }
    }
}