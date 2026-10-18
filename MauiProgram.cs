using Microsoft.Extensions.Logging;
using Microsoft.Maui.Controls.Maps;

namespace PhytoGuard;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();

        builder
            .UseMauiApp<App>()
            .UseMauiMaps();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}