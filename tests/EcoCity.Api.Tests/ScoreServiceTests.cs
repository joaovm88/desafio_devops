using EcoCity.Api.Models;
using EcoCity.Api.Services;
using Xunit;

namespace EcoCity.Api.Tests;

public class ScoreServiceTests
{
    private static Indicador Ind(string pilar, double valor) =>
        new() { Cidade = "São Paulo", Nome = "x", Pilar = pilar, Valor = valor };

    [Fact]
    public void Resumir_CalculaMediaPorPilarEScoreGeral()
    {
        var r = ScoreService.Resumir("São Paulo", new[] { Ind("E", 80), Ind("E", 60), Ind("S", 50), Ind("G", 90) });
        Assert.Equal(70, r.Ambiental);
        Assert.Equal(50, r.Social);
        Assert.Equal(90, r.Governanca);
        Assert.Equal(70, r.ScoreGeral);
    }

    [Fact]
    public void Resumir_IgnoraPilaresSemDados()
    {
        var r = ScoreService.Resumir("X", new[] { Ind("E", 40) });
        Assert.Null(r.Social);
        Assert.Equal(40, r.ScoreGeral);
    }

    [Theory]
    [InlineData("Z", 10)]
    [InlineData("E", -1)]
    [InlineData("E", 101)]
    public void Validar_RejeitaDadosInvalidos(string pilar, double valor) =>
        Assert.NotNull(ScoreService.Validar(Ind(pilar, valor)));

    [Fact]
    public void Validar_AceitaIndicadorValido() => Assert.Null(ScoreService.Validar(Ind("g", 55)));
}
