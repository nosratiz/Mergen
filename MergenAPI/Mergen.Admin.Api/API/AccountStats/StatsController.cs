using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Mergen.Admin.Api.Helpers;
using Mergen.Api.Core.ViewModels;
using Mergen.Core.Managers;
using Mergen.Core.QueryProcessing;
using Microsoft.AspNetCore.Mvc;

namespace Mergen.Admin.Api.API.AccountStats
{
    [ApiController]
    public class StatsController : ApiControllerBase
    {
        private readonly StatsManager _statsManager;

        public StatsController(StatsManager statsManager)
        {
            _statsManager = statsManager;
        }

        [HttpGet]
        [Route("stats")]
        public async Task<ActionResult<ApiResultViewModel<IEnumerable<StatsViewModel>>>> GetAccountStatsAsync([FromQuery]QueryInputModel<StatsFilterInputModel> query, CancellationToken cancellationToken)
        {
            var stats = await _statsManager.GetAllAsync(query, cancellationToken);
            return OkData(StatsViewModel.MapAll(stats.Data), new DataMetaViewModel(stats.TotalCount));
        }
    }
}