using Microsoft.AspNetCore.Identity;

namespace AspNetCore.Identity.Dapper.Repositories;

public interface IUsersProvider<TUser, in TKey> where TUser : IdentityUser<TKey> where TKey : IEquatable<TKey>
{
    Task CreateAsync(TUser user);
    Task UpdateAsync(TUser user);
    Task DeleteAsync(TUser user);
    Task<TUser?> FindByIdAsync(string userId);
    Task<TUser?> FindByNameAsync(string normalizedUserName);
    Task<TUser?> FindUserAsync(TKey userId);
    Task<TUser?> FindByEmailAsync(string normalizedEmail);
}