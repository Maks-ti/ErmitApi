

using ErmitApi;
using ErmitApi.DAL;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using NLog;
using NLog.Extensions.Logging;
using NLog.LayoutRenderers;
using NLog.Web;
using Npgsql;
using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;


try
{

    var builder = WebApplication.CreateBuilder(args);

    // builder конфигурации
    IConfigurationBuilder configurationBuilder = new ConfigurationBuilder()
        .SetBasePath(builder.Environment.ContentRootPath)
        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
        .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", true);

    IConfiguration configuration = configurationBuilder.Build();

    builder.Configuration.AddConfiguration(configuration); // регистрируем конфигурацию

    #region Logging
    LayoutRenderer.Register<LayoutRendererWrapper>("intercept");
    var logger = NLog.LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();

    builder.Logging.ClearProviders();
    builder.Host.UseNLog();
    #endregion

    #region Controllers
    builder.Services.AddControllers().AddNewtonsoftJson(options =>
    {
        options.SerializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Utc;
    });

    builder.Services.AddSwaggerGenNewtonsoftSupport();
    builder.Services.AddEndpointsApiExplorer();

    #endregion

    #region Swagger
    builder.Services.AddSwaggerGen(options =>
    {
        options.SwaggerDoc("v1.0", new OpenApiInfo { Version = "v1.0", Title = "Ermit api Service" });

        options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            Description = "Используйте Bearer-токен для авторизации через заголовки. Example: \"Authrization: {token}\"",
            Name = "Authorization",
            In = ParameterLocation.Header,
            Scheme = JwtBearerDefaults.AuthenticationScheme,
            Type = SecuritySchemeType.Http
        });

        options.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type=ReferenceType.SecurityScheme,
                        Id="Bearer"
                    }
                },
                Array.Empty<string>()
            }
        });

        options.CustomSchemaIds(type => $"{type.Name}_{Guid.NewGuid()}");
    });
    #endregion

    #region AUTH
    builder.Services.AddSingleton<RsaSecurityKey>(provider =>
    {
        RSA rsa = RSA.Create();
        rsa.ImportRSAPublicKey(
            source: Convert.FromBase64String(configuration["Jwt:PublicKey"]),
            bytesRead: out int _
        );

        return new RsaSecurityKey(rsa);
    });

    builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultSignInScheme = JwtBearerDefaults.AuthenticationScheme;


    }).AddJwtBearer(options =>
    {
        SecurityKey rsa = builder.Services.BuildServiceProvider().GetRequiredService<RsaSecurityKey>();
        options.RequireHttpsMetadata = false; // отключаем требование HTTPS
        options.SaveToken = true;
        // проверка параметров токена
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = "ERMIT", // издатель токена
            ValidateIssuerSigningKey = true,

            ValidateAudience = true,
            ValidAudience = "REST_CLIENT",

            ValidateLifetime = true,
            RequireExpirationTime = true,

            IssuerSigningKey = rsa,
            ClockSkew = TimeSpan.Zero
        };
    });

    builder.Services.AddAuthorization(options =>
    {
        options.DefaultPolicy = new AuthorizationPolicyBuilder(JwtBearerDefaults.AuthenticationScheme).RequireAuthenticatedUser().Build();
    });
    #endregion

    #region DataBase Contexts
    // устанавливае старое поведение обработки timestamp в postgres
    AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

    //создаём фабрику логгеров
    LoggerFactory dbloggerFactory = new(new[] { new NLogLoggerProvider() });

    // настройка глобального маппера типов NpgSQL для JSON
    NpgsqlConnection.GlobalTypeMapper.UseJsonNet(settings: new()
    {
        Formatting = Formatting.None,
        ContractResolver = new Newtonsoft.Json.Serialization.DefaultContractResolver
        {
            NamingStrategy = new Newtonsoft.Json.Serialization.CamelCaseNamingStrategy()
        }
    });

    // настройка DbContext 
    builder.Services.AddDbContext<DataBaseContext>((serviceProvider, options) =>
    {
        var loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();
        var connectionString = configuration.GetConnectionString("Postgres");

        options.UseLoggerFactory(loggerFactory)
               .UseNpgsql(connectionString, config => config.CommandTimeout(30));

        if (builder.Environment.IsDevelopment())
        {
            options.EnableSensitiveDataLogging();
        }
    }, ServiceLifetime.Transient);

    #endregion



    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    var app = builder.Build();

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseHttpsRedirection();

    app.UseAuthorization();

    app.MapControllers();

    app.Run();

}
finally
{
    LogManager.Shutdown();
}