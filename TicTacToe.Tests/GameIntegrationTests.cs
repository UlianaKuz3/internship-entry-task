using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestPlatform.Utilities;
using System.Net;
using System.Net.Http.Json;
using TicTacToe.Api;
using TicTacToe.Api.Dto;
using TicTacToe.Api.Models;

namespace TicTacToe.Tests
{
    public class GameIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;
        private object _logger;

        public GameIntegrationTests(WebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient();

            using var scope = factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<GameDbContext>();
            db.Database.EnsureCreated();
        }

        [Fact]
        public async Task HealthEndpoint_ReturnsOk()
        {
            var response = await _client.GetAsync("/health");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }


        [Fact]
        public async Task CreateGame_WithValidParameters_ReturnsCreatedResult()
        {
            var request = new
            {
                BoardSize = 3,
                WinLength = 3
            };

            var response = await _client.PostAsJsonAsync("/game/create", request);
            response.EnsureSuccessStatusCode();

            Assert.Equal(HttpStatusCode.Created, response.StatusCode);

            var game = await response.Content.ReadFromJsonAsync<GameDto>();
            Assert.NotNull(game);
            Assert.Equal("InProgress", game.Status);
            Assert.NotNull(game.Id);
        }


        [Fact]
        public async Task InvalidJson_ReturnsBadRequest()
        {
            var request = new
            {
                BoardSize = 3,
                WinLength = 3
            };
            var createResponse = await _client.PostAsJsonAsync("/game/create", request);
            var game = await createResponse.Content.ReadFromJsonAsync<GameDto>();

            var json = new StringContent("{ invalid_json }", System.Text.Encoding.UTF8, "application/json");
            var response = await _client.PostAsync($"/game/{game.Id}/moves", json);

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task FullGame_LeadsToWinX()
        {
            var request = new
            {
                BoardSize = 3,
                WinLength = 3
            };
            var response = await _client.PostAsJsonAsync("/game/create", request);
            var game = await response.Content.ReadFromJsonAsync<GameDto>();
            var id = game.Id;

            var moves = new[]
            {
                new MoveRequestDTO(Row: 0, Column: 0, Symbole: GameSymbole.X),
                new MoveRequestDTO(Row: 1, Column: 1, Symbole: GameSymbole.O),
                new MoveRequestDTO(Row: 0, Column: 1, Symbole: GameSymbole.X),
                new MoveRequestDTO(Row: 2, Column: 2, Symbole: GameSymbole.O),
                new MoveRequestDTO(Row: 0, Column: 2, Symbole: GameSymbole.X)
            };

            foreach (var move in moves)
            {
                var res = await _client.PostAsJsonAsync($"/game/{id}/moves", move);
                res.EnsureSuccessStatusCode();
            }
            var finalGame = await _client.GetFromJsonAsync<Game>($"/game/{id}");
            Assert.True(finalGame.Status is GameStatus.WonX or GameStatus.InProgress);
        }

        [Fact]
        public async Task FullGame_LeadsToWinO()
        {
            var request = new
            {
                BoardSize = 3,
                WinLength = 3
            };
            var response = await _client.PostAsJsonAsync("/game/create", request);
            var game = await response.Content.ReadFromJsonAsync<GameDto>();
            var id = game.Id;

            var moves = new[]
            {
                new MoveRequestDTO(Row: 2, Column: 0, Symbole: GameSymbole.X),
                new MoveRequestDTO(Row: 0, Column: 0, Symbole: GameSymbole.O),
                new MoveRequestDTO(Row: 2, Column: 1, Symbole: GameSymbole.X),
                new MoveRequestDTO(Row: 0, Column: 1, Symbole: GameSymbole.O),
                new MoveRequestDTO(Row: 1, Column: 2, Symbole: GameSymbole.X),
                new MoveRequestDTO(Row: 0, Column: 2, Symbole: GameSymbole.O)
            };

            foreach (var move in moves)
            {
                var res = await _client.PostAsJsonAsync($"/game/{id}/moves", move);
                res.EnsureSuccessStatusCode();
            }
            var finalGame = await _client.GetFromJsonAsync<Game>($"/game/{id}");
            Assert.True(finalGame.Status is GameStatus.WonO or GameStatus.InProgress);
        }
        private class GameDto
        {
            public string Id { get; set; }
            public string[][] Board { get; set; }
            public string Status { get; set; }
        }
    }
}
