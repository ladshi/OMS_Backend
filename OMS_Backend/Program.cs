using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using OMS_Backend.Data;
using OMS_Backend.Services;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularApp", policy =>
    {
        policy.WithOrigins("http://localhost:4200")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

// Add Entity Framework
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add JWT Authentication
var jwtKey = builder.Configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT Key not configured");
var jwtIssuer = builder.Configuration["Jwt:Issuer"] ?? "OMS";
var jwtAudience = builder.Configuration["Jwt:Audience"] ?? "OMS";

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtIssuer,
        ValidAudience = jwtAudience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
    };
});

builder.Services.AddAuthorization();

// Add Services
builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddScoped<IEmailService, EmailService>();
// builder.Services.AddScoped<IStripeService, StripeService>(); // Removed Stripe

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    // app.UseSwagger(); // Removed Swagger
    // app.UseSwaggerUI(); // Removed SwaggerUI
}

app.UseHttpsRedirection();

app.UseCors("AllowAngularApp");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Seed initial admin user
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    
    try
    {
        // Try to ensure database is created/updated
        context.Database.EnsureCreated();
        
        // Check if admin user exists
        if (!context.Users.Any(u => u.Role == "Admin"))
        {
            var adminUser = new OMS_Backend.Models.User
            {
                Email = "admin@oms.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@123"),
                Role = "Admin",
                FirstName = "Admin",
                LastName = "User",
                IsActive = true,
                IsPasswordChanged = false // Admin needs to change password on first login
            };
            context.Users.Add(adminUser);
            context.SaveChanges();
        }
    }
    catch (Microsoft.Data.SqlClient.SqlException ex) when (ex.Message.Contains("Invalid column name"))
    {
        // If schema mismatch, drop and recreate database in development
        if (app.Environment.IsDevelopment())
        {
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();
            
            // Recreate admin user after database recreation
            var adminUser = new OMS_Backend.Models.User
            {
                Email = "admin@oms.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@123"),
                Role = "Admin",
                FirstName = "Admin",
                LastName = "User",
                IsActive = true,
                IsPasswordChanged = false
            };
            context.Users.Add(adminUser);
            context.SaveChanges();
        }
        else
        {
            // In production, you should use migrations instead
            throw new InvalidOperationException(
                "Database schema mismatch. Please run migrations to update the database schema.", ex);
        }
    }
}

app.Run();