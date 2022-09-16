using GamesAPITestTask.Enums;
using GamesAPITestTask.Interfaces.Models;
using GamesAPITestTask.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GamesAPITestTask.Interfaces.Repositories
{
    public interface IGamesRepository
    {
        public Task<Tuple<OperationResultEnum, IGameBase>> CreateGame(GameDTO data);
        public Task<Tuple<OperationResultEnum, IGameBase>> GetGameById(int gameId);
        public Task<Tuple<OperationResultEnum, IGameBase>> EditGame(int gameId, GameDTO newData);
        public Task<OperationResultEnum> DeleteGame(int gameId);
        public Task<Tuple<OperationResultEnum, IEnumerable<IGameBase>>> GetGamesByGenre(string IdOrGenreName);
    }
}
