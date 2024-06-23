using System.Security.Claims;
using Microsoft.AspNetCore.Identity;

namespace AspNetCore.Identity.Dapper.Repositories;

public interface IUserClaimsProvider<in TUserClaim, TUser, in TKey> where TUserClaim : IdentityUserClaim<TKey>
    where TUser : IdentityUser<TKey>
    where TKey : IEquatable<TKey>
{
    Task<IList<Claim>> GetClaimsAsync(TKey userId);
    Task AddClaimsAsync(IEnumerable<TUserClaim> userClaims);
    Task ReplaceClaimAsync(TKey userId, Claim claim, Claim newClaim);
    Task RemoveClaimsAsync(TKey userId, IEnumerable<Claim> claims);
    Task<IList<TUser>> GetUsersForClaimAsync(Claim claim);
}