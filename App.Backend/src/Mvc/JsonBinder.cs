using System.Text.Json;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace App.Backend.Mvc;

internal class JsonBinder<T> : IModelBinder
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
			return Task.CompletedTask;

		bindingContext.ModelState.SetModelValue(modelName, valueProviderResult);

		var value = valueProviderResult.FirstValue;
		if (value == null)
			return Task.CompletedTask;

		try
		{
			var parameter = JsonSerializer.Deserialize<T>(value, JsonOptions.Options);
			if (parameter == null)
				return Task.CompletedTask;

			bindingContext.Result = ModelBindingResult.Success(parameter);
		}
		catch (JsonException exception)
		{
			bindingContext.ModelState.TryAddModelError(modelName, exception.Message);
		}

		return Task.CompletedTask;
	}
}