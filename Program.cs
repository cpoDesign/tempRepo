using Microsoft.FeatureManagement;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Register feature management from a specific configuration.
builder.Services.AddFeatureManagement();
// filter the configuration to subsection (note this needs to be in sync with app config in used)
// builder.Services.AddFeatureManagement(builder.Configuration.GetSection("MyFeatureFlags"));

var connectionString = builder.Configuration.GetConnectionString("AppConfig");

builder.Host.ConfigureAppConfiguration(builder =>
{
    //Connect to your App Config Store using the connection string
    builder.AddAzureAppConfiguration(options=>options
                                                .Connect(connectionString)
                                                .UseFeatureFlags(featureFlagOptions =>
                                                {
                                                    featureFlagOptions.Label = "online";
                                                    featureFlagOptions.CacheExpirationInterval = TimeSpan.FromMinutes(5);
                                                })
                                    );
})
.ConfigureServices(services =>
{
    services.AddControllersWithViews();
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");

    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();

    // register the configuration for app configuration
    app.UseAzureAppConfiguration();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
