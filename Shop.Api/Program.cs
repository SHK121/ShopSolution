using Shop.Api.Middlewares;
using Shop.Application;
using Shop.Infrastructure;
using Microsoft.OpenApi;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Shop.Infrastructure.Persistence;
using Shop.Application.Interfaces;
using Shop.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter your JWT token."
    });

    options.AddSecurityRequirement(document =>
        new OpenApiSecurityRequirement
        {
            [new OpenApiSecuritySchemeReference("Bearer", document)] = []
        });
});
builder.Services.AddControllers();
builder.Services.AddCors(options =>
{
    options.AddPolicy("angular", policy =>
    {
        policy.WithOrigins("http://localhost:4200")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});


//Console.WriteLine(
    //builder.Configuration.GetConnectionString("DefaultConnection (program.cs)"));
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApplication();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Events = new JwtBearerEvents
        {
            OnAuthenticationFailed = context =>
            {
                Console.WriteLine("===== AUTH FAILED =====");
                //Console.WriteLine(context.Exception.ToString());
                Console.WriteLine("JWT authentication failed.");

                return Task.CompletedTask;
            },

            OnTokenValidated = context =>
            {
                Console.WriteLine("===== TOKEN VALIDATED =====");

                return Task.CompletedTask;
            },

            OnChallenge = context =>
            {
                Console.WriteLine("===== JWT CHALLENGE =====");
                Console.WriteLine($"Error: {context.Error}");
                Console.WriteLine($"ErrorDescription: {context.ErrorDescription}");

                return Task.CompletedTask;
            }
        };
        var jwtSettings = builder.Configuration.GetSection("Jwt");
        var key = jwtSettings["Key"] ?? throw new InvalidOperationException("JWT Key is missing");

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = jwtSettings["Issuer"],

            ValidateAudience = true,
            ValidAudience = jwtSettings["Audience"],

            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),

            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
    });
builder.Services.AddAuthorization();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ShopDbContext>();

    var passwordHasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();

    await context.Database.MigrateAsync();
    await DatabaseInitializer.InitializeAsync(context, passwordHasher);
}

/*using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ShopDbContext>();
    var passwordHasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();
    await DatabaseSeeder.SeedAsync(context, passwordHasher);
}*/

app.UseMiddleware<ExceptionMiddleware>();

app.Use(async (context, next) =>
{
    Console.WriteLine(
        $"===== REQUEST: {context.Request.Method} {context.Request.Path} =====");

   // Console.WriteLine(
  //      $"Authorization: {context.Request.Headers.Authorization}");

    await next();
});

app.UseHttpsRedirection();
app.UseCors("angular");
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.Run();

public partial class Program { } // for integration tests