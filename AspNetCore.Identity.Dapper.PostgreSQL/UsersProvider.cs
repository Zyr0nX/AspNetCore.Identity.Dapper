using System.Data;
using AspNetCore.Identity.Dapper.Repositories;
using Microsoft.AspNetCore.Identity;
using Dapper;

namespace AspNetCore.Identity.Dapper.PostgreSQL;

public class UsersProvider<TUser, TKey>(IDatabase database) : IUsersProvider<TUser, TKey>
    where TUser : IdentityUser<TKey>
    where TKey : IEquatable<TKey>
{
    public async Task CreateAsync(TUser user)
    {
        await using var connection = await database.ConnectAsync();
        var paramNames = user.GetType().GetProperties().Select(p => p.Name)
            .ToList();

        var sql =
            $"INSERT INTO AspNetUsers ({string.Join(", ", paramNames)}) VALUES ({string.Join(", ", paramNames.Select(p => $"@{p}"))})";
        var result = await connection.ExecuteAsync(sql, user);
        if (result == 0) throw new DBConcurrencyException();
    }

    public async Task UpdateAsync(TUser user)
    {
        var paramNames = user.GetType().GetProperties().Select(p => p.Name)
            .ToList();

        var sql =
            $"UPDATE AspNetUsers SET {string.Join(", ", paramNames.Where(n => n != "Id" && n != "ConcurrencyStamp").Select(p => p + "= @" + p))}, ConcurrencyStamp = @NewConcurrencyStamp WHERE Id = @Id AND ConcurrencyStamp = @ConcurrencyStamp";
        var parameters = new DynamicParameters(user);
        parameters.Add("NewConcurrencyStamp", Guid.NewGuid().ToString());

        await using var connection = await database.ConnectAsync();
        var result = await connection.ExecuteAsync(sql, parameters);
        if (result == 0) throw new DBConcurrencyException();
    }

    public async Task DeleteAsync(TUser user)
    {
        const string sql = "DELETE FROM AspNetUsers WHERE Id = @Id AND ConcurrencyStamp = @ConcurrencyStamp";

        await using var connection = await database.ConnectAsync();
        var result = await connection.ExecuteAsync(sql, new { Id = user.Id, ConcurrencyStamp = user.ConcurrencyStamp });
        if (result == 0) throw new DBConcurrencyException();
    }

    public async Task<TUser?> FindByIdAsync(string userId)
    {
        var paramNames = typeof(TUser).GetProperties().Select(p => p.Name).ToList();

        var sql = $"SELECT {string.Join(", ", paramNames)} FROM AspNetUsers WHERE Id = @Id LIMIT 1";

        await using var connection = await database.ConnectAsync();
        var result = await connection.QuerySingleOrDefaultAsync<TUser>(sql, new { Id = userId });
        return result;
    }

    public async Task<TUser?> FindByNameAsync(string normalizedUserName)
    {
        var paramNames = typeof(TUser).GetProperties().Select(p => p.Name).ToList();

        var sql =
            $"SELECT {string.Join(", ", paramNames)} FROM AspNetUsers WHERE NormalizedUserName = @NormalizedUserName LIMIT 1";

        await using var connection = await database.ConnectAsync();
        var result =
            await connection.QueryFirstOrDefaultAsync<TUser>(sql, new { NormalizedUserName = normalizedUserName });
        return result;
    }

    public async Task<TUser?> FindUserAsync(TKey userId)
    {
        var paramNames = typeof(TUser).GetProperties().Select(p => p.Name).ToList();
        var sql = $"SELECT {string.Join(", ", paramNames)} FROM AspNetUsers WHERE Id = @Id LIMIT 1";

        await using var connection = await database.ConnectAsync();
        var result = await connection.QueryFirstOrDefaultAsync<TUser>(sql, new { Id = userId.ToString() });
        return result;
    }

    public async Task<TUser?> FindByEmailAsync(string normalizedEmail)
    {
        var paramNames = typeof(TUser).GetProperties().Select(p => p.Name).ToList();
        var sql =
            $"SELECT {string.Join(", ", paramNames)} FROM AspNetUsers WHERE NormalizedEmail = @NormalizedEmail LIMIT 1";

        await using var connection = await database.ConnectAsync();
        var result = await connection.QuerySingleOrDefaultAsync<TUser>(sql, new { NormalizedEmail = normalizedEmail });
        return result;
    }
}