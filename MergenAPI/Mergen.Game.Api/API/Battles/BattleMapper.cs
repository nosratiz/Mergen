using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Mergen.Core.Data;

namespace Mergen.Game.Api.API.Battles
{
    public class BattleMapper
    {
        private readonly PlayerMiniProfileCache _playerMiniProfileCache;

        public BattleMapper(PlayerMiniProfileCache playerMiniProfileCache)
        {
            _playerMiniProfileCache = playerMiniProfileCache;
        }

        public async ValueTask<OneToOneBattleViewModel> MapAsync(OneToOneBattle battle, CancellationToken cancellationToken)
        {
            var vm = Mapper.Map<OneToOneBattleViewModel>(battle);

            vm.Player1MiniProfile = await _playerMiniProfileCache.GetMiniProfileAsync(battle.Player1Id, cancellationToken);

            if (battle.Player2Id.HasValue)
                vm.Player2MiniProfile = await _playerMiniProfileCache.GetMiniProfileAsync(battle.Player1Id, cancellationToken);

            return vm;
        }
    }
}