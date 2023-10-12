using System.Reflection;
using DrTrottoirApi.Entities;
using DrTrottoirApi.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

namespace DrTrottoirApi.Extensions
{
    public static class StartupExtension
    {
        public static IServiceCollection AddDbConnection(this IServiceCollection serviceCollection)
        {
            var connectionString =
                   $"Host={Environment.GetEnvironmentVariable("DB_IP")};Database=drtrottoirdb;Username={Environment.GetEnvironmentVariable("DB_USER")};Password={Environment.GetEnvironmentVariable("DB_PASSWORD")};TrustServerCertificate=true;";

            serviceCollection.AddDbContext<DrTrottoirDbContext>(options => options.UseNpgsql(connectionString,
                   opt =>
                   {
                       opt.EnableRetryOnFailure(3, TimeSpan.FromSeconds(10), null);
                   }));
            
            return serviceCollection;
        }

        public static IServiceCollection SetupSwagger(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "DrTrottoir API",
                });


                options.OrderActionsBy(apiDesc => $"{apiDesc.ActionDescriptor.RouteValues["controller"]}_{apiDesc.RelativePath}_{apiDesc.HttpMethod}");
                var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
                options.AddSecurityDefinition("Bearer",
                    new OpenApiSecurityScheme()
                    {
                        Description = "JWT Authorization header using the Bearer scheme (Example: 'Bearer 12345abcdef')",
                        Name = "Authorization",
                        In = ParameterLocation.Header,
                        Type = SecuritySchemeType.ApiKey,
                        Scheme = "Bearer"
                    });
                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
                });
            });

            return serviceCollection;
        }

    }
}
