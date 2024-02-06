using App.Lib.Test.Database;

namespace App.Lib.Test;

public interface IDatabaseFixture
{
    PooledDatabase Database { get; }
}
