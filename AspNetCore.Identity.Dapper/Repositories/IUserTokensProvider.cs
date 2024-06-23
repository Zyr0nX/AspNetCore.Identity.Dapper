using Microsoft.AspNetCore.Identity;

namespace AspNetCore.Identity.Dapper.Repositories;

public interface IUserTokensProvider<TUserToken, in TKey> where TUserToken : IdentityUserToken<TKey> where TKey : IEquatable<TKey>
{
    Task<TUserToken?> FindTokenAsync(TKey userId, string loginProvider, string name);
    Task AddUserTokenAsync(TUserToken token);
    Task RemoveUserTokenAsync(TUserToken token);
}