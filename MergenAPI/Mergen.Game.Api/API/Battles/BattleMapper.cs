using AutoMapper;
using Mergen.Core.Entities;
using Mergen.Game.Api.API.Battles.ViewModels;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Mergen.Core.EntityIds;

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

            vm.CurrentTurnPlayerId = battle.Games.LastOrDefault(x => x.BattleId == battle.Id)?.CurrentTurnPlayerId;

            var lastgame = battle.Games.LastOrDefault(x => x.BattleId == battle.Id);

            if (lastgame != null)
            {
                vm.ShouldAnswer =
                    ((vm.CurrentTurnPlayerId == vm.Player1Id &&
                      lastgame.GameState == GameStateIds.Player1AnswerQuestions) ||
                     (vm.CurrentTurnPlayerId == vm.Player2Id && lastgame.GameState == GameStateIds.Player2AnswerQuestions));

                vm.ShouldSelectCategory =
                    ((vm.CurrentTurnPlayerId == vm.Player1Id && lastgame.GameState == GameStateIds.SelectCategory)) ||
                    ((vm.CurrentTurnPlayerId == vm.Player2Id && lastgame.GameState == GameStateIds.SelectCategory));
            }


            if (battle.Games != null)
                vm.Game = Mapper.Map<List<GameViewModel>>(battle.Games);

            vm.Player1MiniProfile = await _playerMiniProfileCache.GetMiniProfileAsync(battle.Player1Id, cancellationToken);

            if (battle.Player2Id.HasValue)
                vm.Player2MiniProfile = await _playerMiniProfileCache.GetMiniProfileAsync(battle.Player2Id.Value, cancellationToken);

            return vm;
        }

        public async ValueTask<IEnumerable<OneToOneBattleViewModel>> MapAllAsync(IEnumerable<OneToOneBattle> battles,
            CancellationToken cancellationToken)
        {
            var vms = new List<OneToOneBattleViewModel>();

            foreach (var oneToOneBattle in battles)
                vms.Add(await MapAsync(oneToOneBattle, cancellationToken));

            return vms;
        }
    }
}