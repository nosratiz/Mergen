﻿using Mergen.Core.QueryProcessing;

namespace Mergen.Game.Api.Security.AuthorizationSystem.PermissionParameterCheckers
{
    public class AccountIdQueryPermissionParameterChecker : QueryPermissionChecker
    {
        public AccountIdQueryPermissionParameterChecker(QueryInputModel value) : base("CreatorAccountId", value)
        {
        }

        public AccountIdQueryPermissionParameterChecker(QueryInputModel value, string queryFieldName) : base(
            "CreatorAccountId", value, queryFieldName)
        {
        }
    }
}