using MongoDB.Bson;
using MongoDB.Driver;

var builder = WebApplication.CreateBuilder(args);

// Grab host and port
var host = builder.Configuration.GetSection("MongoDb:Host").Value;
var port = builder.Configuration.GetSection("MongoDb:Port").Value;

// Create the connection string using the host and port values
var mongoClient = new MongoClient($"mongodb://{host}:{port}");
var database = mongoClient.GetDatabase("get5");
builder.Services.AddSingleton(database);

// Add services to the container.

builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
}

app.UseStaticFiles();
app.UseRouting();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller}/{action=Index}/{id?}");

app.MapFallbackToFile("index.html"); ;

app.Run();