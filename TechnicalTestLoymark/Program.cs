using back_end.Controllers;
using back_end.Helpers;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<ApiSettings>(builder.Configuration.GetSection("ApiSettings"));
builder.Services.AddHttpClient<UserController>((serviceProvider, client) =>
{
    var apiSettings = serviceProvider.GetRequiredService<IOptions<ApiSettings>>().Value;
    client.BaseAddress = new Uri(apiSettings.BaseUrl);  // Set the base URL for the API
});

// Add services to the container
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
