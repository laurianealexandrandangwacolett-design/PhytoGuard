using PhytoGuard.Pages;

namespace PhytoGuard;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();
        Routing.RegisterRoute("login", typeof(LoginPage));
        Routing.RegisterRoute("register", typeof(RegisterPage));
        Routing.RegisterRoute("forgot", typeof(ForgotPasswordPage));
        Routing.RegisterRoute("newparcel", typeof(NewParcelPage));
        Routing.RegisterRoute("diagculture", typeof(DiagCulturePage));
        Routing.RegisterRoute("diagphoto", typeof(DiagPhotoPage));
        Routing.RegisterRoute("diagsymptoms", typeof(DiagSymptomsPage));
        Routing.RegisterRoute("diagresult", typeof(DiagResultPage));
        Routing.RegisterRoute("diseasedetail", typeof(DiseaseDetailPage));
        Routing.RegisterRoute("experts", typeof(ExpertsPage));
        Routing.RegisterRoute("payment", typeof(PaymentPage));
    }
}
