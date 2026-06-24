using LeaveManagement.API.Data;
using LeaveManagement.API.Middleware;
using LeaveManagement.API.Services;
using LeaveManagement.API.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// ══════════════════════════════════════════════════════════════
// 1. DATABASE
// ══════════════════════════════════════════════════════════════
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        npgsqlOptions => npgsqlOptions.EnableRetryOnFailure(
            maxRetryCount: 3,
            maxRetryDelay: TimeSpan.FromSeconds(5),
            errorCodesToAdd: null)
    )
);

// ══════════════════════════════════════════════════════════════
// 2. JWT AUTHENTICATION
// ══════════════════════════════════════════════════════════════
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey   = jwtSettings["SecretKey"]
    ?? throw new InvalidOperationException("JWT SecretKey is not configured.");

builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme    = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer           = true,
            ValidateAudience         = true,
            ValidateLifetime         = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer              = jwtSettings["Issuer"],
            ValidAudience            = jwtSettings["Audience"],
            IssuerSigningKey         = new SymmetricSecurityKey(
                                           Encoding.UTF8.GetBytes(secretKey)),
            ClockSkew                = TimeSpan.Zero
        };

        options.Events = new JwtBearerEvents
        {
            OnChallenge = async context =>
            {
                context.HandleResponse();
                context.Response.StatusCode  = 401;
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsync(
                    "{\"message\":\"Unauthorized. Please provide a valid token.\"}");
            },
            OnForbidden = async context =>
            {
                context.Response.StatusCode  = 403;
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsync(
                    "{\"message\":\"Forbidden. You do not have permission for this action.\"}");
            }
        };
    });

builder.Services.AddAuthorization();

// ══════════════════════════════════════════════════════════════
// 3. CORS — allow React dev server (Vite runs on port 5173)
// ══════════════════════════════════════════════════════════════
builder.Services.AddCors(options =>
{
    options.AddPolicy("ReactApp", policy =>
        policy
            .WithOrigins(
                "http://localhost:5173",
                "http://localhost:3000"
            )
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials()
    );
});

// ══════════════════════════════════════════════════════════════
// 4. APPLICATION SERVICES (Dependency Injection)
// ══════════════════════════════════════════════════════════════
builder.Services.AddScoped<JwtService>();
builder.Services.AddScoped<IAuthService,         AuthService>();
builder.Services.AddScoped<IEmployeeService,     EmployeeService>();
builder.Services.AddScoped<ILeaveRequestService, LeaveRequestService>();

// ══════════════════════════════════════════════════════════════
// 5. CONTROLLERS
// ══════════════════════════════════════════════════════════════
builder.Services.AddControllers()
    .AddJsonOptions(opts =>
    {
        opts.JsonSerializerOptions.Converters.Add(
            new System.Text.Json.Serialization.JsonStringEnumConverter());
    });

// ══════════════════════════════════════════════════════════════
// 6. SWAGGER WITH JWT SUPPORT
// ══════════════════════════════════════════════════════════════
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title       = "Leave Management API",
        Version     = "v1",
        Description = "Employee Leave Management System — JWT authenticated REST API.\n\n" +
                      "**Roles:** Admin (full access) | Employee (own data only)\n\n" +
                      "**Auth:** Login via POST /api/auth/login, then click Authorize and paste: Bearer {token}"
    });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description  = "JWT Authorization header. Format: **Bearer {your_token_here}**",
        Name         = "Authorization",
        In           = ParameterLocation.Header,
        Type         = SecuritySchemeType.ApiKey,
        Scheme       = "Bearer",
        BearerFormat = "JWT"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            Array.Empty<string>()
        }
    });
});

// ══════════════════════════════════════════════════════════════
// BUILD & CONFIGURE MIDDLEWARE PIPELINE
// ══════════════════════════════════════════════════════════════
var app = builder.Build();

app.UseMiddleware<ExceptionMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Leave Management API v1");
        c.RoutePrefix            = "swagger";
        c.DisplayRequestDuration();
        c.EnableDeepLinking();
    });
}

app.UseHttpsRedirection();
app.UseCors("ReactApp");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
