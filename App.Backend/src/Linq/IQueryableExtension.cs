using System.Linq.Dynamic.Core;
using App.Backend.Dto;
using App.Backend.Exception;

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
        var typeProperties = typeof(T).GetProperties()
            .Select(p => p.Name)
            .ToHashSet();

        foreach (var filterParameter in filter.Keys)
        {
            if (!typeProperties.Contains(filterParameter))
            {
                throw new InvalidPropertyException(filterParameter, typeof(T));
            }

            var filterParameterValue = filter[filterParameter];
            if (filterParameterValue.StringValue != null)
            {
                query = query.Where($"{filterParameter} = @0", filterParameterValue.StringValue);
            }
            else if (filterParameterValue.IntValue != null)
            {
                query = query.Where($"{filterParameter} = @0", filterParameterValue.IntValue);
            }
            else if (filterParameterValue.StringCollectionValue != null)
            {
                query = query.Where($"{filterParameter} in @0", filterParameterValue.StringCollectionValue);
            }
            else if (filterParameterValue.IntCollectionValue != null)
            {
                query = query.Where($"{filterParameter} in @0", filterParameterValue.IntCollectionValue);
            }
            else
            {
                query = query.Where($"{filterParameter} is null");
            }
        }
        return query;
    }

    public static IQueryable<T> ApplySort<T>(this IQueryable<T> query, SortParameter sort)
    {
        var direction = sort.Direction == SortDirection.Asc ? "ascending" : "descending";
        try
        {
            var property = typeof(T).GetProperties()
                .Where(p => string.Equals(p.Name, sort.Field, StringComparison.OrdinalIgnoreCase))
                .First();
            return query.OrderBy($"{property.Name} {direction}");
        }
        catch (InvalidOperationException e)
        {
            throw new InvalidPropertyException(sort.Field, typeof(T), e);
        }
    }
}
