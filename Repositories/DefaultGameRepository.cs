using GamesAPITestTask.Database;
using GamesAPITestTask.Enums;
using GamesAPITestTask.Interfaces.Models;
using GamesAPITestTask.Interfaces.Repositories;
using GamesAPITestTask.Models;
using GamesAPITestTask.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GamesAPITestTask.Repositories
{
    public class DefaultGameRepository<T, T2, T3> : IGamesRepository
        where T : class, IGameBase
        where T2 : class, IDeveloperBase
        where T3 : class, IGenreBase
    {
        private DefaultDBContext<T, T2, T3> _db;

        public DefaultGameRepository(DefaultDBContext<T, T2, T3> context)
        {
            _db = context;
        }
        public async Task<Tuple<OperationResultEnum, IGameBase>> CreateGame(GameDTO data)
        {
            if (data.Name == null || data.Name.Length == 0 ||
                ((data.DeveloperName == null || data.DeveloperName.Length == 0) && data.DeveloperId == 0) ||
                ((data.GenresNames == null || data.GenresNames.Length == 0) && (data.GenresIds == null || data.GenresIds.Length == 0)))
                return new Tuple<OperationResultEnum, IGameBase>(OperationResultEnum.WrongInput, null);

            var newEntity = new DefaultGame();

            newEntity.Name = data.Name;

            if (data.DeveloperId != 0)
            {
                var developer = _db.GetDevelopers().FirstOrDefault(d => d.Id == data.DeveloperId);
                if (developer == null)
                    return new Tuple<OperationResultEnum, IGameBase>(OperationResultEnum.WrongInput, null);
                newEntity.Developer = developer;
            }
            else
            {
                var developer = _db.GetDevelopers().FirstOrDefault(d => d.Name == data.DeveloperName);
                if (developer == null)
                {
                    developer = new DefaultDeveloper();
                    developer.Name = data.DeveloperName;

                    await _db.AddDeveloperAsync(developer);
                }

                newEntity.Developer = developer;
            }

            var genres = _db.GetGenres();
            if (data.GenresIds != null && data.GenresIds.Length != 0)
            {
                var currentGenres = data.GenresIds
                    .Select(id => genres.FirstOrDefault(gen => gen.Id == id))
                    .ToList();

                currentGenres = currentGenres.Distinct().ToList(); // удаляем дубликаты, если есть

                if (currentGenres.Any(g => g == null))
                    return new Tuple<OperationResultEnum, IGameBase>(OperationResultEnum.WrongInput, null);

                newEntity.Genres = currentGenres;
            }
            else
            {
                var currentGenres = data.GenresNames
                    .Select(async (name) =>
                    {
                        var result = genres.FirstOrDefault(gen => gen.Name == name);
                        if (result == null)
                        {
                            result = new DefaultGenre();
                            result.Name = name;

                            await _db.AddGenreAsync(result);                        
                        }
                        return result;
                    })
                    .Select(gen => gen.Result)
                    .ToList();
                newEntity.Genres = currentGenres;
            }

            await _db.AddGameAsync(newEntity);
            await _db.SaveChangesAsync();
            return Tuple.Create(OperationResultEnum.Success, newEntity as IGameBase);
        }

        public async Task<OperationResultEnum> DeleteGame(int gameId)
        {
            var allGames = _db.GetGames();
            var gameToDelete = allGames.FirstOrDefault(g => g.Id == gameId);

            if (gameToDelete == null)
                return OperationResultEnum.NotFound;

            _db.Remove(gameToDelete);
            await _db.SaveChangesAsync();

            return OperationResultEnum.Success;
        }

        public async Task<Tuple<OperationResultEnum, IGameBase>> EditGame(int gameId, GameDTO newData)
        {
            var gameToEdit = _db.GetGames().FirstOrDefault(g => g.Id == gameId);

            if (gameToEdit == null)
                return new Tuple<OperationResultEnum, IGameBase>(OperationResultEnum.NotFound, null);

            if (newData.Name != null && newData.Name.Length > 0)
            {
                gameToEdit.Name = newData.Name;
            }

            if (newData.DeveloperId != 0)
            {
                var newDeveloper = _db.GetDevelopers().FirstOrDefault(d => d.Id == newData.DeveloperId);
                if (newDeveloper == null)
                    return new Tuple<OperationResultEnum, IGameBase>(OperationResultEnum.WrongInput, null);

                gameToEdit.Developer = newDeveloper;
            }
            else if (newData.DeveloperName != null && newData.DeveloperName.Length > 0)
            {
                var newDeveloper = _db.GetDevelopers().FirstOrDefault(d => d.Name == newData.DeveloperName);
                if (newDeveloper == null)
                {
                    newDeveloper = new DefaultDeveloper();
                    newDeveloper.Name = newData.DeveloperName;

                    gameToEdit.Developer = newDeveloper;

                    await _db.AddAsync(newDeveloper);
                }

                gameToEdit.Developer = newDeveloper;
            }

            if (newData.GenresIds != null && newData.GenresIds.Length > 0)
            {
                var allGenres = _db.GetGenres();
                var newGenres = newData.GenresIds.Select(id => allGenres.FirstOrDefault(g => g.Id == id)).ToList();
                if (newGenres.Any(g => g == null))
                    return new Tuple<OperationResultEnum, IGameBase>(OperationResultEnum.WrongInput, null);

                gameToEdit.Genres = newGenres;
            }
            else if (newData.GenresNames != null && newData.GenresNames.Length > 0)
            {
                var allGenres = _db.GetGenres();
                var newGenres = newData.GenresNames
                    .Select(async (gName) =>
                    {
                        var genre = allGenres.Where(g => g.Name == gName).FirstOrDefault();
                        if (genre == null)
                        {
                            genre = new DefaultGenre();
                            genre.Name = gName;

                            await _db.AddAsync(genre);
                        }
                        return genre;
                    });

                gameToEdit.Genres = newGenres.Select(g => g.Result).ToList();
            }

            await _db.SaveChangesAsync();

            return Tuple.Create(OperationResultEnum.Success, gameToEdit);
        }

        public async Task<Tuple<OperationResultEnum, IGameBase>> GetGameById(int gameId)
        {
            var result = _db.GetGames().FirstOrDefault(g => g.Id == gameId);
            if (result == null)
                return new Tuple<OperationResultEnum, IGameBase>(OperationResultEnum.NotFound, null);
            return new Tuple<OperationResultEnum, IGameBase>(OperationResultEnum.Success, result);
        }

        public async Task<Tuple<OperationResultEnum, IEnumerable<IGameBase>>> GetGamesByGenre(string IdOrGenreName)
        {
            if (IdOrGenreName == null || IdOrGenreName.Length == 0)
                return new Tuple<OperationResultEnum, IEnumerable<IGameBase>>(OperationResultEnum.WrongInput, null);

            int parsedId;
            IGenreBase result; 
            if (int.TryParse(IdOrGenreName, out parsedId))
                result = _db.GetGenres().FirstOrDefault(g => g.Id == parsedId);
            else
                result = _db.GetGenres().FirstOrDefault(g => g.Name == IdOrGenreName);

            if (result == null)
                return new Tuple<OperationResultEnum, IEnumerable<IGameBase>>(OperationResultEnum.NotFound, null);
            return Tuple.Create(OperationResultEnum.Success, result.Games);
        }
    }
}
