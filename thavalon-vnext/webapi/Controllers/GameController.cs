using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using webapi.services;

namespace webapi.Controllers;

[ApiController]
public class GameController : ControllerBase
{

    private readonly IGameService _gameService;
    public GameController(
        IGameService gameService)
    {
        _gameService = gameService;
    }



    [HttpPost]
    [Route("creategame")]
    public Game CreateGame([FromBody] CreateGameDto gameDto)
    {
        var game = _gameService.CreateGame(gameDto.Players);
        return game;
    }

    [HttpGet]
    [Route("/{gameid}")]
    public Game GetGame(string gameId)
    {
        var game = _gameService.GetGame(gameId);
        return game;
    }

}

public class CreateGameDto
{
    public List<string> Players { get; set; }
}
