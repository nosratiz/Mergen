using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Mergen.Core.Data;
using Mergen.Game.Api.API.Battles.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace Mergen.Game.Api.API.Battles
{
    public class PlayerMiniProfileCache
    {
        private readonly DataContext _context;
        private static readonly ConcurrentDictionary<long, PlayerMiniProfileViewModel> Profiles = new ConcurrentDictionary<long, PlayerMiniProfileViewModel>();

        public PlayerMiniProfileCache(DataContext context)
        {
            _context = context;
        }

        public async Task<PlayerMiniProfileViewModel> GetMiniProfileAsync(long playerId, CancellationToken cancellationToken)
        {
            if (!Profiles.TryGetValue(playerId, out var miniProfile))
            {
                var account = await _context.Accounts.FirstOrDefaultAsync(q => q.Id == playerId, cancellationToken);
                var stats = await _context.AccountStatsSummaries.FirstOrDefaultAsync(q => q.AccountId == playerId, cancellationToken);
                miniProfile = new PlayerMiniProfileViewModel(account.Nickname, stats?.Level ?? 0,account.AvatarImageId);
                Profiles.TryAdd(playerId, miniProfile);
            }

            return miniProfile;
        }
    }
}