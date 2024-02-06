using Bogus;

namespace App.Lib.Test.Data;

public class FakeEntityBuilder<T> where T : class
{
    protected readonly Faker<T> Faker;

    public FakeEntityBuilder()
    {
        Faker = new Faker<T>();
    }

    public FakeEntityBuilder<T> WithDefaults(Action<T> set)
    {
        Faker.FinishWith((faker, entity) =>
        {
            set(entity);
        });
        return this;
    }

    public T Generate()
    {
        return Faker.Generate();
    }
}
