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

    public static IQueryable<T> ApplyFilter<T>(this IQueryable<T> query, FilterParameter filter)
    {
        return query;
    }

    public static IQueryable<T> ApplySort<T>(this IQueryable<T> query, SortParameter sort)
    {
        return query;
    }
}