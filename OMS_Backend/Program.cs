using Microsoft.EntityFrameworkCore;
using OMS_Backend.Data;

var builder = WebApplication.CreateBuilder(args);

// 🔹 Add services
builder.Services.AddControllers();
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
);
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngular",
        policy =>
        {
            policy.WithOrigins("http://localhost:4200", "https://localhost:4200")
                  .AllowAnyMethod()
                  .AllowAnyHeader()
                  .AllowCredentials();
        });
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// 🔹 Build app
var app = builder.Build();

// 🔹 Middleware
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "OMS API v1");
        c.RoutePrefix = string.Empty; // Swagger opens at root
    });
}

app.UseHttpsRedirection();
app.UseCors("AllowAngular");
app.UseAuthorization();

app.MapControllers();

// 🔹 Run app (LAST)
app.Run();
