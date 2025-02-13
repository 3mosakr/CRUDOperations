using CRUDOperations;
using CRUDOperations.Authentication;
using CRUDOperations.Authorization;
using CRUDOperations.Data;
using CRUDOperations.Filters;
using CRUDOperations.Middlewares;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
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
    options.Filters.Add<PermissionBasedAuthorizationFilter>();
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

builder.Services.AddSingleton<IAuthorizationHandler, AgeAuthorizationHandler>();

builder.Services.AddAuthorization(options =>
{
    // policy Requirement
    options.AddPolicy("AgeGreaterThan25", builder => 
        builder.AddRequirements(new AgeGreaterThan25Requirement()));

    //options.AddPolicy("SuperUsersOnly", builder =>
    //{ // simple custom
    //    builder.RequireAssertion(context => context.User.IsInRole("SuperUser"));
    //});
    //options.AddPolicy("SuperUsersOnly", builder =>
    //{
    //    builder.RequireRole("SuperUser");
    //});
    options.AddPolicy("Employee", builder =>
    {
        builder.RequireClaim("UserType", "Employee");
    });
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
