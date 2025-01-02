using CRUDOperations;
using CRUDOperations.Authentication;
using CRUDOperations.Data;
using CRUDOperations.Filters;
using CRUDOperations.Middlewares;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile("Config.json");

// 1
//var attachmentOptions = builder.Configuration.GetSection("Attachment").Get<AttachmentOptions>();
//builder.Services.AddSingleton(attachmentOptions);
// 2
//var attachmentOptions = new AttachmentOptions();
//builder.Configuration.GetSection("Attachment").Bind(attachmentOptions);
//builder.Services.AddSingleton(attachmentOptions);
// 3 options pattern
builder.Services.Configure<AttachmentOptions>(builder.Configuration.GetSection("Attachment"));


// Add services to the DI container.

builder.Services.AddControllers(options =>
{
    options.Filters.Add<LogActivityFilter>();
});
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<ApplicationDbContext>(
    cfg => cfg.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
    );

// Add Basic Authentication
//builder.Services.AddAuthentication()
//    .AddScheme<AuthenticationSchemeOptions, BasicAuthenticationHandler>("Basic", null);

// Add Bearer Authentication
var jwtOptions = builder.Configuration.GetSection("Jwt").Get<JwtOptions>();
builder.Services.AddSingleton(jwtOptions);
builder.Services.AddAuthentication()
    .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
    {
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = jwtOptions.Issuer,
            ValidateAudience = true,
            ValidAudience = jwtOptions.Audience,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.SigningKey))
        };
    });


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<RateLimitMiddleware>();
app.UseMiddleware<ProfilingMiddleware>();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
