using PhytoGuard.Models;
using SQLite;

namespace PhytoGuard.Services;

/// <summary>Base SQLite locale : tout fonctionne hors ligne.</summary>
public class DatabaseService
{
    private SQLiteAsyncConnection? _db;
    private readonly SemaphoreSlim _lock = new(1, 1);

    private async Task<SQLiteAsyncConnection> Db()
    {
        if (_db != null) return _db;
        await _lock.WaitAsync();
        try
        {
            if (_db == null)
            {
                var db = new SQLiteAsyncConnection(Path.Combine(FileSystem.AppDataDirectory, "phytoguard.db3"));
                await db.CreateTableAsync<UserAccount>();
                await db.CreateTableAsync<Parcel>();
                await db.CreateTableAsync<DiagnosticRecord>();
                await db.CreateTableAsync<Appointment>();
                _db = db;
            }
        }
        finally { _lock.Release(); }
        return _db;
    }

    // ---- Utilisateurs
    public async Task<UserAccount?> GetUserAsync(string phone) =>
        await (await Db()).Table<UserAccount>().Where(u => u.Phone == phone).FirstOrDefaultAsync();
    public async Task SaveUserAsync(UserAccount u)
    {
        var db = await Db();
        if (u.Id == 0) await db.InsertAsync(u); else await db.UpdateAsync(u);
    }

    // ---- Parcelles
    public async Task<List<Parcel>> GetParcelsAsync(string phone) =>
        await (await Db()).Table<Parcel>().Where(p => p.OwnerPhone == phone).OrderByDescending(p => p.CreatedAt).ToListAsync();
    public async Task AddParcelAsync(Parcel p) => await (await Db()).InsertAsync(p);

    // ---- Diagnostics
    public async Task<List<DiagnosticRecord>> GetDiagnosticsAsync(string phone) =>
        await (await Db()).Table<DiagnosticRecord>().Where(d => d.OwnerPhone == phone).OrderByDescending(d => d.CreatedAt).ToListAsync();
    public async Task AddDiagnosticAsync(DiagnosticRecord d) => await (await Db()).InsertAsync(d);
    public async Task UpdateDiagnosticAsync(DiagnosticRecord d) => await (await Db()).UpdateAsync(d);

    // ---- Rendez-vous
    public async Task AddAppointmentAsync(Appointment a) => await (await Db()).InsertAsync(a);

    // ---- Synchronisation
    public async Task<int> PendingCountAsync(string phone)
    {
        var db = await Db();
        var p = await db.Table<Parcel>().Where(x => x.OwnerPhone == phone && !x.Synced).CountAsync();
        var d = await db.Table<DiagnosticRecord>().Where(x => x.OwnerPhone == phone && !x.Synced).CountAsync();
        var a = await db.Table<Appointment>().Where(x => x.OwnerPhone == phone && !x.Synced).CountAsync();
        return p + d + a;
    }

    public async Task MarkAllSyncedAsync(string phone)
    {
        var db = await Db();
        await db.ExecuteAsync("UPDATE Parcel SET Synced = 1 WHERE OwnerPhone = ?", phone);
        await db.ExecuteAsync("UPDATE DiagnosticRecord SET Synced = 1 WHERE OwnerPhone = ?", phone);
        await db.ExecuteAsync("UPDATE Appointment SET Synced = 1 WHERE OwnerPhone = ?", phone);
    }
}
