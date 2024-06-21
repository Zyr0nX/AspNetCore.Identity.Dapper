using Microsoft.AspNetCore.Identity;

namespace AspNetCore.Identity.Dapper.Repositories;

public interface IRolesProvider<TRole, TUser, TKey> where TRole : IdentityRole<TKey> where TUser : IdentityUser<TKey> where TKey : IEquatable<TKey>
{
    Task<TRole?> FindRoleAsync(string normalizedRoleName);
    Task<IList<TUser>> GetUsersInRoleAsync(TRole role);
}