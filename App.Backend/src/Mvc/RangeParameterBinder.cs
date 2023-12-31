using App.Backend.Dto;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace App.Backend.Mvc;

internal class RangeParameterBinder : IModelBinder
{
	public Task BindModelAsync(ModelBindingContext bindingContext)
	{
		if (bindingContext == null)
		{
			throw new ArgumentNullException(nameof(bindingContext));
		}

		var modelName = bindingContext.ModelName;
		var valueProviderResult = bindingContext.ValueProvider.GetValue(modelName);

		if (valueProviderResult == ValueProviderResult.None)
		{
			return Task.CompletedTask;
		}

		bindingContext.ModelState.SetModelValue(modelName, valueProviderResult);

		var value = valueProviderResult.FirstValue;
		if (value == null)
		{
			bindingContext.ModelState.TryAddModelError(modelName, "Range must be in the format [from,to]");
			return Task.CompletedTask;
		}

		// Assuming the value is in the format "[from,to]"
		var rangeValues = value.TrimStart('[').TrimEnd(']').Split(',');

		if (rangeValues.Length != 2 || 
		    !int.TryParse(rangeValues[0], out var start) || 
		    !int.TryParse(rangeValues[1], out var end))
		{
			bindingContext.ModelState.TryAddModelError(modelName, "Range must be in the format [from,to]");
			return Task.CompletedTask;
		}

		var rangeParameter = new RangeParameter
		{
			Start = start,
			End = end
		};

		bindingContext.Result = ModelBindingResult.Success(rangeParameter);
		return Task.CompletedTask;
	}
}