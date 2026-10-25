using PhytoGuard.Services;

namespace PhytoGuard;

public partial class App : Application
{
    // SyncService est injecté pour démarrer l'écoute du réseau dès le lancement.
    public App(SyncService sync)
    {
        InitializeComponent();
        MainPage = new AppShell();
    }
}
