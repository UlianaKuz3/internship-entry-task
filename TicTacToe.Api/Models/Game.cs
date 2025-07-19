using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace TicTacToe.Api.Models
{
    public class Game
    {
        [Key]
        public required Guid Id { get; init; }
        public required int BoardSize { get; init; }
        public required int WinLength { get; init; }
        [JsonIgnore]
        public ICollection<Move> Moves { get; init; } = [];
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public GameStatus Status { get; set; }
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public GameSymbole CurrentPlayer { get; set; }
    }
}
