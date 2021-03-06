﻿using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Trfc.ClientFramework;

namespace Trfc.SteamStats.ClientServices.AvailableGames
{
    public sealed class OnlineGameFetcher : IAvailableGamesFetcher
    {
        private readonly string connectionStringKey = "AvailableGames";

        private readonly IConfigurationProvider configurationProvider;
        private readonly IWebGateway webGateway;

        public OnlineGameFetcher(IConfigurationProvider configurationProvider,
            IWebGateway webGateway)
        {
            this.configurationProvider = configurationProvider;
            this.webGateway = webGateway;
        }

        public async Task<AvailableGamesResponse> FetchGamesAsync(CancellationToken token)
        {
            var endpoint = configurationProvider.GetConnectionStringById(connectionStringKey);

            var response = await webGateway.GetResponseFromEndpoint<ResponseDao>(endpoint, token);

            if (response.Succeeded)
            {
                var games = response.Value.AvailableGames.Select(ConvertGameDao).ToList();
                return AvailableGamesResponse.Succeeded(games);
            }

            return AvailableGamesResponse.Failed(response.ResultMessage);
        }

        private Game ConvertGameDao(GameDao gameDao)
        {
            return new Game()
            {
                Id = gameDao.Id,
                Name = gameDao.Name
            };
        }

        [Preserve(AllMembers = true)]
        private sealed class ResponseDao
        {
            public List<GameDao> AvailableGames { get; }
            public ResponseDao(List<GameDao> availableGames)
            {
                AvailableGames = availableGames;
            }
        }

        [Preserve(AllMembers = true)]
        private sealed class GameDao
        {            
            public string Name { get; }
            public int Id { get; }
            public GameDao(string name, int id)
            {
                Name = name;
                Id = id;
            }
        }
    }
}
