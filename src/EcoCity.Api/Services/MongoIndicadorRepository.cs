using EcoCity.Api.Models;
using MongoDB.Bson;
using MongoDB.Driver;

namespace EcoCity.Api.Services;

public class MongoIndicadorRepository : IIndicadorRepository
{
    private readonly IMongoCollection<Indicador> _col;

    public MongoIndicadorRepository(IMongoDatabase db) => _col = db.GetCollection<Indicador>("indicadores");

    public async Task<IReadOnlyList<Indicador>> ListarAsync(string? cidade = null)
    {
        var filtro = string.IsNullOrWhiteSpace(cidade)
            ? Builders<Indicador>.Filter.Empty
            : Builders<Indicador>.Filter.Regex(i => i.Cidade,
                new BsonRegularExpression($"^{System.Text.RegularExpressions.Regex.Escape(cidade)}$", "i"));
        return await _col.Find(filtro).ToListAsync();
    }

    public async Task<Indicador?> ObterAsync(string id) =>
        ObjectId.TryParse(id, out _) ? await _col.Find(i => i.Id == id).FirstOrDefaultAsync() : null;

    public async Task<Indicador> CriarAsync(Indicador indicador)
    {
        indicador.Id = null;
        await _col.InsertOneAsync(indicador);
        return indicador;
    }

    public async Task<bool> RemoverAsync(string id)
    {
        if (!ObjectId.TryParse(id, out _)) return false;
        var r = await _col.DeleteOneAsync(i => i.Id == id);
        return r.DeletedCount > 0;
    }
}
