using System.Net;
using System.Net.Http.Json;
using EcoCity.Api.Models;
using EcoCity.Api.Services;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Xunit;

namespace EcoCity.Api.Tests;

public class ApiTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public ApiTests(WebApplicationFactory<Program> factory)
    {
        // Substitui o MongoDB por repositório em memória: testes não precisam de banco.
        _client = factory.WithWebHostBuilder(b => b.ConfigureServices(s =>
        {
            s.RemoveAll<IIndicadorRepository>();
            s.AddSingleton<IIndicadorRepository, InMemoryIndicadorRepository>();
        })).CreateClient();
    }

    [Fact]
    public async Task Health_RetornaOk() =>
        Assert.Equal(HttpStatusCode.OK, (await _client.GetAsync("/health")).StatusCode);

    [Fact]
    public async Task Post_IndicadorInvalido_Retorna400()
    {
        var r = await _client.PostAsJsonAsync("/api/indicadores",
            new Indicador { Cidade = "Rio", Nome = "CO2", Pilar = "Q", Valor = 10 });
        Assert.Equal(HttpStatusCode.BadRequest, r.StatusCode);
    }

    [Fact]
    public async Task Fluxo_CriarListarResumirRemover()
    {
        var post = await _client.PostAsJsonAsync("/api/indicadores",
            new Indicador { Cidade = "Curitiba", Nome = "Áreas verdes", Pilar = "e", Valor = 80 });
        Assert.Equal(HttpStatusCode.Created, post.StatusCode);
        var criado = (await post.Content.ReadFromJsonAsync<Indicador>())!;

        var lista = await _client.GetFromJsonAsync<List<Indicador>>("/api/indicadores?cidade=curitiba");
        Assert.Single(lista!);

        var resumo = await _client.GetFromJsonAsync<ResumoCidade>("/api/cidades/Curitiba/resumo");
        Assert.Equal(80, resumo!.ScoreGeral);

        Assert.Equal(HttpStatusCode.NoContent, (await _client.DeleteAsync($"/api/indicadores/{criado.Id}")).StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, (await _client.GetAsync($"/api/indicadores/{criado.Id}")).StatusCode);
    }

    [Fact]
    public async Task Resumo_CidadeSemDados_Retorna404() =>
        Assert.Equal(HttpStatusCode.NotFound, (await _client.GetAsync("/api/cidades/Inexistente/resumo")).StatusCode);
}
