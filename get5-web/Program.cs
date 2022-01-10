using AspNetCore.Authentication.CAS;
using AspNetCore.Identity.Mongo;
using AspNetCore.Identity.Mongo.Model;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddIdentityMongoDbProvider<MongoUser>(mongo =>
{
    mongo.ConnectionString = "mongodb://127.0.0.1:27017";
});

builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = CasDefaults.AuthenticationScheme.SCHEME;
        options.DefaultChallengeScheme = CasDefaults.AuthenticationScheme.SCHEME;
        options.DefaultSignInScheme = CasDefaults.AuthenticationScheme.SCHEME;
    })
    .AddCookie()
    .AddCas(casOptions =>
    {
        casOptions.CasServerUrlBase = "https://shib.idm.umd.edu/shibboleth-idp/profile/cas";
        casOptions.Renew = false;
        // casOptions.SaveTokens = true;
        casOptions.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;

        casOptions.CorrelationCookie = new CookieBuilder
        {
            HttpOnly = false,
            SameSite = SameSiteMode.Strict,
            SecurePolicy = CookieSecurePolicy.SameAsRequest,
            Expiration = TimeSpan.FromMinutes(10)
        };
    });

builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
}

app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller}/{action=Index}/{id?}");

app.MapFallbackToFile("index.html"); ;

app.Run();