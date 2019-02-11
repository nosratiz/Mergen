using System;
using System.Threading.Tasks;
using Mergen.Game.Api.Exceptions;
using Mergen.Game.Api.ViewModels;
using Mergen.Game.Api.ViewModels.Errors;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Mergen.Game.Api.Middlewares
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;
        private readonly JsonSerializerSettings _jsonSerializerSettings;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
            _jsonSerializerSettings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (ApiException ex)
            {
                await HandleExceptionAsync(httpContext, ex);
            }
            catch (Exception exception)
            {
                await HandleUnkownExceptionAsync(httpContext, exception);
            }
        }

        private Task HandleExceptionAsync(HttpContext context, ApiException exception)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = exception.StatusCode;
            return context.Response.WriteAsync(JsonConvert.SerializeObject(ApiResultViewModel<object>.FromError(
                    new ErrorViewModel
                        {ErrorCode = exception.ErrorCode, ErrorDescription = exception.ErrorDescription}),
                _jsonSerializerSettings));
        }

        private async Task HandleUnkownExceptionAsync(HttpContext httpContext, Exception exception)
        {
            _logger.LogError(exception, "Unhandled exception occurred in request {TraceIdentifier}",
                httpContext.TraceIdentifier);

            httpContext.Response.ContentType = "application/json";
            httpContext.Response.StatusCode = 500;
            await httpContext.Response.WriteAsync(JsonConvert.SerializeObject(ApiResultViewModel<object>.FromError(
                new InternalErrorViewModel(exception.ToString())), _jsonSerializerSettings));
        }
    }
}