using Microsoft.Extensions.Logging;
using PhytoGuard.Pages;
using PhytoGuard.Services;
using PhytoGuard.ViewModels;

namespace PhytoGuard;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder.UseMauiApp<App>();

        var s = builder.Services;
        // Services
        s.AddSingleton<DatabaseService>();
        s.AddSingleton<AuthService>();
        s.AddSingleton<DiagnosisService>();
        s.AddSingleton<DiagnosticSession>();
        s.AddSingleton<BookingSession>();
        s.AddSingleton<LocationService>();
        s.AddSingleton<PhotoService>();
        s.AddSingleton<SyncService>();
#if USE_ONNX
        s.AddSingleton<IImageClassifier, OnnxImageClassifier>();
#else
        s.AddSingleton<IImageClassifier, NullImageClassifier>();
#endif

        // Pages + ViewModels
        Add<WelcomePage, WelcomeViewModel>(s);
        Add<LoginPage, LoginViewModel>(s);
        Add<RegisterPage, RegisterViewModel>(s);
        Add<ForgotPasswordPage, ForgotPasswordViewModel>(s);
        Add<HomePage, HomeViewModel>(s);
        Add<ParcelsPage, ParcelsViewModel>(s);
        Add<NewParcelPage, NewParcelViewModel>(s);
        Add<HistoryPage, HistoryViewModel>(s);
        Add<AdvicePage, AdviceViewModel>(s);
        Add<DiagCulturePage, DiagCultureViewModel>(s);
        Add<DiagPhotoPage, DiagPhotoViewModel>(s);
        Add<DiagSymptomsPage, DiagSymptomsViewModel>(s);
        Add<DiagResultPage, DiagResultViewModel>(s);
        Add<DiseaseDetailPage, DiseaseDetailViewModel>(s);
        Add<ExpertsPage, ExpertsViewModel>(s);
        Add<PaymentPage, PaymentViewModel>(s);

#if DEBUG
        builder.Logging.AddDebug();
#endif
        return builder.Build();
    }

    private static void Add<TPage, TVm>(IServiceCollection s) where TPage : class where TVm : class
    {
        s.AddTransient<TVm>();
        s.AddTransient<TPage>();
    }
}
