using System.Security.Claims;
using AspNetCore.Identity.Dapper.Repositories;
using Dapper;
using Microsoft.AspNetCore.Identity;

namespace AspNetCore.Identity.Dapper.PostgreSQL;

public class UserClaimsProvider<TUserClaim, TUser, TKey>(IDatabase database)
    : IUserClaimsProvider<TUserClaim, TUser, TKey>
    where TUserClaim : IdentityUserClaim<TKey>
    where TUser : IdentityUser<TKey>
    where TKey : IEquatable<TKey>
{
    public async Task<IList<Claim>> GetClaimsAsync(TKey userId)
    {
        const string sql = $"SELECT ClaimType, ClaimValue FROM AspNetUserClaims WHERE UserId = @UserId";
        await using var connection = await database.ConnectAsync();
        var userClaims = await connection.QueryAsync<TUserClaim>(sql, new { UserId = userId });
        var claims = userClaims.Select(uc => uc.ToClaim()).ToList();
        return claims;
    }

    public async Task AddClaimsAsync(IEnumerable<TUserClaim> userClaims)
    {
        var paramNames = typeof(TUserClaim).GetProperties()
            .Select(p => p.Name)
            .ToList();

        var sql =
            $"INSERT INTO AspNetUserClaims ({string.Join(", ", paramNames)}) VALUES ({string.Join(", ", paramNames.Select(p => $"@{p}"))})";
        await using var connection = await database.ConnectAsync();
        await using var transaction = await connection.BeginTransactionAsync();
        var result = await connection.ExecuteAsync(sql, userClaims, transaction);
        if (result == userClaims.Count()) await transaction.CommitAsync();
        else await transaction.RollbackAsync();
    }

    public async Task ReplaceClaimAsync(TKey userId, Claim claim, Claim newClaim)
    {
        var sql =
            $"UPDATE INTO AspNetUserClaims SET ClaimType = @NewClaimType, ClaimValue = @NewClaimValue WHERE UserId = @UserId AND ClaimType = @ClaimType AND ClaimValue = @ClaimValue";
        await using var connection = await database.ConnectAsync();
        await connection.ExecuteAsync(sql, new
        {
            UserId = userId.ToString(),
            ClaimType = claim.Type,
            ClaimValue = claim.Value,
            NewClaimType = newClaim.Type,
            NewClaimValue = newClaim.Value
        });
    }

    public async Task RemoveClaimsAsync(TKey userId, IEnumerable<Claim> claims)
    {
        const string sql = "DELETE FROM AspNetUserClaims WHERE UserId = @UserId AND ClaimType = @ClaimType AND ClaimValue = @ClaimValue";
        await using var connection = await database.ConnectAsync();
        await using var transaction = await connection.BeginTransactionAsync();

        var enumerable = claims.ToList();
        var result = await connection.ExecuteAsync(sql, enumerable.Select(c => new
        {
            UserId = userId.ToString(),
            ClaimType = c.Type,
            ClaimValue = c.Value
        }), transaction);
        
        if (result == enumerable.Count) await transaction.CommitAsync();
        await transaction.CommitAsync();
    }

    public async Task<IList<TUser>> GetUsersForClaimAsync(Claim claim)
    {
        var paramNames = typeof(TUser).GetProperties().Select(p => p.Name).ToList();
        var sql =
            $"SELECT {string.Join(", ", paramNames.Select(p => $"u.{p}"))} FROM AspNetUserClaims uc JOIN AspNetUsers u ON uc.UserId = u.Id WHERE uc.ClaimType = @ClaimType AND uc.ClaimValue = @ClaimValue";
        await using var connection = await database.ConnectAsync();
        var users = await connection.QueryAsync<TUser>(sql, new
        {
            ClaimType = claim.Type,
            ClaimValue = claim.Value
        });
        return users.ToList();
    }
}