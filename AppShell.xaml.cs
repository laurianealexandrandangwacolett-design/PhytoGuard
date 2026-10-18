namespace PhytoGuard;

public partial class AppShell : Shell
{
	public AppShell()
{
    InitializeComponent();
	Routing.RegisterRoute(nameof(LoginPage), typeof(LoginPage));
	Routing.RegisterRoute(nameof(ResetPasswordPage), typeof(ResetPasswordPage));
    Routing.RegisterRoute(nameof(RegisterPage), typeof(RegisterPage));
	Routing.RegisterRoute(nameof(HomePage), typeof(HomePage));
	Routing.RegisterRoute(nameof(ParcelGpsPage), typeof(ParcelGpsPage));
	Routing.RegisterRoute(nameof(TakePhotoPage), typeof(TakePhotoPage));
	Routing.RegisterRoute(
    nameof(GuidedSymptomsPage),
    typeof(GuidedSymptomsPage));
	Routing.RegisterRoute(nameof(ResultPage), typeof(ResultPage));
	Routing.RegisterRoute(nameof(HistoryPage), typeof(HistoryPage));
	Routing.RegisterRoute(nameof(AdvicePage), typeof(AdvicePage));
	Routing.RegisterRoute(nameof(ExpertsPage), typeof(ExpertsPage));
Routing.RegisterRoute(nameof(PaymentPage), typeof(PaymentPage));
Routing.RegisterRoute(nameof(ConfirmationPage), typeof(ConfirmationPage));
Routing.RegisterRoute(nameof(AccountPage), typeof(AccountPage));
Routing.RegisterRoute(nameof(ParcelsPage), typeof(ParcelsPage));
}
}
