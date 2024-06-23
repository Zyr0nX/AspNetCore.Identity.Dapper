using AspNetCore.Identity.Dapper.Repositories;
using Dapper;
using Microsoft.AspNetCore.Identity;

namespace AspNetCore.Identity.Dapper.PostgreSQL;

public class UserTokensProvider<TUserToken, TKey>(IDatabase database) : IUserTokensProvider<TUserToken, TKey>
    where TUserToken : IdentityUserToken<TKey>  where TKey : IEquatable<TKey>
{
    public async Task<TUserToken?> FindTokenAsync(TKey userId, string loginProvider, string name)
    {
        var paramNames = typeof(TUserToken).GetProperties().Select(p => p.Name).ToList();
        var sql =
            $"SELECT {string.Join(", ", paramNames)} FROM AspNetUserTokens WHERE UserId = @UserId AND LoginProvider = @LoginProvider AND Name = @Name";

        await using var connection = await database.ConnectAsync();
        return await connection.QuerySingleOrDefaultAsync<TUserToken>(sql, new
        {
            UserId = userId.ToString(),
            LoginProvider = loginProvider,
            Name = name
        });
    }

    public async Task AddUserTokenAsync(TUserToken token)
    {
        var paramNames = typeof(TUserToken).GetProperties().Select(p => p.Name).ToList();
        var sql =
            $"INSERT INTO AspNetUserTokens ({string.Join(", ", paramNames)}) VALUES ({string.Join(", ", paramNames.Select(p => "@" + p))})";
        await using var connection = await database.ConnectAsync();
        await connection.ExecuteAsync(sql, token);
    }

    public async Task RemoveUserTokenAsync(TUserToken token)
    {
        const string sql =
            $"DELETE FROM AspNetUserTokens WHERE UserId = @UserId AND LoginProvider = @LoginProvider AND Name = @Name";
        await using var connection = await database.ConnectAsync();
        await connection.ExecuteAsync(sql, new
        {
            token.UserId,
            token.LoginProvider,
            token.Name
        });
    }
}