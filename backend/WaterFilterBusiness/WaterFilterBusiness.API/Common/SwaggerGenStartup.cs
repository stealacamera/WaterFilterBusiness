using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using WaterFilterBusiness.Common.Converters.JsonConverters;
using WaterFilterBusiness.Common.Enums;

namespace WaterFilterBusiness.API.Common;

public static class SwaggerGenStartup
{
    public static void RegisterSwaggerGenOptions(SwaggerGenOptions options)
    {
        // JWT authentication
        options.AddSecurityDefinition(
            JwtBearerDefaults.AuthenticationScheme,
            new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                In = ParameterLocation.Header,
                Scheme = JwtBearerDefaults.AuthenticationScheme,
                BearerFormat = "JWT",
                Description = "JWT authentication. Example: Bearer [token]",
            });

        options.AddSecurityRequirement(new OpenApiSecurityRequirement {
            {
                new OpenApiSecurityScheme {
                    Reference = new OpenApiReference {
                        Type = ReferenceType.SecurityScheme,
                        Id = JwtBearerDefaults.AuthenticationScheme }},
                new string[] {}
            }
        });

        // Type mapping
        options.MapType<DateOnly>(() =>
            new OpenApiSchema
            {
                Type = "string",
                Format = "date",
                Example = new OpenApiString(DateOnly.MinValue.ToString(DateOnlyJsonConverter.DateFormat))
            }
        );

        options.MapType<TimeOnly>(() =>
            new OpenApiSchema
            {
                Type = "string",
                Format = "time",
                Example = new OpenApiString(TimeOnly.MinValue.ToString(TimeOnlyJsonConverter.TimeFormat))
            }
        );

        options.MapType<Role>(() => new OpenApiSchema { Type = "string", Example = new OpenApiString(Role.OperationsChief.Name) });
        options.MapType<PaymentType>(() => new OpenApiSchema { Type = "string", Example = new OpenApiString(PaymentType.FullUpfront.Name) });
        options.MapType<Weekday>(() => new OpenApiSchema { Type = "string", Example = new OpenApiString(Weekday.Friday.Name) });
        options.MapType<CallOutcome>(() => new OpenApiSchema { Type = "string", Example = new OpenApiString(CallOutcome.Success.Name) });
        options.MapType<MeetingOutcome>(() => new OpenApiSchema { Type = "string", Example = new OpenApiString(MeetingOutcome.Successful.Name) });
        options.MapType<InventoryType>(() => new OpenApiSchema { Type = "string", Example = new OpenApiString(InventoryType.TechnicianInventory.Name) });
    }
}
