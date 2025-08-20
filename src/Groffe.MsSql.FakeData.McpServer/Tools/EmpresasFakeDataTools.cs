using Bogus;
using Groffe.MsSql.FakeData.McpServer.Models;
using Groffe.MsSql.FakeData.McpServer.Tools;
using LinqToDB;
using LinqToDB.Data;
using Microsoft.Data.SqlClient;
using ModelContextProtocol.Server;
using System.ComponentModel;

/// <summary>
/// MCP para geracao de dados fake de Empresas em bases do SQL Server
/// </summary>
internal class EmpresasFakeDataTools
{
    [McpServerTool]
    [Description("Popula (e se necessario cria) uma tabela incluindo na mesma dados fake de empresas.")]
    public async Task<Result> PopulateEmpresasTable(
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

            if (numberOfRecords > 1000)
            {
                return new Result
                {
                    IsSuccess = false,
                    Message = "A quantidade de registros não pode ser maior que 1000."
                };
            }

            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using var commandTableExists = connection.CreateCommand();
                commandTableExists.CommandText = @"SELECT CASE WHEN EXISTS (SELECT * FROM sys.objects
                    WHERE object_id = OBJECT_ID(N'[dbo].[Empresas]') AND type in (N'U')) THEN 1 ELSE 0 END";
                if (!Convert.ToBoolean(await commandTableExists.ExecuteScalarAsync()))

                {
                    using var commandCreateTable = connection.CreateCommand();
                    commandCreateTable.CommandText = @"
                        CREATE TABLE [dbo].[Empresas](
                            [Id] [int] IDENTITY(1,1) NOT NULL,
                            [Nome] [varchar](100) NOT NULL,
                            [Cidade] [varchar](100) NOT NULL,
                            [Pais] [varchar](60) NOT NULL,
                            CONSTRAINT [PK_Empresas] PRIMARY KEY CLUSTERED ([Id] ASC)
                        )";
                    await commandCreateTable.ExecuteNonQueryAsync();
                    createdTable = true;
                }
            }

            var db = new DataConnection(new DataOptions()
                .UseSqlServer(connectionString));
            var fakeEmpresas = new Faker<Empresa>("pt_BR").StrictMode(false)
                        .RuleFor(e => e.Nome, f => f.Company.CompanyName())
                        .RuleFor(e => e.Cidade, f => f.Address.City())
                        .RuleFor(e => e.Pais, f => "Brasil")
                        .Generate(numberOfRecords);
            await db.BulkCopyAsync<Empresa>(fakeEmpresas);

            return new Result
            {
                IsSuccess = true,
                CreatedTable = createdTable,
                Message = $"{numberOfRecords} registro(s) inserido(s) na tabela dbo.Empresas com sucesso!"
            };
        }
        catch (Exception ex)
        {
            return new Result
            {
                IsSuccess = false,
                Message = $"Erro ao popular a tabela dbo.Empresas: {ex.Message}"
            };
        }
    }
}