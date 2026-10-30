using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Unimag.Inventario.Api.Common.Middleware;
using Unimag.Inventario.Api.Common.Tenancy;
using Unimag.Inventario.Domain.Common;
using Unimag.Inventario.Application;
using Unimag.Inventario.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentTenant, CurrentTenant>();

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var allowedCorsOrigins = builder.Configuration
    .GetSection("Cors:AllowedOrigins")
    .Get<string[]>() ?? [];

builder.Services.AddCors(options =>
{
    options.AddPolicy("Frontend", policy =>
    {
        policy.WithOrigins(allowedCorsOrigins)
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

// Each layer wires up its own dependencies via an extension method.
// Api never new()s a repository, a DbContext or a use case directly.
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

// JWT authentication
var jwtKey = builder.Configuration["Jwt:Key"] ?? "dev-secret-key-change-me-please-32chars!!";
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"] ?? "Unimag.Inventario",
            ValidAudience = builder.Configuration["Jwt:Audience"] ?? "Unimag.Inventario",
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
        };
    });

builder.Services.AddAuthorization();

var app = builder.Build();

// Configure the HTTP request pipeline.

// Registered first so it wraps every other middleware and controller —
// any exception thrown anywhere downstream is caught and mapped here.
app.UseExceptionHandling();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

// The HTTP development profile does not expose an HTTPS endpoint, so there is
// no valid target for the redirection middleware in that case.
if (!app.Environment.IsDevelopment() ||
    app.Urls.Any(url => url.StartsWith("https://", StringComparison.OrdinalIgnoreCase)))
{
    app.UseHttpsRedirection();
}

    app.UseCors("Frontend");

    app.UseAuthentication();
    app.UseAuthorization();

app.MapControllers();

app.Run();