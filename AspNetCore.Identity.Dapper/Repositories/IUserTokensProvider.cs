using Microsoft.AspNetCore.Identity;

namespace AspNetCore.Identity.Dapper.Repositories;

public interface IUserTokensProvider<TUserToken, in TUser, in TKey> where TUserToken : IdentityUserToken<TKey> where TUser : IdentityUser<TKey> where TKey : IEquatable<TKey>
{
    Task<TUserToken?> FindTokenAsync(TUser user, string loginProvider, string name);
    Task AddUserTokenAsync(TUserToken token);
    Task RemoveUserTokenAsync(TUserToken token);
}