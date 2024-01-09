using Gridify;
using Microsoft.AspNetCore.Mvc;

namespace App.Backend.Mvc;

public class ListResult<T> : ActionResult
{
    private readonly IEnumerable<T> _items;
    private readonly string _resource;
    private readonly int _start;
    private readonly int _end;
    private readonly int _totalCount;

    public ListResult(
        IEnumerable<T> items,
        string resource,
        int start,
        int end,
        int totalCount)
    {
        _items = items;
        _resource = resource.Trim('/');
        _start = start;
        _end = end;
        _totalCount = totalCount;
    }

    public static ListResult<T> Create(
        IEnumerable<T> items,
        string resource,
        GridifyQuery query,
        int totalCount)
    {
        var page = int.Max(1, query.Page);
        var start = (page - 1) * query.PageSize;
        var end = (page - 1) * query.PageSize + items.Count();
        return new ListResult<T>(items, resource, start, end, totalCount);
    }

    public static ListResult<T> Create<TEntity>(
        string resource,
        GridifyQuery query,
        Paging<TEntity> paging,
        Func<TEntity, T> dtoToEntity)
    {
        var items = paging.Data.Select(dtoToEntity);
        var page = int.Max(1, query.Page);
        var start = (page - 1) * query.PageSize;
        var end = (page - 1) * query.PageSize + items.Count();
        return new ListResult<T>(items, resource, start, end, paging.Count);
    }

    public override async Task ExecuteResultAsync(ActionContext context)
    {
        var objectResult = new ObjectResult(_items)
        {
            StatusCode = StatusCodes.Status200OK
        };

        var end = int.Min(_end, _totalCount);
        context.HttpContext.Response.Headers.ContentRange = $"{_resource} {_start}-{end}/{_totalCount}";
        await objectResult.ExecuteResultAsync(context);
    }
}