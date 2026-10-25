namespace PhytoGuard.Controls;

public partial class StepBar : ContentView
{
    public static readonly BindableProperty CurrentProperty = BindableProperty.Create(nameof(Current), typeof(int), typeof(StepBar), 1,
        propertyChanged: (b, o, n) => ((StepBar)b).Refresh());

    public int Current { get => (int)GetValue(CurrentProperty); set => SetValue(CurrentProperty, value); }

    public StepBar()
    {
        InitializeComponent();
        Refresh();
    }

    private void Refresh()
    {
        var labels = new[] { L1, L2, L3, L4 };
        for (int i = 0; i < labels.Length; i++)
        {
            bool on = i + 1 == Current;
            labels[i].TextColor = on ? Color.FromArgb("#A8D08D") : Color.FromArgb("#A9B8AF");
            labels[i].FontAttributes = on ? FontAttributes.Bold : FontAttributes.None;
        }
    }
}

