namespace PhytoGuard.Controls;

public partial class BottomNav : ContentView
{
    public static readonly BindableProperty CurrentProperty = BindableProperty.Create(nameof(Current), typeof(string), typeof(BottomNav), "home",
        propertyChanged: (b, o, n) => ((BottomNav)b).Refresh());

    public string Current { get => (string)GetValue(CurrentProperty); set => SetValue(CurrentProperty, value); }

    public BottomNav()
    {
        InitializeComponent();
        Refresh();
    }

    private void Refresh()
    {
        var routes = new[] { "home", "parcels", "history", "advice" };
        var texts = new[] { T1, T2, T3, T4 };
        var icons = new[] { I1, I2, I3, I4 };
        for (int i = 0; i < routes.Length; i++)
        {
            bool on = routes[i] == Current;
            texts[i].TextColor = on ? Color.FromArgb("#A8D08D") : Color.FromArgb("#A9B8AF");
            texts[i].FontAttributes = on ? FontAttributes.Bold : FontAttributes.None;
            icons[i].Opacity = on ? 1 : 0.6;
        }
    }

    private async void OnTapped(object? sender, TappedEventArgs e)
    {
        if (e.Parameter is not string route || route == Current) return;
        await Shell.Current.GoToAsync($"//{route}");
    }
}

