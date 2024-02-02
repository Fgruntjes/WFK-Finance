namespace App.Lib.Data;

public static class DictionaryExtension
{
    public static object MustGet(this System.Collections.IDictionary dictionary, object key)
    {
        return dictionary[key] ?? throw new KeyNotFoundException($"Key '{key}' not found in dictionary.");
    }

    public static TValue MustGet<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key)
    {
        if (!dictionary.TryGetValue(key, out var value))
            throw new KeyNotFoundException($"Key '{key}' not found in dictionary.");

        return value;
    }
}