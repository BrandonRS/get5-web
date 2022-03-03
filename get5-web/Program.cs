using AspNetCore.Authentication.CAS;
using AspNetCore.Identity.Mongo;
using get5_web.Interfaces.Authentication;
using get5_web.Models.Authentication;
using get5_web.Services.Authentication;
using Microsoft.AspNetCore.Identity;
using MongoDB.Driver;

var builder = WebApplication.CreateBuilder(args);

// Grab mongo host and port
var host = builder.Configuration.GetSection("MongoDb:Host").Value;
var port = builder.Configuration.GetSection("MongoDb:Port").Value;
var databaseName = builder.Configuration.GetSection("MongoDb:Database").Value;
var mongoConnectionString = $"mongodb://{host}:{port}/{databaseName}";

// Create the mongo connection string using the host and port values
var mongoClient = new MongoClient(mongoConnectionString);
var database = mongoClient.GetDatabase("get5");
builder.Services.AddSingleton(databaseName);

// Add services to the container.
builder.Services
    .AddIdentityMongoDbProvider<User>(options =>
    {
        options.ConnectionString = mongoConnectionString;
    });

builder.Services
    .AddAuthentication()
    .AddCas(casOptions =>
    {
        casOptions.CallbackPath = "/api/auth/signin-cas";
        casOptions.CasServerUrlBase = "https://shib.idm.umd.edu/shibboleth-idp/profile/cas/";
        casOptions.CasValidationUrl = "https://shib.idm.umd.edu/shibboleth-idp/profile/cas/serviceValidate";
        casOptions.RemoteAuthenticationTimeout = TimeSpan.FromMinutes(5);
        casOptions.Renew = false;
        casOptions.SignInScheme = IdentityConstants.ExternalScheme;

        casOptions.CorrelationCookie = new CookieBuilder
        {
            HttpOnly = false,
            // SameSite = SameSiteMode.Strict,
            // SecurePolicy = CookieSecurePolicy.SameAsRequest,
            Expiration = TimeSpan.FromMinutes(10)
        };
    })
    .AddSteam("Steam", steamOptions =>
    {
        steamOptions.CallbackPath = "/api/auth/signin-steam";
        steamOptions.SignInScheme = IdentityConstants.ExternalScheme;
    });

builder.Services.AddScoped<IAuthService, AuthService>();

builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller}/{action=Index}/{id?}");

app.MapFallbackToFile("index.html");

app.Run();