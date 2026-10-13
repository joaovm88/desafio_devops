using System.Collections.Concurrent;
using EcoCity.Api.Models;

namespace EcoCity.Api.Services;

/// <summary>Implementação em memória, usada nos testes automatizados.</summary>
public class InMemoryIndicadorRepository : IIndicadorRepository
{
    private readonly ConcurrentDictionary<string, Indicador> _dados = new();

    public Task<IReadOnlyList<Indicador>> ListarAsync(string? cidade = null)
    {
        IEnumerable<Indicador> q = _dados.Values;
        if (!string.IsNullOrWhiteSpace(cidade))
            q = q.Where(i => string.Equals(i.Cidade, cidade, StringComparison.OrdinalIgnoreCase));
        return Task.FromResult<IReadOnlyList<Indicador>>(q.ToList());
    }

    public Task<Indicador?> ObterAsync(string id) =>
        Task.FromResult(_dados.TryGetValue(id, out var i) ? i : null);

    public Task<Indicador> CriarAsync(Indicador indicador)
    {
        indicador.Id = Guid.NewGuid().ToString("N");
        _dados[indicador.Id] = indicador;
        return Task.FromResult(indicador);
    }

    public Task<bool> RemoverAsync(string id) => Task.FromResult(_dados.TryRemove(id, out _));
}
