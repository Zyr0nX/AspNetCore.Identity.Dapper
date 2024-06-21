using System.Data;
using System.Text;
using AspNetCore.Identity.Dapper.Repositories;
using Microsoft.AspNetCore.Identity;
using Dapper;

namespace AspNetCore.Identity.Dapper.PostgreSQL;

public class UsersProvider<TUser, TKey> : IUsersProvider<TUser, TKey>
    where TUser : IdentityUser<TKey>
    where TKey : IEquatable<TKey>
{
    private readonly IDatabase _database;
    private readonly IdentityErrorDescriber _errorDescriber;

    public UsersProvider(IDatabase database, IdentityErrorDescriber errorDescriber)
    {
        _errorDescriber = errorDescriber;
        _database = database;
    }

    public async Task CreateAsync(TUser user)
    {
        await using var connection = await _database.ConnectAsync();
        var paramNames = user.GetType().GetProperties().Select(p => p.Name).ToList();
        var cols = string.Join(", ", paramNames);
        var colsParams = string.Join(", ", paramNames.Select(p => "@" + p));
        
        var sql = $"INSERT INTO AspNetUsers ({cols}) VALUES ({colsParams})";
        var result = await connection.ExecuteAsync(sql, user);
        if (result == 0) throw new DBConcurrencyException();
    }

    public async Task UpdateAsync(TUser user)
    {
        var paramNames = user.GetType().GetProperties().Select(p => p.Name).ToList();
        var builder = new StringBuilder();
        builder.Append("UPDATE AspNetUsers SET ");
        builder.Append(string.Join(", ", paramNames.Where(n => n != "Id" && n != "ConcurrencyStamp").Select(p => p + "= @" + p)));
        builder.Append("ConcurrencyStamp = @NewConcurrencyStamp WHERE Id = @Id AND ConcurrencyStamp = @ConcurrencyStamp");

        var sql = builder.ToString();
        var parameters = new DynamicParameters(user);
        parameters.Add("NewConcurrencyStamp", Guid.NewGuid().ToString());
        
        await using var connection = await _database.ConnectAsync();
        var result = await connection.ExecuteAsync(sql, parameters);
        if (result == 0) throw new DBConcurrencyException();
    }

    public async Task DeleteAsync(TUser user)
    {
        const string sql = "DELETE FROM AspNetUsers WHERE Id = @Id AND ConcurrencyStamp = @ConcurrencyStamp";

        var parameters = new DynamicParameters();
        parameters.Add("Id", user.Id);
        parameters.Add("ConcurrencyStamp", user.ConcurrencyStamp);

        await using var connection = await _database.ConnectAsync();
        var result = await connection.ExecuteAsync(sql, parameters);
        if (result == 0) throw new DBConcurrencyException();

    }
    
    public async Task<TUser?> FindByIdAsync(string userId)
    {
        var paramNames = typeof(TUser).GetProperties().Select(p => p.Name).ToList();
        var builder = new StringBuilder("SELECT TOP 1 ");
        builder.Append(string.Join(", ", paramNames));
        builder.Append(string.Join(", ", paramNames));
        builder.Append(" FROM AspNetUsers WHERE Id = @Id");

        var sql = builder.ToString();
    
        await using var connection = await _database.ConnectAsync();
        var result = await connection.QueryFirstOrDefaultAsync<TUser>(sql, new { Id = userId });
        return result;
    }
    
    public async Task<TUser?> FindByNameAsync(string normalizedUserName)
    {
        var paramNames = typeof(TUser).GetProperties().Select(p => p.Name).ToList();
        var builder = new StringBuilder("SELECT TOP 1 ");
        builder.Append(string.Join(", ", paramNames));
        builder.Append(string.Join(", ", paramNames));
        builder.Append(" FROM AspNetUsers WHERE NormalizedUserName = @NormalizedUserName");
        
        var sql = builder.ToString();
        
        await using var connection = await _database.ConnectAsync();
        var result = await connection.QueryFirstOrDefaultAsync<TUser>(sql, new {NormalizedUserName = normalizedUserName});
        return result;
    }
    
    public async Task<TUser?> FindUserAsync(TKey userId)
    {
        var paramNames = typeof(TUser).GetProperties().Select(p => p.Name).ToList();
        var builder = new StringBuilder("SELECT TOP 1 ");
        builder.Append(string.Join(", ", paramNames));
        builder.Append(string.Join(", ", paramNames));
        builder.Append(" FROM AspNetUsers WHERE Id = @Id");
        
        var sql = builder.ToString();
        
        await using var connection = await _database.ConnectAsync();
        var result = await connection.QueryFirstOrDefaultAsync<TUser>(sql, new {Id = userId.ToString()});
        return result;
    }

    public Task<TUser?> FindByEmailAsync(string normalizedEmail)
    {
        throw new NotImplementedException();
    }
}