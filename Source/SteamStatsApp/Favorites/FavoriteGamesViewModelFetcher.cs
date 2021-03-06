﻿using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Trfc.SteamStats.ClientServices.AvailableGames;
using Trfc.SteamStats.ClientServices.GameFavorites;
using Trfc.SteamStats.ClientServices.GamePictures;
using Trfc.SteamStats.ClientServices.PlayerCount;

namespace SteamStatsApp.Favorites
{
    public sealed class FavoriteGamesViewModelFetcher : IFavoriteGamesViewModelFetcher
    {
        private readonly IAvailableGamesFetcher fetcher;
        private readonly IGameFavoriter favoriter;
        private readonly IFavoriteGameFetcher favoriteFetcher;
        private readonly IGamePictureFetcher pictureFetcher;
        private readonly IPlayerCountFetcher playerCountFetcher;

        public FavoriteGamesViewModelFetcher(IAvailableGamesFetcher fetcher,
            IFavoriteGameFetcher favoriteFetcher,
            IGameFavoriter favoriter,
            IGamePictureFetcher pictureFetcher,
            IPlayerCountFetcher playerCountFetcher)
        {
            this.fetcher = fetcher;
            this.favoriteFetcher = favoriteFetcher;
            this.favoriter = favoriter;
            this.pictureFetcher = pictureFetcher;
            this.playerCountFetcher = playerCountFetcher;
        }

        public async Task<IEnumerable<FavoriteGameViewModel>> FetchGameViewModelsAsync(CancellationToken token)
        {
            var response = await fetcher.FetchGamesAsync(token);
            var favoriteGames = await favoriteFetcher.GetFavoritedGames();

            IEnumerable<FavoriteGameViewModel> viewModels = Enumerable.Empty<FavoriteGameViewModel>();

            if (response.Successful)
            {
                viewModels = ConvertToGameViewModels(response.Games, favoriteGames);
            }            

            return viewModels;
        }

        private IEnumerable<FavoriteGameViewModel> ConvertToGameViewModels(IEnumerable<Game> allGames, IEnumerable<int> favoriteGames)
        {
            //TODO: This could be probably be made more efficient
            return allGames.Select(ConvertToViewModel(favoriteGames))
                .Where(game => game.IsFavorited)
                .ToList();
        }

        private System.Func<Game, FavoriteGameViewModel> ConvertToViewModel(IEnumerable<int> favoriteGames)
        {
            return game => new FavoriteGameViewModel(game.Name, game.Id, favoriteGames.Contains(game.Id), this.favoriter, this.pictureFetcher, this.playerCountFetcher);
        }
    }
}
