using System;
using System.Text;
using System.Text.Json.Serialization;
using BookLibwithSub.Repo;
using BookLibwithSub.Repo.Interfaces;
using BookLibwithSub.Service.Interfaces;
using BookLibwithSub.Service.Models;
using BookLibwithSub.API.Middleware;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using BookLibwithSub.Service.Service;
using BookLibwithSub.Repo.repository;
using Microsoft.Data.SqlClient; // <-- added

var builder = WebApplication.CreateBuilder(args);

// --- Connection string normalization (fix local TLS cert trust) ---
var rawCnn = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? "Server=(localdb)\\MSSQLLocalDB;Database=BookLibDb;Trusted_Connection=True;MultipleActiveResultSets=true";

var csb = new SqlConnectionStringBuilder(rawCnn);

// Always encrypt unless caller explicitly turned it off
// (Newer SQL client defaults to Encrypt=True, but we enforce it to be explicit)
if (!csb.ContainsKey("Encrypt"))
{
    csb.Encrypt = true;
}

// On developer machines, skip CA validation if not already specified.
// This prevents: "The certificate chain was issued by an authority that is not trusted."
if (!csb.ContainsKey("TrustServerCertificate") && builder.Environment.IsDevelopment())
{
    csb.TrustServerCertificate = true;
}

// Helpful default for EF patterns that open multiple readers
if (!csb.ContainsKey("MultipleActiveResultSets"))
{
    csb.MultipleActiveResultSets = true;
}

builder.Services.AddDbContext<AppDbContext>(opts =>
    opts.UseSqlServer(
        csb.ConnectionString,
        sql =>
        {
            // Resilient connections (handy when DB restarts)
            sql.EnableRetryOnFailure();
        }));

// --- DI registrations ---
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ISubscriptionPlanRepository, SubscriptionPlanRepository>();
builder.Services.AddScoped<ISubscriptionPlanService, SubscriptionPlanService>();
builder.Services.AddScoped<ISubscriptionRepository, SubscriptionRepository>();
builder.Services.AddScoped<ITransactionRepository, TransactionRepository>();
builder.Services.AddScoped<IPaymentService, PaymentService>();
builder.Services.AddScoped<ITransactionService, TransactionService>();
builder.Services.AddScoped<ISubscriptionService, SubscriptionService>();
builder.Services.AddScoped<ILoanRepository, LoanRepository>();
builder.Services.AddScoped<ILoanService, LoanService>();
builder.Services.AddScoped<IBookRepository, BookRepository>();

const string CorsPolicy = "AppCors";
var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins")
    .Get<string[]>() ?? new[] { "http://localhost:5173" };

builder.Services.AddCors(o => o.AddPolicy(
    CorsPolicy,
    p => p.WithOrigins(allowedOrigins).AllowAnyHeader().AllowAnyMethod()
));

var jwtOptions = new JwtOptions();
builder.Configuration.GetSection("Jwt").Bind(jwtOptions);
jwtOptions.Key = Environment.GetEnvironmentVariable("JWT__KEY") ?? jwtOptions.Key;
jwtOptions.Issuer = Environment.GetEnvironmentVariable("JWT__ISSUER") ?? jwtOptions.Issuer;

if (string.IsNullOrWhiteSpace(jwtOptions.Key) && !builder.Environment.IsDevelopment())
{
    throw new InvalidOperationException(
        "JWT signing key is not configured. Set env var JWT__KEY or configuration Jwt:Key.");
}

builder.Services.Configure<JwtOptions>(o =>
{
    o.Key = jwtOptions.Key;
    o.Issuer = jwtOptions.Issuer;
});

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = false,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtOptions.Issuer,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.Key ?? string.Empty))
        };
    });

builder.Services.AddAuthorization();

builder.Services.AddControllers().AddJsonOptions(o =>
{
    o.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    o.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "BookLibWithSub API", Version = "v1" });

    var securityScheme = new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Description = "Enter JWT as: Bearer {token}",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
    };
    c.AddSecurityDefinition("Bearer", securityScheme);
    c.AddSecurityRequirement(new OpenApiSecurityRequirement { { securityScheme, Array.Empty<string>() } });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/error");
}

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "BookLibWithSub API v1");
    });
});

app.UseHttpsRedirection();
app.UseCors(CorsPolicy);
app.UseAuthentication();
app.UseMiddleware<TokenValidationMiddleware>();
app.UseAuthorization();

app.MapControllers();
app.MapGet("/", () => "BookLibWithSub API is running");

app.Run();
