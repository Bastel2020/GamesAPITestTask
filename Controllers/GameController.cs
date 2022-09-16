using GamesAPITestTask.Interfaces.Repositories;
using GamesAPITestTask.Models.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GamesAPITestTask.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class GameController : ControllerBase
    {

        private readonly IGamesRepository _repo;

        public GameController(IGamesRepository repository)
        {
            _repo = repository;
        }

        /// <summary>
        /// Создать новую игру
        /// </summary>
        /// <param name="data">Данные о добавляемой игре. Нужно указать Id существующих жанра(ов) и разработчика ИЛИ названия существующих или новых жанра(ов) и разработчика.</param>
        /// <returns>Новую игру</returns>
        /// <response code="200">Успешное создание игры</response>
        /// <response code="400">Ошибка в запросе. Проверьте обязательные поля Name, DeveloperId или DeveloperName, GenresIds или GenreNames</response>
        /// <response code="500">Ошибка на стороне сервера</response>
        [HttpPost]
        public async Task<IActionResult> CreateGame([FromBody] GameDTO data)
        {
            var result = await _repo.CreateGame(data);
            if (result.Item1 == Enums.OperationResultEnum.Success)
                return Ok(result.Item2.ConvertToJsonCompatible());
            else if (result.Item1 == Enums.OperationResultEnum.NotFound)
                return NotFound();
            else if (result.Item1 == Enums.OperationResultEnum.WrongInput)
                return BadRequest();
            return StatusCode(500);
        }

        /// <summary>
        /// Получить игру по её Id
        /// </summary>
        /// <param name="id">Id игры для получения</param>
        /// <returns>Информацию о игре с указанным Id</returns>
        /// <response code="200">Игра с указанным Id найдена</response>
        /// <response code="404">Игра с указанным Id не найдена</response>
        /// <response code="500">Ошибка на стороне сервера</response>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetGame(int id)
        {
            var result = await _repo.GetGameById(id);
            if (result.Item1 == Enums.OperationResultEnum.Success)
                return Ok(result.Item2.ConvertToJsonCompatible());
            else if (result.Item1 == Enums.OperationResultEnum.NotFound)
                return NotFound();
            else if (result.Item1 == Enums.OperationResultEnum.WrongInput)
                return BadRequest();
            return StatusCode(500);
        }

        /// <summary>
        /// Изменить данные о игре с указанным Id
        /// </summary>
        /// <param name="id">Id игры для редактирования</param>
        /// <param name="data">Данные о изменяемой игре. Можно указать Id существующих жанра(ов) и разработчика ИЛИ названия существующих или новых жанра(ов) и разработчика.</param>
        /// <returns>Информацию о игре с новыми данными</returns>
        /// <response code="200">Информация о игре успешно отредактирована</response>
        /// <response code="400">Ошибка в запросе. Id разработчика и/или жанров не найдены.</response>
        /// <response code="404">Игра с указанным Id не найдена</response>
        /// <response code="500">Ошибка на стороне сервера</response>
        [HttpPut("{id}")]
        public async Task<IActionResult> EditGame(int id, [FromBody] GameDTO data)
        {
            var result = await _repo.EditGame(id, data);
            if (result.Item1 == Enums.OperationResultEnum.Success)
                return Ok(result.Item2.ConvertToJsonCompatible());
            else if (result.Item1 == Enums.OperationResultEnum.NotFound)
                return NotFound();
            else if (result.Item1 == Enums.OperationResultEnum.WrongInput)
                return BadRequest();
            return StatusCode(500);
        }

        /// <summary>
        /// Удалить игру с указанным Id
        /// </summary>
        /// <param name="id">Id игры для удаления</param>
        /// <returns>Статус операции</returns>
        /// <response code="200">Информация о игре удалаена</response>
        /// <response code="404">Игра с указанным Id не найдена</response>
        /// <response code="500">Ошибка на стороне сервера</response>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteGame(int id)
        {
            var result = await _repo.DeleteGame(id);
            if (result == Enums.OperationResultEnum.Success)
                return Ok();
            else if (result == Enums.OperationResultEnum.NotFound)
                return NotFound();
            else if (result == Enums.OperationResultEnum.WrongInput)
                return BadRequest();
            return StatusCode(500);
        }

        /// <summary>
        /// Получить список игр с указанным жанром
        /// </summary>
        /// <param name="IdOrGenreName">Id или название жанра</param>
        /// <returns>Статус операции</returns>
        /// <response code="200">Успешный поиск</response>
        /// <response code="500">Ошибка на стороне сервера</response>
        [HttpGet("GetByGenre/{IdOrGenreName}")]
        public async Task<IActionResult> GetGamesByGenre(string IdOrGenreName)
        {
            var result = await _repo.GetGamesByGenre(IdOrGenreName);
            if (result.Item1 == Enums.OperationResultEnum.Success)
                return Ok(result.Item2.Select(g => g.ConvertToJsonCompatible()));
            else if (result.Item1 == Enums.OperationResultEnum.NotFound)
                return NotFound();
            else if (result.Item1 == Enums.OperationResultEnum.WrongInput)
                return BadRequest();
            return StatusCode(500);
        }
    }
}
