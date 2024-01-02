using App.Backend.Dto;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace App.Backend.Mvc;

internal class JsonBinderProvider : IModelBinderProvider
{
    public IModelBinder? GetBinder(ModelBinderProviderContext context)
    {
        return context.Metadata.ModelType switch
        {
            var type when type == typeof(InstitutionFilterParameter) => new JsonBinder<InstitutionFilterParameter>(),
            var type when type == typeof(RangeParameter) => new JsonBinder<RangeParameter>(),
            var type when type == typeof(SortParameter) => new JsonBinder<SortParameter>(),
            var type when type == typeof(FilterParameter) => new JsonBinder<FilterParameter>(),
            _ => null,
        };
    }
}