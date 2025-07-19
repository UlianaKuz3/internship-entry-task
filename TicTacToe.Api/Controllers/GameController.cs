using Microsoft.AspNetCore.Mvc;
using TicTacToe.Api.Dto;
using TicTacToe.Api.Services;

namespace TicTacToe.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class GameController(GameService gameService) 
        : ControllerBase
    {
        private readonly GameService _gameService = gameService;

        [HttpGet("/health")]
        public IActionResult Health() => Ok("Healthy");

        [HttpPost("create")]
        public async Task<IActionResult> CreateGame([FromBody] CreateGameRequestDTO request)
        {
            if (request.BoardSize < 3 
                || request.WinLength < 3 
                || request.WinLength > request.BoardSize)
            {
                return BadRequest(new ProblemDetails
                {
                    Title = "Invalid board size or win condition",
                    Status = 400,
                    Detail = "BoardSize must be >= 3, WinLength must be >= 3 and <= BoardSize"
                });
            }

            var game = await _gameService.CreateGameAsync(request.BoardSize, request.WinLength);
            return CreatedAtAction(nameof(GetGame), new { game.Id }, game);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetGame(Guid id)
        {
            var game = await _gameService.GetGameAsync(id);
            if (game == null)
                return NotFound();

            return Ok(game);
        }

        [HttpPost("{id:guid}/moves")]
        public async Task<IActionResult> MakeMove(Guid id, [FromBody] MoveRequestDTO request)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ProblemDetails
                {
                    Title = "Invalid request",
                    Status = 400,
                    Detail = "Request must contain valid row and column"
                });

            MoveResponseDTO response = await _gameService.MakeMoveAsync(id, request.Row, request.Column, request.Symbole);
            if (!response.Success)
            {
                return BadRequest(new ProblemDetails
                {
                    Title = "Invalid move",
                    Status = 400,
                    Detail = response.Message
                });
            }

            return Ok(response);
        }
    }
}
