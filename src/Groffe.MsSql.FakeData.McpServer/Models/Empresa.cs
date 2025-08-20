using LinqToDB.Mapping;

namespace Groffe.MsSql.FakeData.McpServer.Models;

[Table("Empresas")]
public class Empresa
{
    [PrimaryKey, Identity]
    public int Id { get; set; }

    [Column]
    public string? Nome { get; set; }

    [Column]
    public string? Cidade { get; set; }

    [Column]
    public string? Pais { get; set; }
}