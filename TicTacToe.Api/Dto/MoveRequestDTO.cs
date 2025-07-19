using TicTacToe.Api.Dto;
using TicTacToe.Api.Models;

namespace TicTacToe.Api.Dto
{
    public record MoveRequestDTO(int Row, int Column, GameSymbole Symbole);
}
