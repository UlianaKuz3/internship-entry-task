using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using TicTacToe.Api;
using TicTacToe.Api.Dto;
using TicTacToe.Api.Models;
using TicTacToe.Api.Services;


var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<HttpsRedirectionOptions>(options =>
{
    options.HttpsPort = null;
});

var boardSize = int.Parse(Environment.GetEnvironmentVariable("GAME_BOARD_SIZE") ?? "3");
var winLength = int.Parse(Environment.GetEnvironmentVariable("GAME_WIN_LENGTH") ?? "3");


builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });
builder.Services.AddOpenApi();

builder.Services.AddDbContext<GameDbContext>(opt =>
    opt.UseSqlite("Data Source=games.db"));

builder.Services.AddScoped<GameService>();

var app = builder.Build();

app.UseExceptionHandler("/error");
app.MapGet("/error", () => "Произошла ошибка! Проверьте логи сервера.");

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}



app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

//app.Use(async (context, next) =>
//{
//    Console.WriteLine($"{context.Request.Method} {context.Request.Path}");
//    await next();
//});

app.Run();

//
public partial class Program { }
