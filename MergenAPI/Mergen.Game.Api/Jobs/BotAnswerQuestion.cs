using System.Threading.Tasks;
using Mergen.Core.Data;
using Mergen.Core.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Quartz;

namespace Mergen.Game.Api.Jobs
{
    public class BotAnswerQuestion : IJob
    {

        private readonly ConnectionStringOption _connectionStringOption;

        public BotAnswerQuestion(IOptions<ConnectionStringOption> connectionStringOption)
        {
            _connectionStringOption = connectionStringOption.Value;
        }

        public Task Execute(IJobExecutionContext context)
        {
            var option = new DbContextOptionsBuilder<DataContext>();
            option.UseSqlServer(_connectionStringOption.Mergen);

            return Task.CompletedTask;
        }
    }
}