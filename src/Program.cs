using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using TaskManagement.src.Infrastructure.Options;
using TaskManagement.src.Domain.Models;
using TaskManagement.src.Infrastructure.Database;
using TaskManagement.src.Domain.Services;
using TaskManagement.src.Presentation.Middlewares;
using TaskManagement.src.Presentation.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(
    options => options.UseNpgsql(builder.Configuration.GetConnectionString("Postgres"))
);

builder.Services.AddScoped<PasswordHasher<User>>();
builder.Services.AddScoped<UsersService>();
builder.Services.AddScoped<DutiesService>();
builder.Services.AddSingleton<IValidateOptions<JwtOptions>, JwtOptionsValidator>();

builder.Services
    .AddOptions<JwtOptions>()
    .BindConfiguration(JwtOptions.SectionName)
    .ValidateOnStart();

builder.Services.AddSingleton<JwtService>();
builder.Services.AddControllers();
builder.Services.AddOpenApi();

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer();

builder.Services
    .AddOptions<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme)
    .Configure<JwtService>((options, jwtService) =>
    {
        options.TokenValidationParameters = jwtService.GetTokenValidationParameters();
    });

builder.Services.AddAuthorization();

var app = builder.Build();

app.UseMiddleware<ExceptionHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
