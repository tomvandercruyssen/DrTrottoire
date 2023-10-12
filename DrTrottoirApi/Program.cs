using System.Reflection;
using System.Text;
using System.Text.Json.Serialization;
using DrTrottoirApi.CloudStorage;
using DrTrottoirApi.Data;
using DrTrottoirApi.Entities;
using DrTrottoirApi.Extensions;
using DrTrottoirApi.Helpers;
using DrTrottoirApi.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);
var corsOrigins = "OriginsCors";

// Comment this line for development
builder.Services.AddDbConnection();
builder.Services.AddControllers().AddJsonOptions(options => options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);

builder.Services.AddRouting(options => options.LowercaseUrls = true);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Uncomment this line below for local development
//builder.Services.AddDbContext<DrTrottoirDbContext>(options => options.UseNpgsql(builder.Configuration.GetConnectionString("DrTrottoirDbContext")));

builder.Services.AddIdentity<User, Role>().AddEntityFrameworkStores<DrTrottoirDbContext>();
builder.Services.AddScoped<ICloudStorage, CloudStorage>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<ICompanyRepository, CompanyRepository>();
builder.Services.AddScoped<IRoundRepository, RoundRepository>();
builder.Services.AddScoped<IGarbageTypeRepository, GarbageTypeRepository>();
builder.Services.AddScoped<IGarbageCollectionRepository, GarbageCollectionRepository>();
builder.Services.AddScoped<IWorkAreaRepository, WorkAreaRepository>();
builder.Services.AddScoped<ISyndicRepository, SyndicRepository>();
builder.Services.AddScoped<ITaskRepository, TaskRepository>();

var appSettingsSection = builder.Configuration.GetSection("AppSettings");
builder.Services.Configure<AppSettings>(appSettingsSection);

var appSettings = appSettingsSection.Get<AppSettings>();
var key = Encoding.ASCII.GetBytes(appSettings.Secret);

var url = $"http://0.0.0.0:80";

builder.Services.AddAuthentication(x =>
    {
        x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(y =>
    {
        y.RequireHttpsMetadata = false;
        y.SaveToken = true;
        y.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateIssuer = false,
            ValidateAudience = false,
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.SetupSwagger();

builder.Services.AddCors(options =>
{
    options.AddPolicy(corsOrigins, policy =>
    {
        policy.AllowAnyHeader()
            .AllowAnyMethod()
            .WithOrigins("...")
            .AllowCredentials();
        policy.SetIsOriginAllowed(_ => true);
    });
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = scope.ServiceProvider.GetRequiredService<DrTrottoirDbContext>();
    context.Database.Migrate();
    DbInitializer.Initialize(services);
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();

app.UseCors(corsOrigins);
app.UseAuthorization();
app.MapControllers();

app.Run(url);
