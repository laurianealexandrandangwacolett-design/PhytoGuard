namespace PhytoGuard.Controls;

public partial class LabeledEntry : ContentView
{
    public static readonly BindableProperty TitleProperty = BindableProperty.Create(nameof(Title), typeof(string), typeof(LabeledEntry), "",
        propertyChanged: (b, o, n) => ((LabeledEntry)b).TitleLabel.Text = (string)n);
    public static readonly BindableProperty TextProperty = BindableProperty.Create(nameof(Text), typeof(string), typeof(LabeledEntry), "",
        BindingMode.TwoWay,
        propertyChanged: (b, o, n) => { var c = (LabeledEntry)b; if (c.Input.Text != (string?)n) c.Input.Text = (string?)n; });
    public static readonly BindableProperty IsPasswordProperty = BindableProperty.Create(nameof(IsPassword), typeof(bool), typeof(LabeledEntry), false,
        propertyChanged: (b, o, n) => ((LabeledEntry)b).Input.IsPassword = (bool)n);
    public static readonly BindableProperty KeyboardProperty = BindableProperty.Create(nameof(Keyboard), typeof(Keyboard), typeof(LabeledEntry), Keyboard.Default,
        propertyChanged: (b, o, n) => ((LabeledEntry)b).Input.Keyboard = (Keyboard)n);

    public string Title { get => (string)GetValue(TitleProperty); set => SetValue(TitleProperty, value); }
    public string Text { get => (string)GetValue(TextProperty); set => SetValue(TextProperty, value); }
    public bool IsPassword { get => (bool)GetValue(IsPasswordProperty); set => SetValue(IsPasswordProperty, value); }
    public Keyboard Keyboard { get => (Keyboard)GetValue(KeyboardProperty); set => SetValue(KeyboardProperty, value); }

    public LabeledEntry()
    {
        InitializeComponent();
        Input.TextChanged += (s, e) => Text = e.NewTextValue ?? "";
    }
}

