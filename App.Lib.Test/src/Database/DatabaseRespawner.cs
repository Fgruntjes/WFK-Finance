using Respawn;

namespace App.Lib.Test.Database;

public class DatabaseRespawner
{
    private readonly string _connectionString;
    private Respawner? _respawner;

    public DatabaseRespawner(string connectionString)
    {
        _connectionString = connectionString;
    }

    public async Task InitializeAsync()
    {
        _respawner = await Respawner.CreateAsync(_connectionString);
        // Ensure we always start with a clean database
        await _respawner.ResetAsync(_connectionString);
    }

    public async Task ResetAsync()
    {
        if (_respawner != null)
        {
            await _respawner.ResetAsync(_connectionString);
        }
    }
}