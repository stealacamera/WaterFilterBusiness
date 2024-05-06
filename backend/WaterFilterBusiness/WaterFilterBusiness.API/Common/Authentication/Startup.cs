using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using WaterFilterBusiness.Common.Options;

namespace WaterFilterBusiness.API.Common.Authentication;

public static class Startup
{
    public static void RegisterAuthorizationServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IJwtProvider, JwtProvider>();
        services.ConfigureOptions<JwtOptionsSetup>();

        JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

        services.AddAuthentication(e =>
                {
                    e.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                    e.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    e.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidAudience = configuration["Jwt:Audience"],
                        ValidIssuer = configuration["Jwt:Issuer"],
                        RequireExpirationTime = true,
                        IssuerSigningKey =
                            new SymmetricSecurityKey(
                                Encoding.UTF8.GetBytes(configuration["Jwt:SecretKey"]!)),
                        ValidateIssuerSigningKey = true
                    };
                });

        services.AddAuthorization();
        services.AddSingleton<IAuthorizationHandler, PermissionAuthorizationHandler>();
        services.AddSingleton<IAuthorizationPolicyProvider, PermissionAuthorizationPolicyProvider>();
    }
}
