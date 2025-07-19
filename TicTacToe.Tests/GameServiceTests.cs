using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using System.Threading.Tasks;
using TicTacToe.Api;
using TicTacToe.Api.Dto;
using TicTacToe.Api.Models;
using TicTacToe.Api.Services;

namespace TicTacToe.Tests
{
    public class GameServiceTests
    {
        private GameDbContext CreateSqliteContext()
        {
            var connection = new SqliteConnection("DataSource=:memory:");
            connection.Open();

            var options = new DbContextOptionsBuilder<GameDbContext>()
                .UseSqlite(connection)
                .Options;

            var context = new GameDbContext(options);
            context.Database.EnsureCreated();
            return context;
        }

        [Fact]
        public void CreateGame_ShouldWork()
        {
            var context = CreateSqliteContext();
            var logger = new Mock<ILogger<GameService>>();
            var service = new GameService(context, logger.Object);

            var game = service.CreateGameAsync(3, 3).Result;

            Assert.NotNull(game);
            Assert.Equal(3, game.BoardSize);
            Assert.Equal(GameStatus.InProgress, game.Status);
        }

        [Fact]
        public async Task MakeMove_ShouldUpdateBoard()
        {
            var connection = new SqliteConnection("DataSource=:memory:");
            connection.Open();

            var options = new DbContextOptionsBuilder<GameDbContext>()
                .UseSqlite(connection)
                .Options;

            using var context = new GameDbContext(options);
            context.Database.EnsureCreated();

            var logger = new Mock<ILogger<GameService>>();
            var service = new GameService(context, logger.Object);

            var game = await service.CreateGameAsync(3, 3);
            var result = await service.MakeMoveAsync(game.Id, 0, 0, GameSymbole.X);
        }


    }
}
