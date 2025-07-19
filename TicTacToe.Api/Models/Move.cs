using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TicTacToe.Api.Models
{
    public class Move
    {
        [Key]
        public required Guid Id { get; init; }
        public required Guid GameId { get; init; }

        [ForeignKey(nameof(GameId))]
        public Game Game { get; init; } = null!;
        public required int Row { get; init; }
        public required int Column { get; init; }
        public required GameSymbole Symbol { get; init; }
        public GameSymbole Player { get; internal set; }
    }
}
