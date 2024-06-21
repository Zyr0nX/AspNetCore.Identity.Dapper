using Microsoft.Data.SqlClient;

namespace AspNetCore.Identity.Dapper.Repositories;

public interface IDatabase
{
    Task<SqlConnection> ConnectAsync();
}