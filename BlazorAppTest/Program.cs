using Serilog.Events;
using Serilog;
using BlazorAppTest.Services;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
    //.MinimumLevel.Override("Microsoft", LogEventLevel.Information)
    .WriteTo.Console()
    //.WriteTo.Debug()
    .CreateBootstrapLogger();


try
{
    var builder = WebApplication.CreateBuilder(args);

    // Add services to the container.
    builder.Services.AddRazorPages();
    builder.Services.AddServerSideBlazor();

    builder.Services.AddHttpClient<IClienteService, ClienteService>(
    client =>
    {
        client.BaseAddress = new Uri("https://localhost:7242");
    });

    builder.Services.AddHttpClient<TokenService>();

    
    var app = builder.Build();

    // Configure the HTTP request pipeline.
    if (!app.Environment.IsDevelopment())
    {
        app.UseExceptionHandler("/Error");
        // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
        app.UseHsts();
    }

    app.UseHttpsRedirection();

    app.UseStaticFiles();

    app.UseRouting();

    app.MapBlazorHub();
    app.MapFallbackToPage("/_Host");

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}