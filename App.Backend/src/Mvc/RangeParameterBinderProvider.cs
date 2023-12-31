using App.Backend.Dto;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace App.Backend.Mvc;

internal class RangeParameterBinderProvider : IModelBinderProvider
{
	public IModelBinder? GetBinder(ModelBinderProviderContext context)
	{
		return context.Metadata.ModelType == typeof(RangeParameter) ? new RangeParameterBinder() : null;
	}
}