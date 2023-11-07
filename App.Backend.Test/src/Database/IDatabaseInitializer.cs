namespace App.Backend.Test.Database;

public interface IDatabaseInitializer
{
    void Initialize(IServiceProvider services);
}