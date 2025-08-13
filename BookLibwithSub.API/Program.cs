using System.Text;
using BookLibwithSub.Repo;                // <-- DbContext
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// -------------------------------
// 1) Infrastructure wiring
// -------------------------------

// EF Core DbContext (Code-First)
builder.Services.AddDbContext<AppDbContext>(opts =>
    opts.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// CORS (allow your FE origins)
const string CorsPolicy = "AppCors";
var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins")
    .Get<string[]>() ?? new[] { "http://localhost:5173" };

builder.Services.AddCors(o => o.AddPolicy(
    CorsPolicy,
    p => p.WithOrigins(allowedOrigins).AllowAnyHeader().AllowAnyMethod()
));

// JWT Authentication (minimal, easy to extend later)
var jwtKey = builder.Configuration["Jwt:Key"] ?? "dev-secret-key-change-me";
var jwtIssuer = builder.Configuration["Jwt:Issuer"] ?? "BookLibIssuer";
var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(opt =>
    {
        opt.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = jwtIssuer,

            ValidateAudience = false, // no specific audience for now
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = signingKey,

            ValidateLifetime = true
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", p => p.RequireRole("Admin"));
    options.AddPolicy("LibrarianOrAdmin", p => p.RequireRole("Librarian", "Admin"));
});

// MVC + Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "BookLibWithSub API", Version = "v1" });

    // Enable "Authorize" button in Swagger (Bearer)
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
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        { securityScheme, new string[] { } }
    });
});

var app = builder.Build();

// -------------------------------
// 2) HTTP pipeline
// -------------------------------

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    // Generic handler for production errors
    app.UseExceptionHandler("/error");
}

// Swagger (enable in all envs for now)
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "BookLibWithSub API v1");
});

// Force HTTPS if you terminate TLS at the app (keep if applicable)
app.UseHttpsRedirection();

// CORS before auth
app.UseCors(CorsPolicy);

// AuthZ stack
app.UseAuthentication();
app.UseAuthorization();

// Controllers
app.MapControllers();

// Root health check
app.MapGet("/", () => "BookLibWithSub API is running 🚀");

app.Run();
