# MCP Server - Dados Fake no padrao brasileiro para SQL Server

Solucao de dados fake no padrao brasileiro para SQL Server, utilizando o Model Context Protocol (MCP) para interagir com Chats de IA. Desenvolvido por Renato Groffe.

Repo do projeto: https://github.com/renatogroffe/dotnet9-mcp-fakedata-sqlserver

Repo em que acontece o build deste package: https://github.com/groffedemos/mcp-sqlserver-fakedata-nuget

---

Implementação em .NET 9 de MCP Server para a geração de dados fake no padrão brasileiro em bases do SQL Server. Inclui o uso da biblioteca Bogus e de um script do Docker Compose para a montagem do ambiente de testes.

---

Para instalar este MCP execute a instrução a seguir em Windows, Linux ou macOS, em um ambiente que conte pelo menos com o .NET 9:

```bash
dotnet tool install --global FakeDataSqlSrvMcpServer --version 1.0.0
```

Para saber mais sobre o template de projeto .NET para MCP acesse: https://devblogs.microsoft.com/dotnet/mcp-server-dotnet-nuget-quickstart/

Este MCP pode ser utilizando em conjunto com o MCP Server do SQL Server. Criei uma versão do MCP do SQL Server que pode ser instalada como uma .NET Global Tool e que foi publicada no NuGet. Basta apenas executar a instrução a seguir em Windows, Linux ou macOS, em um ambiente que conte pelo menos com o .NET 8 previamente instalado:

```bash
dotnet tool install --global mcpsqlserver-preview-202508d --version 1.0.0
```

Para saber mais sobre o package que criei para o MCP do SQL Server acesse: https://github.com/renatogroffe/sqlserver-mcp-dotnet-tool-scripts

Exemplo de arquivo mcp.json do VS Code configurado para uso destes 2 MCPs:

```json
{
	"servers": {
		"mcp-sqlserver": {
            "type": "stdio",
            "command": "mcpsqlserver",
            "args": [],
            "env": {
                "CONNECTION_STRING": "Server=localhost;Database=BaseTestesMcp;User Id=sa;Password=SqlServer2025!;TrustServerCertificate=True;"
            }
        },
		"mcp-fakedata": {
            "type": "stdio",
            "command": "mcpfakedatasqlserver",
            "args": [],
            "env": {
                "CONNECTION_STRING": "Server=localhost;Database=BaseTestesMcp;User Id=sa;Password=SqlServer2025!;TrustServerCertificate=True;"
            }
        }
	},
	"inputs": []
}
```