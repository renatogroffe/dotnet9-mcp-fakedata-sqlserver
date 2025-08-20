using Bogus;
using Groffe.MsSql.FakeData.McpServer.Models;
using Groffe.MsSql.FakeData.McpServer.Tools;
using LinqToDB;
using LinqToDB.Data;
using Microsoft.Data.SqlClient;
using ModelContextProtocol.Server;
using System.ComponentModel;

/// <summary>
/// MCP para geracao de dados fake de Contatos em bases do SQL Server
/// </summary>
internal class ContatosFakeDataTools
{
    [McpServerTool]
    [Description("Popula (e se necessario cria) uma tabela incluindo na mesma dados fake de contatos.")]
    public async Task<Result> PopulateContatosTable(
        [Description("Quantidade de registros")] int numberOfRecords)
    {
        try
        {
            bool createdTable = false;
            var connectionString = Environment.GetEnvironmentVariable("CONNECTION_STRING")!;
            if (string.IsNullOrEmpty(connectionString))
            {
                return new Result
                {
                    IsSuccess = false,
                    Message = "A variável de ambiente CONNECTION_STRING não está definida."
                };
            }

            if (numberOfRecords <= 0)
            {
                return new Result
                {
                    IsSuccess = false,
                    Message = "A quantidade de registros deve ser maior que zero."
                };
            }

            if (numberOfRecords > 2000)
            {
                return new Result
                {
                    IsSuccess = false,
                    Message = "A quantidade de registros não pode ser maior que 2000."
                };
            }

            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using var commandTableExists = connection.CreateCommand();
                commandTableExists.CommandText = @"SELECT CASE WHEN EXISTS (SELECT * FROM sys.objects
                    WHERE object_id = OBJECT_ID(N'[dbo].[Contatos]') AND type in (N'U')) THEN 1 ELSE 0 END";
                if (!Convert.ToBoolean(await commandTableExists.ExecuteScalarAsync()))

                {
                    using var commandCreateTable = connection.CreateCommand();
                    commandCreateTable.CommandText = @"
                        CREATE TABLE [dbo].[Contatos](
                            [Id] [int] IDENTITY(1,1) NOT NULL,
                            [Nome] [varchar](100) NOT NULL,
                            [Telefone] [varchar](20) NOT NULL,
                            CONSTRAINT [PK_Contatos] PRIMARY KEY ([Id])
                        )";
                    await commandCreateTable.ExecuteNonQueryAsync();
                    createdTable = true;
                }
            }

            var db = new DataConnection(new DataOptions()
                .UseSqlServer(connectionString));
            var fakeContatos = new Faker<Contato>("pt_BR").StrictMode(false)
                        .RuleFor(c => c.Nome, f => f.Company.CompanyName())
                        .RuleFor(c => c.Telefone, f => f.Phone.PhoneNumber())
                        .Generate(numberOfRecords);
            await db.BulkCopyAsync<Contato>(fakeContatos);

            return new Result
            {
                IsSuccess = true,
                CreatedTable = createdTable,
                Message = $"{numberOfRecords} registro(s) inserido(s) na tabela dbo.Contatos com sucesso!"
            };
        }
        catch (Exception ex)
        {
            return new Result
            {
                IsSuccess = false,
                Message = $"Erro ao popular a tabela dbo.Contatos: {ex.Message}"
            };
        }
    }
}