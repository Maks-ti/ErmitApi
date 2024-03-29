
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
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.AspNetCore.Rewrite;
using ErmitApi.BLL;


try
{

    var builder = WebApplication.CreateBuilder(args);

    // builder ������������
    IConfigurationBuilder configurationBuilder = new ConfigurationBuilder()
        .SetBasePath(builder.Environment.ContentRootPath)
        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
        .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", true);

    IConfiguration configuration = configurationBuilder.Build();

    builder.Configuration.AddConfiguration(configuration); // ������������ ������������

    #region Kestrel
    // ����������� ������ �� ������ ���������
    int httpPort, httpsPort;
    if (builder.Environment.IsDevelopment())
    {
        httpPort = builder.Configuration.GetValue<int>("Kestrel:Development:HttpPort");
        httpsPort = builder.Configuration.GetValue<int>("Kestrel:Development:HttpsPort");
    }
    else
    {
        httpPort = builder.Configuration.GetValue<int>("Kestrel:Production:HttpPort");
        httpsPort = builder.Configuration.GetValue<int>("Kestrel:Production:HttpsPort");
    }

    // ��������� Kestrel
    bool successParseBodySize = int.TryParse(configuration["MaxRequestBodySize"], out int maxRequestBodySize); // ������ �������� �� ������������
    builder.WebHost.ConfigureKestrel(options =>
    {
        int size = (successParseBodySize) ? maxRequestBodySize : 30;
        // ������������� ����� �� ���� �������
        options.Limits.MaxRequestBodySize = size * 1024 * 1024;

        // ��������� ������
        options.ListenAnyIP(httpPort); // HTTP
        // options.ListenAnyIP(httpsPort); // HTTPS
    });
    #endregion

    #region Logging
    // LayoutRenderer.Register<LayoutRendererWrapper>("intercept");
    var logger = NLog.LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();

    builder.Logging.ClearProviders();
    builder.Host.UseNLog();

    logger.Info("����������� ��������");
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
            Description = "����������� Bearer-����� ��� ����������� ����� ���������. Example: \"Authrization: {token}\"",
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
        options.RequireHttpsMetadata = false; // ��������� ���������� HTTPS
        options.SaveToken = true;
        // �������� ���������� ������
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = "ERMIT", // �������� ������
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
    // ������������ ������ ��������� ��������� timestamp � postgres
    AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

    //������ ������� ��������
    LoggerFactory dbloggerFactory = new(new[] { new NLogLoggerProvider() });

    // ��������� ����������� ������� ����� NpgSQL ��� JSON
    NpgsqlConnection.GlobalTypeMapper.UseJsonNet(settings: new()
    {
        Formatting = Formatting.None,
        ContractResolver = new Newtonsoft.Json.Serialization.DefaultContractResolver
        {
            NamingStrategy = new Newtonsoft.Json.Serialization.CamelCaseNamingStrategy()
        }
    });

    // ��������� DbContext 
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

    #region Behavior
    builder.Services.AddMvcCore();
    #endregion

    #region Services

    // ����������� ����� ������������ ����������

    // ������������ �������
    builder.Services.AddScoped<AuthService>();

    builder.Services.AddTransient<AchievementService>();
    builder.Services.AddTransient<LocationService>();
    builder.Services.AddTransient<ShowplaceService>();
    builder.Services.AddTransient<StandService>();
    builder.Services.AddTransient<UserAchievementService>();
    // builder.Services.AddTransient<>();
    

    // ��������� hhtp-������������
    builder.Services.AddHttpLogging(options =>
    {
        options.LoggingFields = HttpLoggingFields.RequestHeaders |
                                HttpLoggingFields.RequestBody |
                                HttpLoggingFields.ResponseHeaders |
                                HttpLoggingFields.ResponseBody;
    });

    builder.Services.AddControllersWithViews(); // ���������� ����� ��� ������ � ������������� � ���������������
    #endregion

    #region CORS
    // ��������� ��������� CORS
    builder.Services.AddCors(options =>
    {
        options.AddPolicy("AllowSpecificOrigin",
            builder =>
            {
                builder.WithOrigins("http://*")
                        .AllowAnyHeader()
                        .AllowAnyMethod();
            });
    });
    #endregion

    var app = builder.Build();

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment() || app.Configuration.GetValue<bool>("enable_swagger"))
    {
        app.UseDeveloperExceptionPage();

        app.UseSwagger();
        app.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint("../swagger/v1.0/swagger.json", "v1.0");
        });

        // ����� ������ ReriteOptions
        RewriteOptions redirections = new();
        // ��������� ������� ���������������
        redirections.AddRedirect("^$", "swagger");
        // ������������ �������� ������� ���������������
        app.UseRewriter(redirections);
    }

    app.UseHttpsRedirection();
    app.UseRouting();

    app.UseHttpLogging();
    app.UseCors("AllowSpecificOrigin");

    app.UseAuthentication();
    app.UseAuthorization();

    app.MapControllers();

    app.UseEndpoints(endpoints =>
    {
        endpoints.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}");
    });

    // app.UseErrorHandlingMiddleware();

    app.Lifetime.ApplicationStarted.Register(() =>
    {
        app.Logger.LogInformation("���������� ��������.");
    });

    app.Lifetime.ApplicationStopped.Register(() =>
    {
        app.Logger.LogInformation("���������� �����������.");
    });


    await app.RunAsync();
}
catch (Exception ex)
{
    NLog.LogManager.GetCurrentClassLogger().Error(ex, "������ ��� ������� ����������");
}
finally
{
    NLog.LogManager.Shutdown();
}