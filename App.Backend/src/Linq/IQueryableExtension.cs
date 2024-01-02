using App.Backend.Dto;

namespace App.Backend.Linq;

internal static class IQueryableExtension
{
    public static IQueryable<T> ApplyRange<T>(this IQueryable<T> query, RangeParameter range)
    {
        return query
            .Skip(range.Offset)
            .Take(range.Limit);
    }
}