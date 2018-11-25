using Mergen.Admin.Api.CustomResult;
using Mergen.Admin.Api.Security;
using Mergen.Admin.Api.ViewModels;
using Mergen.Core.Entities;
using Mergen.Core.QueryProcessing;
using Microsoft.AspNetCore.Mvc;
using BadRequestResult = Mergen.Admin.Api.CustomResult.BadRequestResult;
using NotFoundResult = Mergen.Admin.Api.CustomResult.NotFoundResult;

namespace Mergen.Admin.Api.API
{
    public class ApiControllerBase : Controller
    {
        public int AccountId => (User as AccountPrincipal)?.AccountId ?? 0;
        public string AccountEmail => (User as AccountPrincipal)?.AccountEmail;

        public OkObjectResult OkData<TData>(TData data, object meta = null)
        {
            return Ok(ApiResultViewModel<TData>.FromData(data, meta));
        }

        public OkObjectResult OkData<TData>(QueryResult<TData> queryResult)
        {
            return Ok(ApiResultViewModel<TData>.FromData(queryResult.Data, new DataMetaViewModel(queryResult.TotalCount)));
        }

        public CreatedResult CreatedData<TData>(TData data, object meta = null)
        {
            return Created(string.Empty, ApiResultViewModel<TData>.FromData(data));
        }

        [NonAction]
        public ForbiddenResult Forbidden(string errorCode = "forbidden", string description = null)
        {
            return new ForbiddenResult(errorCode, description);
        }

        [NonAction]
        public BadRequestResult BadRequest(string errorCode, string description)
        {
            return new BadRequestResult(errorCode, description);
        }

        [NonAction]
        public NotFoundResult NotFound(string errorCode = "not_found", string description = null)
        {
            return new NotFoundResult(errorCode, description);
        }

        [NonAction]
        public new OkObjectResult Ok()
        {
            return new OkObjectResult("");
        }
    }
}