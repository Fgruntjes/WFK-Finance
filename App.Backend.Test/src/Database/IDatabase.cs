namespace App.Backend.Test.Database;

public interface IDatabase
{
    string ConnectionString { get; }

    public void Initialize(IServiceProvider services);
    public ValueTask Clean();
}