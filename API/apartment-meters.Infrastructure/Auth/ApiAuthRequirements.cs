using Microsoft.AspNetCore.Authorization;

namespace Infrastructure.Auth;

/// <summary>
/// Требование авторизации для доступа к API/auth/me
/// </summary>
public class ApiAuthMeRequirement : IAuthorizationRequirement
{
} 