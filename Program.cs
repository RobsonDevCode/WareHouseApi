using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using NLog;
using NLog.Config;
using NLog.Extensions.Logging;
using NLog.Targets;
using SelfProjectApi.Configuration;
using SelfProjectApi.Extentions.SQLExtentions;
using SelfProjectApi.Models.Sales;
using SelfProjectApi.Processing.OrderProcessing;
using SelfProjectApi.Processing.OrderProcessing.OrderCaching;
using SelfProjectApi.Repository;
using System.Reflection;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
//setting up logging we would use a database but i dont have one
var config = new LoggingConfiguration();
var appBaseDir = AppContext.BaseDirectory;
var logDirPath = Path.Combine(appBaseDir, "logs");
var logFilePath = System.IO.Path.Combine(appBaseDir, "logs", "logfile.csv");

//ensure log directory exits 
if(!Directory.Exists(logDirPath))
{
    Directory.CreateDirectory(logDirPath);
}

var csvFileTarget = new FileTarget("csvFile")
{
    FileName = logFilePath,
    Layout = "${longdate},${level},${message},${exception:format=tostring}",
    KeepFileOpen = true,
    ConcurrentWrites = false, // Improves performance for CSV logging
};
//add target location
config.AddTarget(csvFileTarget);

config.AddRule(NLog.LogLevel.Info, NLog.LogLevel.Fatal, csvFileTarget);

LogManager.Configuration = config;


//Add Nlog to logging
builder.Logging.AddNLog();


builder.Configuration.SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile($"appsettings.json", optional: false, reloadOnChange: true)
                    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
                    .AddEnvironmentVariables();

//Add JWT authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).
AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration.GetValue<string>("JwtSettings:Issuer"), 
        ValidAudience = builder.Configuration.GetValue<string>("JwtSettings:Audience"),
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration.GetValue<string>("JwtSettings:Key"))),
        ClockSkew = TimeSpan.Zero 
    };
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy(Settings.AdminUserPolicyName, p =>
    {
        p.RequireRole(Settings.AdminUserClaimName, "true");
    });
});
//set up our lazy evaluation congfiguration class  
Settings.Initilize(builder.Configuration);


// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//Dependcy Injections 
builder.Services.AddTransient<IOrderApiCallCaching, OrderApiCallCaching>();
builder.Services.AddSingleton<OrderMemoryCache>();
builder.Services.AddTransient<IOrderProcessing, OrderProcessing>();
builder.Services.AddTransient<IOrderSQLAccess, OrderSQLAccess>();
builder.Services.AddTransient<ISQLExtentions, SQLExtentions>();

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "Self Project Api"
    });

    // Set the comments path for the Swagger JSON and UI so we can see it on swagger UI.
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    options.IncludeXmlComments(xmlPath);
});


//configure cors this may be unnecessary for this project but for a production product "WithOrigins" is likley to have more values.
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigins",
                      policy =>
                      {
                          policy
                          .WithOrigins("http://localhost:4200")
                          .AllowAnyHeader()
                          .AllowAnyMethod()
                          .AllowCredentials();
                      });
});

var app = builder.Build();

app.UseRouting();
app.UseCors("AllowSpecificOrigins");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();


app.MapControllers();

app.Run();
