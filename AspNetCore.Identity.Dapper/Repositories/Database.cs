using System.Data;
using System.Data.Common;
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
    
    /// <summary>
    /// Asynchronously connects to the SQL server using the provided connection string.
    /// If the connection is not already open, it will be opened.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation. The task result contains the SqlConnection object.</returns>
    public async Task<DbConnection> ConnectAsync() {
        var sqlConnection = new SqlConnection(_connectionString);
        if (sqlConnection.State != ConnectionState.Open) await sqlConnection.OpenAsync();
        return sqlConnection;
    }
}