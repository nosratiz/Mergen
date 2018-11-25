﻿using System;
using Mergen.Admin.Api.ViewModels.Errors;

namespace Mergen.Admin.Api.Exceptions
{
    public abstract class ApiException : Exception
    {
        public int StatusCode { get; set; }
        public string ErrorCode { get; set; }
        public string ErrorDescription { get; set; }

        public virtual ErrorViewModel GetError()
        {
            return new ErrorViewModel
            {
                ErrorCode = ErrorCode,
                ErrorDescription = ErrorDescription
            };
        }

        protected ApiException(int statusCode, string errorCode, string errorDescription)
        {
            StatusCode = statusCode;
            ErrorCode = errorCode;
            ErrorDescription = errorDescription;
        }
    }
}