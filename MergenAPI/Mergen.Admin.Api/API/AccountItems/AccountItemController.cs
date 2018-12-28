using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Mergen.Admin.Api.API.AccountItems
{
    public class AccountItemController : ApiControllerBase
    {
        public async Task<ActionResult<AccountItemViewModel>> GetItems()
        {

        }
    }

    public class AccountItemViewModel
    {
        public string ItemTypeId { get; set; }

    }
}