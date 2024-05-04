using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using WaterFilterBusiness.API.Common;
using WaterFilterBusiness.API.Common.Authentication;
using WaterFilterBusiness.BLL;
using WaterFilterBusiness.Common.Converters.JsonConverters;
using WaterFilterBusiness.Common.Enums;
using WaterFilterBusiness.Common.Options;
using WaterFilterBusiness.DAL;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.RegisterDALServices(builder.Configuration);
builder.Services.RegisterBLLServices();

builder.Services.AddScoped<IJwtProvider, JwtProvider>();
builder.Services.AddTransient<ExceptionHandlingMiddleware>();

builder.Services.AddControllers()
                .AddJsonOptions(options =>
                {
                    options.AllowInputFormatterExceptionMessages = false;

                    options.JsonSerializerOptions.Converters.Add(new DateOnlyJsonConverter());
                    options.JsonSerializerOptions.Converters.Add(new TimeOnlyJsonConverter());

                    options.JsonSerializerOptions.Converters.Add(new RoleJsonConverter());
                    options.JsonSerializerOptions.Converters.Add(new WeekdayJsonConverter());
                    options.JsonSerializerOptions.Converters.Add(new PaymentTypeJsonConverter());

                    options.JsonSerializerOptions.Converters.Add(new CallOutcomeJsonConverter());
                    options.JsonSerializerOptions.Converters.Add(new MeetingOutcomeJsonConverter());
                });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
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
                new string[] { "Bearer " }
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

        options.MapType<Role>(() => new OpenApiSchema { Type = "string", Example = new OpenApiString(Role.Admin.Name) });
        options.MapType<PaymentType>(() => new OpenApiSchema { Type = "string", Example = new OpenApiString(PaymentType.FullUpfront.Name) });
        options.MapType<Weekday>(() => new OpenApiSchema { Type = "string", Example = new OpenApiString(Weekday.Friday.Name) });
        options.MapType<CallOutcome>(() => new OpenApiSchema { Type = "string", Example = new OpenApiString(CallOutcome.Success.Name) });
        options.MapType<MeetingOutcome>(() => new OpenApiSchema { Type = "string", Example = new OpenApiString(MeetingOutcome.Successful.Name) });
    });

builder.Services.ConfigureOptions<JwtOptionsSetup>();
JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

builder.Services.AddAuthentication(e =>
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
                        ValidAudience = builder.Configuration["Jwt:Audience"],
                        ValidIssuer = builder.Configuration["Jwt:Issuer"],
                        RequireExpirationTime = true,
                        IssuerSigningKey =
                            new SymmetricSecurityKey(
                                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:SecretKey"]!)),
                        ValidateIssuerSigningKey = true
                    };
                });

builder.Services.AddAuthorization();
builder.Services.AddSingleton<IAuthorizationHandler, PermissionAuthorizationHandler>();
builder.Services.AddSingleton<IAuthorizationPolicyProvider, PermissionAuthorizationPolicyProvider>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.MapControllers();

await app.Services.SeedAdmin();

app.Run();
