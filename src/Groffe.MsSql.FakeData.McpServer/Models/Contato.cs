using LinqToDB.Mapping;

namespace Groffe.MsSql.FakeData.McpServer.Models;

[Table("Contatos")]
public class Contato
{
    [PrimaryKey, Identity]
    public int Id { get; set; }

    [Column]
    public string? Nome { get; set; }

    [Column]
    public string? Telefone { get; set; }
}