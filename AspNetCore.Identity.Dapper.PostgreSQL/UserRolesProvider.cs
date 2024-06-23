using System.Data;
using AspNetCore.Identity.Dapper.Repositories;
using Dapper;
using Microsoft.AspNetCore.Identity;

namespace AspNetCore.Identity.Dapper.PostgreSQL;

public class UserRolesProvider<TUserRole, TUser, TKey>(IDatabase database)
    : IUserRolesProvider<TUserRole, TUser, TKey> where TUserRole : IdentityUserRole<TKey>
    where TUser : IdentityUser<TKey>
    where TKey : IEquatable<TKey>
{
    public async Task<TUserRole?> FindUserRoleAsync(TKey userId, TKey roleId)
    {
        var paramNames = typeof(TUserRole).GetProperties().Select(p => p.Name).ToList();
        var sql =
            $"SELECT {string.Join(", ", paramNames)} FROM AspNetUserRoles WHERE UserId = @UserId AND RoleId = @RoleId LIMIT 1";

        await using var connection = await database.ConnectAsync();
        var result =
            await connection.QuerySingleOrDefaultAsync<TUserRole>(sql,
                new { UserId = userId.ToString(), RoleId = roleId.ToString() });
        return result;
    }

    public async Task AddUserRoleAsync(TUserRole userRole)
    {
        var paramNames = typeof(TUserRole).GetProperties().Select(p => p.Name)
            .ToList();

        var sql =
            $"INSERT INTO AspNetUserRoles ({string.Join(", ", paramNames)}) VALUES ({string.Join(", ", paramNames.Select(p => "@" + p))})";
        using var connection = await database.ConnectAsync();
        var result = await connection.ExecuteAsync(sql, userRole);
        if (result == 0) throw new DBConcurrencyException();
    }

    public async Task RemoveUserRoleAsync(TKey userId, TKey roleId)
    {
        const string sql = $"DELETE FROM AspNetUserRoles WHERE UserId = @UserId AND RoleId = @RoleId";
        await using var connection = await database.ConnectAsync();
        var result = await connection.ExecuteAsync(sql, new { UserId = userId.ToString(), RoleId = roleId.ToString() });
        if (result == 0) throw new DBConcurrencyException();
    }

    public async Task<IList<string>> GetRolesAsync(TKey userId)
    {
        const string sql =
            $"SELECT r.Name FROM AspNetUserRoles ur JOIN AspNetRoles r ON ur.RoleId = r.Id WHERE ur.UserId = @UserId";
        await using var connection = await database.ConnectAsync();
        var roleNames = await connection.QueryAsync<string>(sql, new { UserId = userId.ToString() });
        return roleNames.ToList();
    }

    public async Task<IList<TUser>> GetUsersInRoleAsync(TKey roleId)
    {
        var paramNames = typeof(TUser).GetProperties().Select(p => p.Name).ToList();
        var sql =
            $"SELECT {string.Join(", ", paramNames.Select(p => $"u.{p}"))} FROM AspNetUserRoles ur JOIN AspNetUsers u ON ur.UserId = u.Id WHERE ur.RoleId = @RoleId";
        await using var connection = await database.ConnectAsync();
        var users = await connection.QueryAsync<TUser>(sql, new { RoleId = roleId });
        return users.ToList();
    }
}