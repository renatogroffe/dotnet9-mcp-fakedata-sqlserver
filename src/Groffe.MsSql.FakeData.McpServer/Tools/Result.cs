namespace Groffe.MsSql.FakeData.McpServer.Tools;

public class Result
{
    public bool? IsSuccess { get; set; }
    public bool CreatedTable { get; set; } = false;
    public string? Message { get; set; }
}