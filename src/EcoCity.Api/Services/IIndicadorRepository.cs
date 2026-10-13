using EcoCity.Api.Models;

namespace EcoCity.Api.Services;

public interface IIndicadorRepository
{
    Task<IReadOnlyList<Indicador>> ListarAsync(string? cidade = null);
    Task<Indicador?> ObterAsync(string id);
    Task<Indicador> CriarAsync(Indicador indicador);
    Task<bool> RemoverAsync(string id);
}
