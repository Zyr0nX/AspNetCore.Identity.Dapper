using AspNetCore.Identity.Dapper.Repositories;
using Dapper;
using Microsoft.AspNetCore.Identity;

namespace AspNetCore.Identity.Dapper.PostgreSQL;

public class UserLoginsProvider<TUserLogin, TKey>(IDatabase database)
    : IUserLoginsProvider<TUserLogin, TKey> where TUserLogin : IdentityUserLogin<TKey>
    where TKey : IEquatable<TKey>
{
    public async Task<TUserLogin?> FindUserLoginAsync(TKey userId, string loginProvider, string providerKey)
    {
        var paramNames = typeof(TUserLogin).GetProperties().Select(p => p.Name).ToList();
        var sql =
            $"SELECT {string.Join(", ", paramNames)} FROM AspNetUserLogins WHERE UserId = @UserId AND LoginProvider = @LoginProvider AND ProviderKey = @ProviderKey";

        await using var connection = await database.ConnectAsync();
        return await connection.QuerySingleOrDefaultAsync<TUserLogin>(sql, new
        {
            UserId = userId.ToString(),
            LoginProvider = loginProvider,
            ProviderKey = providerKey
        });
    }

    public async Task<TUserLogin?> FindUserLoginAsync(string loginProvider, string providerKey)
    {
        var paramNames = typeof(TUserLogin).GetProperties().Select(p => p.Name).ToList();
        var sql =
            $"SELECT {string.Join(", ", paramNames)} FROM AspNetUserLogins WHERE LoginProvider = @LoginProvider AND ProviderKey = @ProviderKey";

        await using var connection = await database.ConnectAsync();
        return await connection.QuerySingleOrDefaultAsync<TUserLogin>(sql, new
        {
            LoginProvider = loginProvider,
            ProviderKey = providerKey
        });
    }

    public async Task AddLoginAsync(TKey userId, UserLoginInfo login)
    {
        const string sql =
            $"INSERT INTO AspNetUserLogins (UserId, LoginProvider, ProviderKey, ProviderDisplayName) VALUES (@UserId, @LoginProvider, @ProviderKey, @ProviderDisplayName)";
        await using var connection = await database.ConnectAsync();
        await connection.ExecuteAsync(sql, new
        {
            UserId = userId,
            login.LoginProvider,
            login.ProviderKey,
            login.ProviderDisplayName
        });
    }

    public async Task RemoveLoginAsync(TUserLogin userLogin)
    {
        const string sql =
            $"DELETE FROM AspNetUserLogins WHERE LoginProvider = @LoginProvider AND ProviderKey = @ProviderKey";
        await using var connection = await database.ConnectAsync();
        await connection.ExecuteAsync(sql, new
        {
            userLogin.LoginProvider,
            userLogin.ProviderKey,
        });
    }

    public async Task<IList<UserLoginInfo>> GetLoginsAsync(TKey userId)
    {
        const string sql =
            $"SELECT LoginProvider, ProviderKey, ProviderDisplayName FROM AspNetUserLogins WHERE UserId = @UserId";

        await using var connection = await database.ConnectAsync();
        var userLogins = await connection.QueryAsync<UserLoginInfo>(sql, new
        {
            UserId = userId.ToString()
        });
        return userLogins.ToList();
    }
}