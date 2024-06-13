using System.Net;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.StackExchangeRedis;
using Microsoft.Extensions.Options;
using StackExchange.Redis;
using webFerum.Models;
using webFerum.Controllers;
using webFerum.Utils;
using webFerum.Service;
using webFerum.Middleware;
using webFerum.Models.AppContext;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<WebFerumContext>();
builder.Services.AddTransient<UserService>();
builder.Services.AddTransient<RoleService>();
builder.Services.AddTransient<ArticleService>();
builder.Services.AddTransient<ItemService>();
builder.Services.AddTransient<ClientService>();
//builder.Services.AddTransient<TgService>();

string RedisEndPoint = "127.0.0.1:6379";
RedisCacheOptions redisOptions = new RedisCacheOptions()
{
    Configuration = "localhost:6379",
    InstanceName = "redis_serv",
    ConfigurationOptions = new ConfigurationOptions()
    {
        EndPoints = { { RedisEndPoint },
        },
        DefaultDatabase = 0
    },
};

builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = "localhost:6379";
    options.InstanceName = "redis_serv";
    options.ConfigurationOptions = new ConfigurationOptions()
    {
        EndPoints = { { RedisEndPoint },
         },
        DefaultDatabase = 0
    };
    options = redisOptions;
});

builder.Services.AddSession();

builder.Services.AddControllersWithViews();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = AuthOptions.ISSUER,
            ValidateAudience = true,
            ValidAudience = AuthOptions.AUDIENCE,
            ValidateLifetime = true,
            IssuerSigningKey = AuthOptions.GetSymmetricSecurityKey(),
            ValidateIssuerSigningKey = true,

            RoleClaimType = "RolePolicy"
        };
    });


builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Admin", policy => policy.RequireClaim("RolePolicy", new[] { "Admin" }));
    options.AddPolicy("Editor", policy => policy.RequireClaim("RolePolicy", new[] { "Editor", "Admin" }));
    options.AddPolicy("Operator", policy => policy.RequireClaim("RolePolicy", new[] { "Operator", "Admin" }));
    options.AddPolicy("User", policy => policy.RequireClaim("RolePolicy", new[] { "User" }));
    options.AddPolicy("Any", policy => policy.RequireClaim("RolePolicy", new[] { "User", "Admin", "Editor", "Operator" }));
    options.AddPolicy("Empl", policy => policy.RequireClaim("RolePolicy", new[] { "Admin", "Editor", "Operator" }));
});


var app = builder.Build();

app.UseSession();
app.UseMiddleware<JwtMiddleware>();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseStatusCodePages(async context =>
{
    var request = context.HttpContext.Request;
    var response = context.HttpContext.Response;

    if (response.StatusCode == (int)HttpStatusCode.Unauthorized)
    {
        response.Redirect("/Auth");
    }
    else if (response.StatusCode == (int)HttpStatusCode.NotFound)
    {
        response.Redirect("/");
    }
    else if (response.StatusCode == (int)HttpStatusCode.Forbidden)
    {
        response.Redirect("/");
    }
});

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");


WebFerumContext ctx = new WebFerumContext();
RedisCache cache = new RedisCache(Options.Create(redisOptions));
TgService telegram = new TgService(ctx, cache, new UserService(ctx, cache), new ItemService(ctx, cache));


app.Run();
