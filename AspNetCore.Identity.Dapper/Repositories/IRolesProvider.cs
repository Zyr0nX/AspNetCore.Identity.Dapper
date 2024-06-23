using Microsoft.AspNetCore.Identity;

namespace AspNetCore.Identity.Dapper.Repositories;

public interface IRolesProvider<TRole, in TKey> where TRole : IdentityRole<TKey> where TKey : IEquatable<TKey>
{
    Task<TRole?> FindRoleAsync(string normalizedRoleName);
    Task CreateAsync(TRole role);
    Task UpdateAsync(TRole role);
    Task DeleteAsync(TRole role);
    Task SetRoleNameAsync(TKey roleId, string? roleName);
    Task<TRole?> FindByIdAsync(TKey id);
    Task<TRole?> FindByNameAsync(string normalizedName);
    Task SetNormalizedRoleNameAsync(TKey roleId, string? normalizedName);
}