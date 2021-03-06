﻿using Bundesliga.Api.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;

namespace Bundesliga.Api
{
    public class BundesligaService : IBundesligaService
    {
        private readonly IBundesligaContextService _bundesligaContextService;

        public BundesligaService(IBundesligaContextService bundesligaContextService)
        {
            _bundesligaContextService = bundesligaContextService;
        }

        public List<Team> GetAllTeams()
        {
            return _bundesligaContextService.GetAllTeams();
        }

        public Game AddGame(Game game)
        {
            var gamesInSpecificStage = _bundesligaContextService.GetGamesByStage(game.Stage);
            if (gamesInSpecificStage.Any(x => x.Team1Id == game.Team1Id || x.Team2Id == game.Team1Id || x.Team1Id == game.Team2Id || x.Team2Id == game.Team2Id))
            {
                ThrowFault("Cannot insert multiple games for the same team in one stage.");
            }

            if (game.Team1Id == game.Team2Id)
            {
                ThrowFault("Invalid game. A team cannot play itself");
            }

            return _bundesligaContextService.InsertGame(game);
        }

        public void RemoveGame(string id)
        {
            int gameId;
            if (!int.TryParse(id, out gameId))
            {
                ThrowFault("Invalid game Id");
            }
            _bundesligaContextService.RemoveGame(gameId);
        }

        public List<Game> GetAllGames()
        {
            return _bundesligaContextService.GetAllGames();
        }

        private static void ThrowFault(string errorMessage, Exception innerException = null)
        {
            var igf = new InvalidGameFault
            {
                ErrorMessage = errorMessage,
                InnerException = innerException
            };
            throw new FaultException<InvalidGameFault>(igf);
        }

        public int gameId { get; set; }
    }
}
