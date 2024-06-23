using System.Security.Claims;
using AspNetCore.Identity.Dapper.Repositories;
using Dapper;
using Microsoft.AspNetCore.Identity;

namespace AspNetCore.Identity.Dapper.PostgreSQL;

public class RoleClaimsProvider<TRoleClaim, TRole, TKey>(IDatabase database) : IRoleClaimsProvider<TRoleClaim, TRole, TKey>
    where TRoleClaim : IdentityRoleClaim<TKey>
    where TRole : IdentityRole<TKey>
    where TKey : IEquatable<TKey>
{
    public async Task<IList<Claim>> GetClaimsAsync(TKey roleId)
    {
        const string sql = $"SELECT ClaimType, ClaimValue FROM AspNetRoleClaims WHERE RoleId = @RoleId";
        await using var connection = await database.ConnectAsync();
        var roleClaims = await connection.QueryAsync<TRoleClaim>(sql, new { RoleId = roleId });
        var claims = roleClaims.Select(rc => rc.ToClaim()).ToList();
        return claims;
    }

    public async Task AddClaimAsync(TKey roleId, Claim claim)
    {
        const string sql = $"INSERT INTO AspNetRoleClaims (RoleId, ClaimType, ClaimValue) VALUES (@RoleId, @ClaimType, @ClaimValue)";
        await using var connection = await database.ConnectAsync();
        await connection.ExecuteAsync(sql, new
        {
            RoleId = roleId.ToString(),
            ClaimType = claim.Type,
            ClaimValue = claim.Value
        });
    }

    public async Task ReplaceClaimAsync(TKey roleId, Claim claim, Claim newClaim)
    {
        var sql =
            $"UPDATE INTO AspNetRoleClaims SET ClaimType = @NewClaimType, ClaimValue = @NewClaimValue WHERE RoleId = @roleId AND ClaimType = @ClaimType AND ClaimValue = @ClaimValue";
        await using var connection = await database.ConnectAsync();
        await connection.ExecuteAsync(sql, new
        {
            UserId = roleId.ToString(),
            ClaimType = claim.Type,
            ClaimValue = claim.Value,
            NewClaimType = newClaim.Type,
            NewClaimValue = newClaim.Value
        });
    }

    public async Task RemoveClaimAsync(TKey roleId, Claim claim)
    {
        const string sql = "DELETE FROM AspNetRoleClaims WHERE RoleId = @roleId AND ClaimType = @ClaimType AND ClaimValue = @ClaimValue";
        await using var connection = await database.ConnectAsync();
        
        await connection.ExecuteAsync(sql, new
        {
            UserId = roleId.ToString(),
            ClaimType = claim.Type,
            ClaimValue = claim.Value
        });
    }
}