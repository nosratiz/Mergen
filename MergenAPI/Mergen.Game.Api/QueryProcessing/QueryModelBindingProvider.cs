using Mergen.Core.QueryProcessing;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Mergen.Game.Api.QueryProcessing
{
    public class QueryModelBindingProvider : IModelBinderProvider
    {
        public IModelBinder GetBinder(ModelBinderProviderContext context)
        {
            if (!typeof(QueryInputModel).IsAssignableFrom(context.Metadata.ModelType))
                return null;

            var inputProcessor = (InputProcessor) context.Services.GetService(typeof(InputProcessor));
            return new QueryModelBinder(inputProcessor);
        }
    }
}