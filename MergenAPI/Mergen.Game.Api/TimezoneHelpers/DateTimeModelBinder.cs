using System;
using System.Globalization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Mergen.Game.Api.TimezoneHelpers
{
    public class DateTimeOffsetModelBinder : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            bool nullable = bindingContext.ModelType == typeof(DateTime?)
                            || bindingContext.ModelType == typeof(DateTimeOffset?);

            var valueProviderResult = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);

            if (valueProviderResult == ValueProviderResult.None)
                return Task.CompletedTask;

            var value = valueProviderResult.FirstValue;

            if (value == null && !nullable)
                return Task.CompletedTask;
            if (!DateTimeOffset.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.None,
                out var dateTimeOffset))
            {
                bindingContext.ModelState.TryAddModelError(bindingContext.ModelName,
                    bindingContext.ModelMetadata.ModelBindingMessageProvider
                        .ValueIsInvalidAccessor(valueProviderResult.ToString()));

                return Task.CompletedTask;
            }

            if (bindingContext.ModelType == typeof(DateTime) || bindingContext.ModelType == typeof(DateTime?))
                bindingContext.Result = ModelBindingResult.Success(dateTimeOffset.UtcDateTime);
            else
                bindingContext.Result = ModelBindingResult.Success(dateTimeOffset);

            return Task.CompletedTask;
        }
    }
}