namespace PhytoGuard;

public static class Helpers
{
    public static string Ago(DateTime d)
    {
        var s = DateTime.Now - d;
        if (s.TotalMinutes < 1) return "à l'instant";
        if (s.TotalHours < 1) return $"il y a {(int)s.TotalMinutes} min";
        if (s.TotalDays < 1) return $"il y a {(int)s.TotalHours} h";
        var days = (int)s.TotalDays;
        return days == 1 ? "il y a 1 jour" : $"il y a {days} jours";
    }

    public static Task Alert(string title, string msg) =>
        Shell.Current.DisplayAlert(title, msg, "OK");
}

