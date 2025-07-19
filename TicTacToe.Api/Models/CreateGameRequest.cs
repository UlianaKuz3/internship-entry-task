using TicTacToe.Api.Dto;

namespace TicTacToe.Api.Models
{

    public class CreateGameRequest
    {
        public int BoardSize { get; init; } = 3;
        public int WinLength { get; init; } = 3;
    }
}
