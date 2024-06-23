// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Data;
using System.Security.Claims;
using AspNetCore.Identity.Dapper.Repositories;
using Microsoft.AspNetCore.Identity;

namespace AspNetCore.Identity.Dapper;

/// <summary>
/// Represents a new instance of a persistence store for the specified user and role types.
/// </summary>
/// <typeparam name="TUser">The type representing a user.</typeparam>
/// <typeparam name="TContext">The type of the data context class used to access the store.</typeparam>
public class UserOnlyStore<TUser> : UserOnlyStore<TUser, string>
    where TUser : IdentityUser<string>
{
    /// <summary>
    /// Constructs a new instance of <see cref="UserStore"/>.
    /// </summary>
    /// <param name="context">The <see cref="DbContext"/>.</param>
    /// <param name="describer">The <see cref="IdentityErrorDescriber"/>.</param>
    public UserOnlyStore(IUsersProvider<TUser, string> usersProvider,
        IUserLoginsProvider<IdentityUserLogin<string>, string> userLoginsProvider,
        IUserClaimsProvider<IdentityUserClaim<string>, TUser, string> userClaimsProvider,
        IUserTokensProvider<IdentityUserToken<string>, string> userTokensProvider,
        IdentityErrorDescriber? describer = null) : base(usersProvider, userLoginsProvider, userClaimsProvider,
        userTokensProvider, describer)
    {
    }
}

/// <summary>
/// Represents a new instance of a persistence store for the specified user and role types.
/// </summary>
/// <typeparam name="TUser">The type representing a user.</typeparam>
/// <typeparam name="TContext">The type of the data context class used to access the store.</typeparam>
/// <typeparam name="TKey">The type of the primary key for a role.</typeparam>
public class UserOnlyStore<TUser, TKey> : UserOnlyStore<TUser, TKey, IdentityUserClaim<TKey>, IdentityUserLogin<TKey>,
    IdentityUserToken<TKey>>
    where TUser : IdentityUser<TKey>
    where TKey : IEquatable<TKey>
{
    /// <summary>
    /// Constructs a new instance of <see cref="UserStore{TUser, TRole, TContext, TKey}"/>.
    /// </summary>
    /// <param name="context">The <see cref="DbContext"/>.</param>
    /// <param name="describer">The <see cref="IdentityErrorDescriber"/>.</param>
    public UserOnlyStore(IUsersProvider<TUser, TKey> usersProvider,
        IUserLoginsProvider<IdentityUserLogin<TKey>, TKey> userLoginsProvider,
        IUserClaimsProvider<IdentityUserClaim<TKey>, TUser, TKey> userClaimsProvider,
        IUserTokensProvider<IdentityUserToken<TKey>, TKey> userTokensProvider,
        IdentityErrorDescriber? describer = null) : base(usersProvider, userLoginsProvider, userClaimsProvider,
        userTokensProvider, describer)
    {
    }
}

/// <summary>
/// Represents a new instance of a persistence store for the specified user and role types.
/// </summary>
/// <typeparam name="TUser">The type representing a user.</typeparam>
/// <typeparam name="TContext">The type of the data context class used to access the store.</typeparam>
/// <typeparam name="TKey">The type of the primary key for a role.</typeparam>
/// <typeparam name="TUserClaim">The type representing a claim.</typeparam>
/// <typeparam name="TUserLogin">The type representing a user external login.</typeparam>
/// <typeparam name="TUserToken">The type representing a user token.</typeparam>
public class UserOnlyStore<TUser, TKey, TUserClaim, TUserLogin, TUserToken> :
    UserStoreBase<TUser, TKey, TUserClaim, TUserLogin, TUserToken>,
    IUserLoginStore<TUser>,
    IUserClaimStore<TUser>,
    IUserPasswordStore<TUser>,
    IUserSecurityStampStore<TUser>,
    IUserEmailStore<TUser>,
    IUserLockoutStore<TUser>,
    IUserPhoneNumberStore<TUser>,
    IQueryableUserStore<TUser>,
    IUserTwoFactorStore<TUser>,
    IUserAuthenticationTokenStore<TUser>,
    IUserAuthenticatorKeyStore<TUser>,
    IUserTwoFactorRecoveryCodeStore<TUser>,
    IProtectedUserStore<TUser>
    where TUser : IdentityUser<TKey>
    where TKey : IEquatable<TKey>
    where TUserClaim : IdentityUserClaim<TKey>, new()
    where TUserLogin : IdentityUserLogin<TKey>, new()
    where TUserToken : IdentityUserToken<TKey>, new()
{
    /// <summary>
    /// Creates a new instance of the store.
    /// </summary>
    /// <param name="context">The context used to access the store.</param>
    /// <param name="describer">The <see cref="IdentityErrorDescriber"/> used to describe store errors.</param>
    public UserOnlyStore(IUsersProvider<TUser, TKey> usersProvider,
        IUserLoginsProvider<TUserLogin, TKey> userLoginsProvider,
        IUserClaimsProvider<TUserClaim, TUser, TKey> userClaimsProvider,
        IUserTokensProvider<TUserToken, TKey> userTokensProvider, IdentityErrorDescriber? describer = null) : base(
        describer ?? new IdentityErrorDescriber())
    {
        _usersProvider = usersProvider;
        _userLoginsProvider = userLoginsProvider;
        _userClaimsProvider = userClaimsProvider;
        _userTokensProvider = userTokensProvider;
    }

    private readonly IUsersProvider<TUser, TKey> _usersProvider;
    private readonly IUserLoginsProvider<TUserLogin, TKey> _userLoginsProvider;
    private readonly IUserClaimsProvider<TUserClaim, TUser, TKey> _userClaimsProvider;
    private readonly IUserTokensProvider<TUserToken, TKey> _userTokensProvider;

    /// <summary>
    /// Creates the specified <paramref name="user"/> in the user store.
    /// </summary>
    /// <param name="user">The user to create.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
    /// <returns>The <see cref="Task"/> that represents the asynchronous operation, containing the <see cref="IdentityResult"/> of the creation operation.</returns>
    public override async Task<IdentityResult> CreateAsync(TUser user,
        CancellationToken cancellationToken = default(CancellationToken))
    {
        cancellationToken.ThrowIfCancellationRequested();
        ThrowIfDisposed();
        ArgumentNullException.ThrowIfNull(user);
        await _usersProvider.CreateAsync(user);
        return IdentityResult.Success;
    }

    /// <summary>
    /// Updates the specified <paramref name="user"/> in the user store.
    /// </summary>
    /// <param name="user">The user to update.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
    /// <returns>The <see cref="Task"/> that represents the asynchronous operation, containing the <see cref="IdentityResult"/> of the update operation.</returns>
    public override async Task<IdentityResult> UpdateAsync(TUser user,
        CancellationToken cancellationToken = default(CancellationToken))
    {
        cancellationToken.ThrowIfCancellationRequested();
        ThrowIfDisposed();
        ArgumentNullException.ThrowIfNull(user);

        try
        {
            await _usersProvider.UpdateAsync(user);
        }
        catch (DBConcurrencyException)
        {
            return IdentityResult.Failed(ErrorDescriber.ConcurrencyFailure());
        }

        return IdentityResult.Success;
    }

    /// <summary>
    /// Deletes the specified <paramref name="user"/> from the user store.
    /// </summary>
    /// <param name="user">The user to delete.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
    /// <returns>The <see cref="Task"/> that represents the asynchronous operation, containing the <see cref="IdentityResult"/> of the update operation.</returns>
    public override async Task<IdentityResult> DeleteAsync(TUser user,
        CancellationToken cancellationToken = default(CancellationToken))
    {
        cancellationToken.ThrowIfCancellationRequested();
        ThrowIfDisposed();
        ArgumentNullException.ThrowIfNull(user);

        try
        {
            await _usersProvider.DeleteAsync(user);
        }
        catch (DBConcurrencyException)
        {
            return IdentityResult.Failed(ErrorDescriber.ConcurrencyFailure());
        }

        return IdentityResult.Success;
    }

    /// <summary>
    /// Finds and returns a user, if any, who has the specified <paramref name="userId"/>.
    /// </summary>
    /// <param name="userId">The user ID to search for.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
    /// <returns>
    /// The <see cref="Task"/> that represents the asynchronous operation, containing the user matching the specified <paramref name="userId"/> if it exists.
    /// </returns>
    public override Task<TUser?> FindByIdAsync(string userId,
        CancellationToken cancellationToken = default(CancellationToken))
    {
        cancellationToken.ThrowIfCancellationRequested();
        ThrowIfDisposed();
        var id = ConvertIdFromString(userId);
        return _usersProvider.FindByIdAsync(userId);
    }

    /// <summary>
    /// Finds and returns a user, if any, who has the specified normalized user name.
    /// </summary>
    /// <param name="normalizedUserName">The normalized user name to search for.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
    /// <returns>
    /// The <see cref="Task"/> that represents the asynchronous operation, containing the user matching the specified <paramref name="normalizedUserName"/> if it exists.
    /// </returns>
    public override Task<TUser?> FindByNameAsync(string normalizedUserName,
        CancellationToken cancellationToken = default(CancellationToken))
    {
        cancellationToken.ThrowIfCancellationRequested();
        ThrowIfDisposed();

        return _usersProvider.FindByNameAsync(normalizedUserName);
    }

    /// <summary>
    /// Return a user with the matching userId if it exists.
    /// </summary>
    /// <param name="userId">The user's id.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
    /// <returns>The user if it exists.</returns>
    protected override async Task<TUser?> FindUserAsync(TKey userId, CancellationToken cancellationToken)
    {
        return await _usersProvider.FindUserAsync(userId);
    }

    /// <summary>
    /// Return a user login with the matching userId, provider, providerKey if it exists.
    /// </summary>
    /// <param name="userId">The user's id.</param>
    /// <param name="loginProvider">The login provider name.</param>
    /// <param name="providerKey">The key provided by the <paramref name="loginProvider"/> to identify a user.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
    /// <returns>The user login if it exists.</returns>
    protected override async Task<TUserLogin?> FindUserLoginAsync(TKey userId, string loginProvider, string providerKey,
        CancellationToken cancellationToken)
    {
        return await _userLoginsProvider.FindUserLoginAsync(userId, loginProvider, providerKey);
    }

    /// <summary>
    /// Return a user login with  provider, providerKey if it exists.
    /// </summary>
    /// <param name="loginProvider">The login provider name.</param>
    /// <param name="providerKey">The key provided by the <paramref name="loginProvider"/> to identify a user.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
    /// <returns>The user login if it exists.</returns>
    protected override async Task<TUserLogin?> FindUserLoginAsync(string loginProvider, string providerKey,
        CancellationToken cancellationToken)
    {
        return await _userLoginsProvider.FindUserLoginAsync(loginProvider, providerKey);
    }

    /// <summary>
    /// Get the claims associated with the specified <paramref name="user"/> as an asynchronous operation.
    /// </summary>
    /// <param name="user">The user whose claims should be retrieved.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
    /// <returns>A <see cref="Task{TResult}"/> that contains the claims granted to a user.</returns>
    public override async Task<IList<Claim>> GetClaimsAsync(TUser user,
        CancellationToken cancellationToken = default(CancellationToken))
    {
        ThrowIfDisposed();
        ArgumentNullException.ThrowIfNull(user);

        return await _userClaimsProvider.GetClaimsAsync(user.Id);
    }

    /// <summary>
    /// Adds the <paramref name="claims"/> given to the specified <paramref name="user"/>.
    /// </summary>
    /// <param name="user">The user to add the claim to.</param>
    /// <param name="claims">The claim to add to the user.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
    /// <returns>The <see cref="Task"/> that represents the asynchronous operation.</returns>
    public override async Task AddClaimsAsync(TUser user, IEnumerable<Claim> claims,
        CancellationToken cancellationToken = default(CancellationToken))
    {
        ThrowIfDisposed();
        ArgumentNullException.ThrowIfNull(user);
        ArgumentNullException.ThrowIfNull(claims);
        var userClaims = claims.Select(claim => CreateUserClaim(user, claim)).ToList();
        await _userClaimsProvider.AddClaimsAsync(userClaims);
    }

    /// <summary>
    /// Replaces the <paramref name="claim"/> on the specified <paramref name="user"/>, with the <paramref name="newClaim"/>.
    /// </summary>
    /// <param name="user">The user to replace the claim on.</param>
    /// <param name="claim">The claim replace.</param>
    /// <param name="newClaim">The new claim replacing the <paramref name="claim"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
    /// <returns>The <see cref="Task"/> that represents the asynchronous operation.</returns>
    public override async Task ReplaceClaimAsync(TUser user, Claim claim, Claim newClaim,
        CancellationToken cancellationToken = default(CancellationToken))
    {
        ThrowIfDisposed();
        ArgumentNullException.ThrowIfNull(user);
        ArgumentNullException.ThrowIfNull(claim);
        ArgumentNullException.ThrowIfNull(newClaim);

        await _userClaimsProvider.ReplaceClaimAsync(user.Id, claim, newClaim);
    }

    /// <summary>
    /// Removes the <paramref name="claims"/> given from the specified <paramref name="user"/>.
    /// </summary>
    /// <param name="user">The user to remove the claims from.</param>
    /// <param name="claims">The claim to remove.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
    /// <returns>The <see cref="Task"/> that represents the asynchronous operation.</returns>
    public override async Task RemoveClaimsAsync(TUser user, IEnumerable<Claim> claims,
        CancellationToken cancellationToken = default(CancellationToken))
    {
        ThrowIfDisposed();
        ArgumentNullException.ThrowIfNull(user);
        ArgumentNullException.ThrowIfNull(claims);

        await _userClaimsProvider.RemoveClaimsAsync(user.Id, claims);
    }

    /// <summary>
    /// Adds the <paramref name="login"/> given to the specified <paramref name="user"/>.
    /// </summary>
    /// <param name="user">The user to add the login to.</param>
    /// <param name="login">The login to add to the user.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
    /// <returns>The <see cref="Task"/> that represents the asynchronous operation.</returns>
    public override async Task AddLoginAsync(TUser user, UserLoginInfo login,
        CancellationToken cancellationToken = default(CancellationToken))
    {
        cancellationToken.ThrowIfCancellationRequested();
        ThrowIfDisposed();
        ArgumentNullException.ThrowIfNull(user);
        ArgumentNullException.ThrowIfNull(login);

        await _userLoginsProvider.AddLoginAsync(user.Id, login);
    }

    /// <summary>
    /// Removes the <paramref name="loginProvider"/> given from the specified <paramref name="user"/>.
    /// </summary>
    /// <param name="user">The user to remove the login from.</param>
    /// <param name="loginProvider">The login to remove from the user.</param>
    /// <param name="providerKey">The key provided by the <paramref name="loginProvider"/> to identify a user.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
    /// <returns>The <see cref="Task"/> that represents the asynchronous operation.</returns>
    public override async Task RemoveLoginAsync(TUser user, string loginProvider, string providerKey,
        CancellationToken cancellationToken = default(CancellationToken))
    {
        cancellationToken.ThrowIfCancellationRequested();
        ThrowIfDisposed();
        ArgumentNullException.ThrowIfNull(user);
        var entry = await FindUserLoginAsync(user.Id, loginProvider, providerKey, cancellationToken);
        if (entry != null)
        {
            await _userLoginsProvider.RemoveLoginAsync(entry);
        }
    }

    /// <summary>
    /// Retrieves the associated logins for the specified <param ref="user"/>.
    /// </summary>
    /// <param name="user">The user whose associated logins to retrieve.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
    /// <returns>
    /// The <see cref="Task"/> for the asynchronous operation, containing a list of <see cref="UserLoginInfo"/> for the specified <paramref name="user"/>, if any.
    /// </returns>
    public override async Task<IList<UserLoginInfo>> GetLoginsAsync(TUser user,
        CancellationToken cancellationToken = default(CancellationToken))
    {
        cancellationToken.ThrowIfCancellationRequested();
        ThrowIfDisposed();
        ArgumentNullException.ThrowIfNull(user);
        var userId = user.Id;
        return await _userLoginsProvider.GetLoginsAsync(userId);
    }

    /// <summary>
    /// Retrieves the user associated with the specified login provider and login provider key.
    /// </summary>
    /// <param name="loginProvider">The login provider who provided the <paramref name="providerKey"/>.</param>
    /// <param name="providerKey">The key provided by the <paramref name="loginProvider"/> to identify a user.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
    /// <returns>
    /// The <see cref="Task"/> for the asynchronous operation, containing the user, if any which matched the specified login provider and key.
    /// </returns>
    public override async Task<TUser?> FindByLoginAsync(string loginProvider, string providerKey,
        CancellationToken cancellationToken = default(CancellationToken))
    {
        cancellationToken.ThrowIfCancellationRequested();
        ThrowIfDisposed();
        var userLogin = await FindUserLoginAsync(loginProvider, providerKey, cancellationToken);
        if (userLogin != null)
        {
            return await FindUserAsync(userLogin.UserId, cancellationToken);
        }

        return null;
    }

    /// <summary>
    /// Gets the user, if any, associated with the specified, normalized email address.
    /// </summary>
    /// <param name="normalizedEmail">The normalized email address to return the user for.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
    /// <returns>
    /// The task object containing the results of the asynchronous lookup operation, the user if any associated with the specified normalized email address.
    /// </returns>
    public override async Task<TUser?> FindByEmailAsync(string normalizedEmail,
        CancellationToken cancellationToken = default(CancellationToken))
    {
        cancellationToken.ThrowIfCancellationRequested();
        ThrowIfDisposed();

        return await _usersProvider.FindByEmailAsync(normalizedEmail);
    }

    /// <summary>
    /// Retrieves all users with the specified claim.
    /// </summary>
    /// <param name="claim">The claim whose users should be retrieved.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
    /// <returns>
    /// The <see cref="Task"/> contains a list of users, if any, that contain the specified claim.
    /// </returns>
    public override async Task<IList<TUser>> GetUsersForClaimAsync(Claim claim,
        CancellationToken cancellationToken = default(CancellationToken))
    {
        cancellationToken.ThrowIfCancellationRequested();
        ThrowIfDisposed();
        ArgumentNullException.ThrowIfNull(claim);

        return await _userClaimsProvider.GetUsersForClaimAsync(claim);
    }

    /// <summary>
    /// Find a user token if it exists.
    /// </summary>
    /// <param name="user">The token owner.</param>
    /// <param name="loginProvider">The login provider for the token.</param>
    /// <param name="name">The name of the token.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
    /// <returns>The user token if it exists.</returns>
    protected override Task<TUserToken?> FindTokenAsync(TUser user, string loginProvider, string name,
        CancellationToken cancellationToken)
        => _userTokensProvider.FindTokenAsync(user.Id, loginProvider, name);

    /// <summary>
    /// Add a new user token.
    /// </summary>
    /// <param name="token">The token to be added.</param>
    /// <returns></returns>
    protected override async Task AddUserTokenAsync(TUserToken token)
    {
        await _userTokensProvider.AddUserTokenAsync(token);
    }

    /// <summary>
    /// Remove a new user token.
    /// </summary>
    /// <param name="token">The token to be removed.</param>
    /// <returns></returns>
    protected override async Task RemoveUserTokenAsync(TUserToken token)
    {
        await _userTokensProvider.RemoveUserTokenAsync(token);
    }

    public override IQueryable<TUser> Users { get; }
}