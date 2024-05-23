namespace WaterFilterBusiness.API.Common;

public static class CorsStartup
{
    public static string CorsPolicyName = "WaterFilterBusinessAppCors";

    public static void RegisterCors(this IServiceCollection services)
    {
        services.AddCors(options =>
        {
            options.AddPolicy(name: CorsPolicyName,
                              policy => policy.WithOrigins("http://localhost:3000",
                                                           "http://localhost:3001",
                                                           "https://localhost:44340",
                                                           "https://localhost:7117")
                                              .AllowAnyHeader()
                                              .AllowAnyMethod());
        });
    }
}
