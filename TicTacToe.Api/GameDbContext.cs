using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using TicTacToe.Api.Models;

namespace TicTacToe.Api
{
    public class GameDbContext : DbContext
    {
        public DbSet<Game> Games => Set<Game>();
        public DbSet<Move> Moves => Set<Move>();

        public GameDbContext(DbContextOptions<GameDbContext> options) : base(options) { }
    }
}
