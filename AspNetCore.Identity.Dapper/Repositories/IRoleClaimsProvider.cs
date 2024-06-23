using System.Security.Claims;
using Microsoft.AspNetCore.Identity;

namespace AspNetCore.Identity.Dapper.Repositories;

public interface IRoleClaimsProvider<TRoleClaim, TRole, in TKey> where TRoleClaim : IdentityRoleClaim<TKey>
    where TRole : IdentityRole<TKey>
    where TKey : IEquatable<TKey>
{
    Task<IList<Claim>> GetClaimsAsync(TKey roleId);
    Task AddClaimAsync(TKey roleId, Claim claim);
    Task ReplaceClaimAsync(TKey roleId, Claim claim, Claim newClaim);
    Task RemoveClaimAsync(TKey roleId, Claim claim);
}