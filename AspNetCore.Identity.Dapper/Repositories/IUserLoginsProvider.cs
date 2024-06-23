using Microsoft.AspNetCore.Identity;

namespace AspNetCore.Identity.Dapper.Repositories;

public interface IUserLoginsProvider<TUserLogin, in TKey> where TUserLogin : IdentityUserLogin<TKey> where TKey : IEquatable<TKey>
{
    Task<TUserLogin?> FindUserLoginAsync(TKey userId, string loginProvider, string providerKey);
    Task<TUserLogin?> FindUserLoginAsync(string loginProvider, string providerKey);
    Task AddLoginAsync(TKey userId, UserLoginInfo login);
    Task RemoveLoginAsync(TUserLogin userLogin);
    Task<IList<UserLoginInfo>> GetLoginsAsync(TKey userId);
}