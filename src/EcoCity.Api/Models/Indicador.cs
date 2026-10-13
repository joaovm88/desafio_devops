using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace EcoCity.Api.Models;

/// <summary>Indicador ESG de uma cidade (Environmental, Social ou Governance).</summary>
public class Indicador
{
    [BsonId, BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }
    public string Cidade { get; set; } = string.Empty;
    /// <summary>E, S ou G.</summary>
    public string Pilar { get; set; } = string.Empty;
    public string Nome { get; set; } = string.Empty;
    /// <summary>Pontuação de 0 a 100.</summary>
    public double Valor { get; set; }
    public DateTime Data { get; set; } = DateTime.UtcNow;
}

public record ResumoCidade(string Cidade, double? Ambiental, double? Social, double? Governanca, double ScoreGeral);
