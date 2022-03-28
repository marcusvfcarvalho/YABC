using Microsoft.EntityFrameworkCore;

using Microsoft.Extensions.DependencyInjection;
using YABC;
using YABC.Data;
using YABC.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<YABCContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("YABCContext")));

// Add services to the container.
builder.Services.AddHostedService<MiningHostService>();
builder.Services.AddControllersWithViews();
builder.Services.AddSignalR();
builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy", builder => builder
    .WithOrigins("http://localhost:44402")
    .AllowAnyMethod()
    .AllowAnyHeader()
    .AllowCredentials());
});

builder.Services.AddScoped<IBlockService, BlockService>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseCors("CorsPolicy");


app.MapControllerRoute(
    name: "default",
    pattern: "{controller}/{action=Index}/{id?}");

app.MapFallbackToFile("index.html");
app.MapHub<BlockHub>("/blockHub");

app.Run();
