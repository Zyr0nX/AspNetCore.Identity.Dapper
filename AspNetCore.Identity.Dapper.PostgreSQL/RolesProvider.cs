using System.Data;
using AspNetCore.Identity.Dapper.Repositories;
using Dapper;
using Microsoft.AspNetCore.Identity;

namespace AspNetCore.Identity.Dapper.PostgreSQL;

public class RolesProvider<TRole, TKey>(IDatabase database)
    : IRolesProvider<TRole, TKey> where TRole : IdentityRole<TKey> where TKey : IEquatable<TKey>
{
    public async Task<TRole?> FindRoleAsync(string normalizedRoleName)
    {
        var paramNames = typeof(TRole).GetProperties().Select(p => p.Name).ToList();
        var sql =
            $"SELECT {string.Join(", ", paramNames)} FROM AspNetRoles WHERE NormalizedName = @NormalizedName LIMIT 1";

        await using var connection = await database.ConnectAsync();
        var result =
            await connection.QuerySingleOrDefaultAsync<TRole>(sql, new { NormalizedName = normalizedRoleName });
        return result;
    }

    public async Task CreateAsync(TRole role)
    {
        await using var connection = await database.ConnectAsync();
        var paramNames = role.GetType().GetProperties().Select(p => p.Name)
            .ToList();

        var sql =
            $"INSERT INTO AspNetRoles ({string.Join(", ", paramNames)}) VALUES ({string.Join(", ", paramNames.Select(p => $"@{p}"))})";
        var result = await connection.ExecuteAsync(sql, role);
        if (result == 0) throw new DBConcurrencyException();
    }

    public async Task UpdateAsync(TRole role)
    {
        var paramNames = typeof(TRole).GetProperties().Select(p => p.Name)
            .ToList();

        var sql =
            $"UPDATE AspNetRoles SET {string.Join(", ", paramNames.Where(n => n != "Id" && n != "ConcurrencyStamp").Select(p => p + "= @" + p))}, ConcurrencyStamp = @NewConcurrencyStamp WHERE Id = @Id AND ConcurrencyStamp = @ConcurrencyStamp";
        var parameters = new DynamicParameters(role);
        parameters.Add("NewConcurrencyStamp", Guid.NewGuid().ToString());

        await using var connection = await database.ConnectAsync();
        var result = await connection.ExecuteAsync(sql, parameters);
        if (result == 0) throw new DBConcurrencyException();
    }

    public async Task DeleteAsync(TRole role)
    {
        const string sql = "DELETE FROM AspNetRoles WHERE Id = @Id AND ConcurrencyStamp = @ConcurrencyStamp";

        await using var connection = await database.ConnectAsync();
        var result = await connection.ExecuteAsync(sql, new { role.Id, role.ConcurrencyStamp });
        if (result == 0) throw new DBConcurrencyException();
    }

    public async Task SetRoleNameAsync(TKey roleId, string? roleName)
    {
        const string sql = $"UPDATE AspNetRoles SET Name = @Name WHERE Id = @Id";
        await using var connection = await database.ConnectAsync();
        await connection.ExecuteAsync(sql, new { roleId });
    }

    public async Task<TRole?> FindByIdAsync(TKey id)
    {
        var paramNames = typeof(TRole).GetProperties().Select(p => p.Name).ToList();

        var sql = $"SELECT {string.Join(", ", paramNames)} FROM AspNetRoles WHERE Id = @Id LIMIT 1";

        await using var connection = await database.ConnectAsync();
        var result = await connection.QuerySingleOrDefaultAsync<TRole>(sql, new { Id = id });
        return result;
    }

    public async Task<TRole?> FindByNameAsync(string normalizedName)
    {
        var paramNames = typeof(TRole).GetProperties().Select(p => p.Name).ToList();

        var sql =
            $"SELECT {string.Join(", ", paramNames)} FROM AspNetRoles WHERE NormalizedName = @NormalizedName LIMIT 1";

        await using var connection = await database.ConnectAsync();
        var result =
            await connection.QueryFirstOrDefaultAsync<TRole>(sql, new { NormalizedName = normalizedName });
        return result;
    }

    public async Task SetNormalizedRoleNameAsync(TKey roleId, string? normalizedName)
    {
        const string sql = $"UPDATE AspNetRoles SET NormalizedName = @NormalizedName WHERE Id = @Id";
        await using var connection = await database.ConnectAsync();
        await connection.ExecuteAsync(sql, new { Id = roleId, NormalizedName = normalizedName });
    }
}