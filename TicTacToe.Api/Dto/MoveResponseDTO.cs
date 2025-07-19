using TicTacToe.Api.Dto;
using TicTacToe.Api.Models;

namespace TicTacToe.Api.Dto
{
    public record MoveResponseDTO(bool Success, string Message, GameStatus Status);
}
