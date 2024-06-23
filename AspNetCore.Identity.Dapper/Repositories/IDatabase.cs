using System.Data.Common;

namespace AspNetCore.Identity.Dapper.Repositories;

public interface IDatabase
{
    Task<DbConnection> ConnectAsync();
}