using Microsoft.EntityFrameworkCore;
using TicTacToe.Api.Dto;
using TicTacToe.Api.Models;

namespace TicTacToe.Api.Services
{
    public class GameService(GameDbContext context,
                             ILogger<GameService> logger)
    {
        private readonly GameDbContext _context = context;
        private readonly ILogger<GameService> _logger = logger;


        public async Task<Game> CreateGameAsync(int boardSize, int winLength)
        {
            var board = new GameSymbole[boardSize, boardSize];

            for (int i = 0; i < boardSize; i++)
            {
                for (int j = 0; j < boardSize; j++)
                {
                    board[i, j] = GameSymbole.Empty;
                }
            }
            var game = new Game
            {
                Id = Guid.NewGuid(),
                BoardSize = boardSize,
                WinLength = winLength,
                Status = GameStatus.InProgress,
                CurrentPlayer = GameSymbole.X
            };

            _context.Games.Add(game);
            await _context.SaveChangesAsync();
            return game;
        }

        public async Task<Game?> GetGameAsync(Guid id)
        {
            return await _context.Games.Include(g => g.Moves).FirstOrDefaultAsync(g => g.Id == id);
        }

        public async Task<MoveResponseDTO> MakeMoveAsync(Guid gameId,
                                                      int row,
                                                      int column,
                                                      GameSymbole symbolPlayer)
        {
            try
            {

                var game = await _context.Games
                    .Include(g => g.Moves)
                    .AsTracking()
                    .FirstOrDefaultAsync(g => g.Id == gameId);

                if (game is null)
                    return new MoveResponseDTO
                    (
                        Success: false,
                        Message: "Game not found",
                        Status: game.Status
                    );


                if (game.Status != GameStatus.InProgress)
                    return new MoveResponseDTO
                    (
                        Success: false,
                        Message: "Game already completed",
                        Status: game.Status
                    ); 

                if (!IsInsideBoard(game, row, column))
                    return new MoveResponseDTO
                    (
                        Success: false,
                        Message: "Move out of bounds",
                        Status: game.Status
                    );

                var board = new GameSymbole[game.BoardSize, game.BoardSize];
                foreach (var m in game.Moves)
                {
                    board[m.Row, m.Column] = m.Player;
                }
                if (board[row, column] != GameSymbole.Empty)
                    return new MoveResponseDTO
                    (
                        Success: true,
                        Message: "Cell already occupied",
                        Status: game.Status
                    ); 

                GameSymbole symbol = game.CurrentPlayer;

                if (symbolPlayer != symbol)
                    return new MoveResponseDTO
                    (
                        Success: false,
                        Message: "Wrong player",
                        Status: game.Status
                    );

                if (game.Moves.Count > 0 && game.Moves.Count % 3 == 0)
                {
                    if (Random.Shared.NextDouble() <= 0.1)
                    {
                        symbol = symbol == GameSymbole.X ? GameSymbole.O : GameSymbole.X;
                        _logger.LogInformation("Move {Move} was flipped to {Symbol}", game.Moves.Count + 1, symbol);
                    }
                }

                var move = new Move
                {
                    Id = Guid.NewGuid(),
                    GameId = game.Id,
                    Row = row,
                    Column = column,
                    Symbol = symbol
                };

                _context.Moves.Add(move);
                game.Moves.Add(move);

                if (CheckWin(game, symbol))
                {
                    game.Status = symbol == GameSymbole.X ? GameStatus.WonX : GameStatus.WonO;
                }
                else if (game.Moves.Count == game.BoardSize * game.BoardSize)
                {
                    game.Status = GameStatus.Draw;
                }
                else
                {
                    game.CurrentPlayer = game.CurrentPlayer == GameSymbole.X ? GameSymbole.O : GameSymbole.X;
                }

                await _context.SaveChangesAsync();

                var response = new MoveResponseDTO
                (
                    Success: true,
                    Message: "Move accepted",
                    Status: game.Status
                );
                return response;
            }
            catch (DbUpdateConcurrencyException ex)
            {
                _logger.LogError(ex, "Concurrency error in MakeMoveAsync");
                return new MoveResponseDTO
                (
                    Success: false,
                    Message: "Game state was modified by another player. Please refresh.",
                    Status: GameStatus.InProgress
                );
            }
        }

        private static bool IsInsideBoard(Game game, int row, int column)
        {
            return row >= 0 && row < game.BoardSize && column >= 0 && column < game.BoardSize;
        }

        public static bool CheckWin(Game game, GameSymbole player)
        {
            var boardSize = game.BoardSize;
            var winLength = game.WinLength;
            var playerMoves = game.Moves
                .Where(m => m.Symbol == player) 
                .ToList();

            var board = new bool[boardSize, boardSize];
            foreach (var move in playerMoves)
            {
                board[move.Row, move.Column] = true;
            }

            for (int i = 0; i < boardSize; i++)
            {
                for (int j = 0; j < boardSize; j++)
                {
                    // Горизонтальная проверка
                    if (j <= boardSize - winLength)
                    {
                        bool win = true;
                        for (int k = 0; k < winLength; k++)
                        {
                            if (!board[i, j + k])
                            {
                                win = false;
                                break;
                            }
                        }
                        if (win) return true;
                    }
                    // Вертикальная проверка
                    if (i <= boardSize - winLength)
                    {
                        bool win = true;
                        for (int k = 0; k < winLength; k++)
                        {
                            if (!board[i + k, j])
                            {
                                win = false;
                                break;
                            }
                        }
                        if (win) return true;
                    }
                    // Диагональ (сверху вниз)
                    if (i <= boardSize - winLength && j <= boardSize - winLength)
                    {
                        bool win = true;
                        for (int k = 0; k < winLength; k++)
                        {
                            if (!board[i + k, j + k])
                            {
                                win = false;
                                break;
                            }
                        }
                        if (win) return true;
                    }
                    // Диагональ (снизу вверх)
                    if (i >= winLength - 1 && j <= boardSize - winLength)
                    {
                        bool win = true;
                        for (int k = 0; k < winLength; k++)
                        {
                            if (!board[i - k, j + k])
                            {
                                win = false;
                                break;
                            }
                        }
                        if (win) return true;
                    }
                }
            }

            return false;
        }

    }
}
