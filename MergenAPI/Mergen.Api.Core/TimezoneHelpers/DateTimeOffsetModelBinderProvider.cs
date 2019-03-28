using System;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Mergen.Api.Core.TimezoneHelpers
{
    public class DateTimeOffsetModelBinderProvider : IModelBinderProvider
    {
        public IModelBinder GetBinder(ModelBinderProviderContext context)
        {
            if (context.Metadata.UnderlyingOrModelType == typeof(DateTime) ||
                context.Metadata.UnderlyingOrModelType == typeof(DateTime?) ||
                context.Metadata.UnderlyingOrModelType == typeof(DateTimeOffset) ||
                context.Metadata.UnderlyingOrModelType == typeof(DateTimeOffset?))
                return new DateTimeOffsetModelBinder();

            return null;
        }
    }
}