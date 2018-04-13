﻿using SteamStatsApp.Favorites;
using SteamStatsApp.Main;
using System.Collections.Generic;
using Trfc.ClientFramework;
using Trfc.SteamStats.ClientServices.AvailableGames;
using Trfc.SteamStats.ClientServices.GameFavorites;
using Xamarin.Forms;

namespace SteamStatsApp
{
    internal sealed class Bootstrapper
    {        
        //TODO: Abstract Application
        public static BootstrappedApplication Bootstrap(IToastMessageService toastMessageService, Application app)
        {
            CommandFactory.CommandFactoryInstance = new XamarinCommandFactory();

            var mainEndpoint = "https://steamstatsapi.herokuapp.com/";
            var configurationValues = new Dictionary<string, string>()
            {
                { "AvailableGames", $"{mainEndpoint}/api/v1.0/availablegames"}
            };
            var webGateway = new WebGateway();
            var configurationProvider = new ConfigurationProvider(configurationValues);
            var availableGameFetcher = new OnlineGameFetcher(configurationProvider, webGateway);

            var stringSerializer = new StringSerializer();
            var stringDeserializer = new StringDeserializer();
            var storageProvider = new StorageProvider<LocalGameFavorites.GameFavoritesDao>(app, stringDeserializer, stringSerializer);

            var gameFavoriter = new LocalGameFavorites(storageProvider, toastMessageService);

            var gameFetcher = new Main.GamesViewModelFetcher(availableGameFetcher, gameFavoriter, gameFavoriter);
            var viewModel = new MainPageViewModel(gameFetcher, gameFavoriter);

            var favoritesViewModelFetcher = new FavoriteGamesViewModelFetcher(availableGameFetcher, gameFavoriter, gameFavoriter);
            var favoritesViewModel = new FavoritesViewModel(favoritesViewModelFetcher, gameFavoriter);
            var favoritesView = new FavoritesView()
            {
                BindingContext = favoritesViewModel
            };

            var gameListView = new MainPage
            {
                BindingContext = viewModel
            };

            return new BootstrappedApplication(new View[] 
            {
                favoritesView,
                gameListView
            });
        }
    }
}