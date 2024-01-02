using System.Diagnostics.CodeAnalysis;

namespace App.Backend.Dto;

public class FilterParameter : Dictionary<string, FilterParameterValue>
{
    public bool TryGetStringValue(string param, [MaybeNullWhen(false)] out string result)
    {
        result = null;

        if (!TryGetValue(param, out var value))
            return false;

        if (value.StringValue == null)
            return false;

        result = value.StringValue;
        return true;
    }

    public bool TryGetStringCollectionValue(string param, [MaybeNullWhen(false)] out ICollection<string> result)
    {
        result = null;

        if (!TryGetValue(param, out var value))
            return false;

        if (value.StringCollectionValue == null)
            return false;

        result = value.StringCollectionValue;
        return true;
    }

    public bool TryGetIntValue(string param, [MaybeNullWhen(false)] out int result)
    {
        result = 0;
        if (!TryGetValue(param, out var value))
            return false;

        if (value.IntValue == null)
            return false;

        result = (int)value.IntValue;
        return true;
    }

    public bool TryGetIntCollectionValue(string param, [MaybeNullWhen(false)] out ICollection<int> result)
    {
        result = null;

        if (!TryGetValue(param, out var value))
            return false;

        if (value.IntCollectionValue == null)
            return false;

        result = value.IntCollectionValue;
        return true;
    }
}
