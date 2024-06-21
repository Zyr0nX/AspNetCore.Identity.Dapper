using System.Data;
using Microsoft.Data.SqlClient;

namespace AspNetCore.Identity.Dapper.Repositories;

public class Database : IDatabase
{
    private readonly string _connectionString;

    public Database(string? connectionString, string schema)
    {
        var replace = schema.Replace("[", string.Empty).Replace("]", string.Empty);
        _connectionString = connectionString ?? string.Empty;
    }

    public async Task<SqlConnection> ConnectAsync() {
        var sqlConnection = new SqlConnection(_connectionString);
        if (sqlConnection.State != ConnectionState.Open) await sqlConnection.OpenAsync();
        return sqlConnection;
    }
}