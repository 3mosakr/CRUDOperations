using CRUDOperations;
using CRUDOperations.Authentication;
using CRUDOperations.Data;
using CRUDOperations.Filters;
using CRUDOperations.Middlewares;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

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

// 
builder.Services.AddAuthentication()
    .AddScheme<AuthenticationSchemeOptions, BasicAuthenticationHandler>("Basic", null);


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
