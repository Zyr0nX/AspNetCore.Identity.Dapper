using System.Security.Claims;
using Microsoft.AspNetCore.Identity;

namespace AspNetCore.Identity.Dapper.Repositories;

public interface IUserClaimsProvider<TUserClaim, TUser, TKey> where TUserClaim : IdentityUserClaim<TKey>
    where TUser : IdentityUser<TKey>
    where TKey : IEquatable<TKey>
{
    Task<IList<Claim>> GetClaimsAsync(TUser user);
    Task AddClaimsAsync(IEnumerable<TUserClaim> userClaim);
    Task ReplaceClaimAsync(TUser user, Claim claim, Claim newClaim);
    Task RemoveClaimsAsync(TUser user, IEnumerable<Claim> claims);
    Task<IList<TUser>> GetUsersForClaimAsync(Claim claim);
}