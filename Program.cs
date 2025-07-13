using Microsoft.EntityFrameworkCore;
using procurementsystem.Data;
using procurementsystem.IService;
using procurementsystem.middleware;
using procurementsystem.Services;
using procurementsystem.utils;
using Scalar.AspNetCore;
using DotNetEnv;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);


Env.Load();
var smtpSettings = new SmtpSettings();
builder.Configuration.Bind("SMTP", smtpSettings);
builder.Services.AddSingleton<IEmailService>(new EmailService(smtpSettings));
builder.Services.AddScoped<ICloudinaryService, CloudinaryService>();
builder.Services.AddScoped<IProcurementItem, ProcurementItemService>();

builder.Services.AddScoped<IUser, UserService>();
builder.Services.AddScoped<IAuth, AuthService>();

// Configure JWT settings from environment variables with better error handling
var jwtSettings = new JwtSettings
{
    Secret = Environment.GetEnvironmentVariable("JWT_SECRET") ?? "fallback-secret-key-for-development",
    Issuer = Environment.GetEnvironmentVariable("JWT_ISSUER") ?? "MyProcurementSystem",
    Audience = Environment.GetEnvironmentVariable("JWT_AUDIENCE") ?? "MyProcurementSystemUsers",
    ExpiryMinutes = int.TryParse(Environment.GetEnvironmentVariable("JWT_EXPIRY_MINUTES"), out var expiry) ? expiry : 60
};

builder.Services.Configure<JwtSettings>(options =>
{
    options.Secret = jwtSettings.Secret;
    options.Issuer = jwtSettings.Issuer;
    options.Audience = jwtSettings.Audience;
    options.ExpiryMinutes = jwtSettings.ExpiryMinutes;
});

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen();

// Build connection string from environment variables with better error handling
var dbHost = Environment.GetEnvironmentVariable("DB_HOST");
var dbPort = Environment.GetEnvironmentVariable("DB_PORT") ?? "5432";
var dbName = Environment.GetEnvironmentVariable("DB_NAME");
var dbUser = Environment.GetEnvironmentVariable("DB_USER");
var dbPassword = Environment.GetEnvironmentVariable("DB_PASSWORD");

if (string.IsNullOrEmpty(dbHost) || string.IsNullOrEmpty(dbName) ||
    string.IsNullOrEmpty(dbUser) || string.IsNullOrEmpty(dbPassword))
{
    throw new InvalidOperationException("Database environment variables are missing. Please check your environment configuration.");
}

var connectionString = $"User Id={dbUser};Password={dbPassword};Server={dbHost};Port={dbPort};Database={dbName};Pooling=true;Minimum Pool Size=0;Maximum Pool Size=100;";

builder.Services.AddDbContext<ApplicationDBContext>(options =>
{
    options.UseNpgsql(connectionString);
});

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings.Issuer,
            ValidAudience = jwtSettings.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(
                System.Text.Encoding.UTF8.GetBytes(jwtSettings.Secret)
            )
        };
    });

builder.Services.AddControllers();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<ErrorHandler>();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

// Add graceful shutdown
app.Lifetime.ApplicationStopping.Register(() =>
{
    Console.WriteLine("Application is shutting down...");
});

app.Run();

