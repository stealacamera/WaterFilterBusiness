using WaterFilterBusiness.API.Common;
using WaterFilterBusiness.API.Common.Authentication;
using WaterFilterBusiness.API.Common.Quartz;
using WaterFilterBusiness.BLL;
using WaterFilterBusiness.Common.Converters.JsonConverters;
using WaterFilterBusiness.DAL;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.RegisterDALServices(builder.Configuration);
builder.Services.RegisterBLLServices();
builder.Services.RegisterAuthorizationServices(builder.Configuration);
builder.Services.RegisterQuartzServices();

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
                    options.JsonSerializerOptions.Converters.Add(new InventoryTypeJsonConverter());

                    options.JsonSerializerOptions.Converters.Add(new CallOutcomeJsonConverter());
                    options.JsonSerializerOptions.Converters.Add(new MeetingOutcomeJsonConverter());
                });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(SwaggerGenStartup.RegisterSwaggerGenOptions);

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
