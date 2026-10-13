using EcoCity.Api.Models;
using EcoCity.Api.Services;
using MongoDB.Driver;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Cadeia de conexão vem de variável de ambiente (MongoDb__ConnectionString) - nunca no código.
var conn = builder.Configuration["MongoDb:ConnectionString"] ?? "mongodb://localhost:27017";
var dbName = builder.Configuration["MongoDb:Database"] ?? "ecocity";
builder.Services.AddSingleton<IMongoClient>(_ => new MongoClient(conn));
builder.Services.AddSingleton(sp => sp.GetRequiredService<IMongoClient>().GetDatabase(dbName));
builder.Services.AddSingleton<IIndicadorRepository, MongoIndicadorRepository>();

var app = builder.Build();

// Swagger habilitado em todos os ambientes para facilitar a evidência de staging/produção.
app.UseSwagger();
app.UseSwaggerUI();

app.MapGet("/", () => Results.Ok(new
{
    projeto = "EcoCity Analytics - API de Governança ESG",
    ambiente = app.Environment.EnvironmentName,
    swagger = "/swagger"
}));

// Liveness: não depende do banco.
app.MapGet("/health", () => Results.Ok(new { status = "ok" }));

app.MapGet("/api/indicadores", async (string? cidade, IIndicadorRepository repo) =>
    Results.Ok(await repo.ListarAsync(cidade)));

app.MapGet("/api/indicadores/{id}", async (string id, IIndicadorRepository repo) =>
    await repo.ObterAsync(id) is { } i ? Results.Ok(i) : Results.NotFound());

app.MapPost("/api/indicadores", async (Indicador novo, IIndicadorRepository repo) =>
{
    var erro = ScoreService.Validar(novo);
    if (erro is not null) return Results.BadRequest(new { erro });
    novo.Pilar = novo.Pilar.ToUpperInvariant();
    novo.Data = DateTime.UtcNow;
    var criado = await repo.CriarAsync(novo);
    return Results.Created($"/api/indicadores/{criado.Id}", criado);
});

app.MapDelete("/api/indicadores/{id}", async (string id, IIndicadorRepository repo) =>
    await repo.RemoverAsync(id) ? Results.NoContent() : Results.NotFound());

app.MapGet("/api/cidades/{cidade}/resumo", async (string cidade, IIndicadorRepository repo) =>
{
    var itens = await repo.ListarAsync(cidade);
    return itens.Count == 0 ? Results.NotFound() : Results.Ok(ScoreService.Resumir(cidade, itens));
});

app.Run();

public partial class Program { }
