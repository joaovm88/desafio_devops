using EcoCity.Api.Models;

namespace EcoCity.Api.Services;

public static class ScoreService
{
    public static readonly string[] Pilares = { "E", "S", "G" };

    public static string? Validar(Indicador i)
    {
        if (string.IsNullOrWhiteSpace(i.Cidade)) return "Cidade é obrigatória.";
        if (string.IsNullOrWhiteSpace(i.Nome)) return "Nome é obrigatório.";
        if (!Pilares.Contains(i.Pilar?.ToUpperInvariant())) return "Pilar deve ser E, S ou G.";
        if (i.Valor < 0 || i.Valor > 100) return "Valor deve estar entre 0 e 100.";
        return null;
    }

    /// <summary>Média por pilar; score geral = média dos pilares que possuem dados.</summary>
    public static ResumoCidade Resumir(string cidade, IEnumerable<Indicador> indicadores)
    {
        var lista = indicadores.ToList();
        double? Media(string p)
        {
            var v = lista.Where(i => i.Pilar.Equals(p, StringComparison.OrdinalIgnoreCase)).Select(i => i.Valor).ToList();
            return v.Count == 0 ? null : Math.Round(v.Average(), 2);
        }
        var e = Media("E"); var s = Media("S"); var g = Media("G");
        var presentes = new[] { e, s, g }.Where(x => x.HasValue).Select(x => x!.Value).ToList();
        var geral = presentes.Count == 0 ? 0 : Math.Round(presentes.Average(), 2);
        return new ResumoCidade(cidade, e, s, g, geral);
    }
}
