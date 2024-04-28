using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using WaterFilterBusiness.API.Common;
using WaterFilterBusiness.API.Common.Authentication;
using WaterFilterBusiness.BLL;
using WaterFilterBusiness.Common.Enums;
using WaterFilterBusiness.Common.JsonConverters;
using WaterFilterBusiness.Common.Options;
using WaterFilterBusiness.Common.Utils;
using WaterFilterBusiness.DAL;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.RegisterDALServices(builder.Configuration);
builder.Services.RegisterBLLServices();
builder.Services.AddTransient<ExceptionHandlingMiddleware>();

builder.Services.AddControllers()
                .AddJsonOptions(options => {
                    options.AllowInputFormatterExceptionMessages = false;

                    options.JsonSerializerOptions.Converters.Add(new DateOnlyJsonConverter());
                    options.JsonSerializerOptions.Converters.Add(new TimeOnlyJsonConverter());
                    options.JsonSerializerOptions.Converters.Add(new CallOutcomeJsonConverter());
                    options.JsonSerializerOptions.Converters.Add(new WeekdayJsonConverter());
                    options.JsonSerializerOptions.Converters.Add(new MeetingOutcomeJsonConverter());
                });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
    {
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

        options.MapType<Weekday>(() => new OpenApiSchema { Type = "string", Example = new OpenApiString("Wednesday") });
        options.MapType<CallOutcome>(() => new OpenApiSchema { Type = "string", Example = new OpenApiString("Success") });
        options.MapType<MeetingOutcome>(() => new OpenApiSchema { Type = "string", Example = new OpenApiString("Successful") });
    });

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer();

builder.Services.ConfigureOptions<JwtOptionsSetup>();
builder.Services.ConfigureOptions<JwtBearerOptionsSetup>();

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

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseAuthorization();
app.UseAuthentication();

app.MapControllers();

await app.Services.SeedAdmin();

app.Run();
