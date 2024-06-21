using Microsoft.AspNetCore.Identity;

namespace AspNetCore.Identity.Dapper.Repositories;

public interface IUserRolesProvider<TUserRole, TUser, in TKey> where TUserRole : IdentityUserRole<TKey> where TUser : IdentityUser<TKey> where TKey : IEquatable<TKey>
{
    Task<TUserRole?> FindUserRoleAsync(TKey userId, TKey roleId);
    Task AddUserRoleAsync(TUserRole userRole);
    Task RemoveUserRoleAsync(TUserRole userRole);
    Task<IList<string>> GetRolesAsync(TKey userId);
    Task<bool> IsInRoleAsync(TUser user, string normalizedRoleName);
}