using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using WaterFilterBusiness.BLL;
using WaterFilterBusiness.Common.Enums;
using WaterFilterBusiness.Common.JsonConverters;
using WaterFilterBusiness.Common.Utils;
using WaterFilterBusiness.DAL;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.RegisterDALServices(builder.Configuration);
builder.Services.RegisterBLLServices();

builder.Services.AddControllers()
                .AddJsonOptions(options => {
                    options.AllowInputFormatterExceptionMessages = false;

                    options.JsonSerializerOptions.Converters.Add(new DateOnlyJsonConverter());
                    options.JsonSerializerOptions.Converters.Add(new TimeOnlyJsonConverter());
                    options.JsonSerializerOptions.Converters.Add(new CallOutcomeJsonConverter());
                    options.JsonSerializerOptions.Converters.Add(new WeekdayJsonConverter());
                    options.JsonSerializerOptions.Converters.Add(new MeetingOutcomeJsonConverter());
                });

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
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

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

await app.Services.SeedAdmin();

app.Run();
