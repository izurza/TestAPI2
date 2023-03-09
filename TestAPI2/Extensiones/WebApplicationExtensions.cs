namespace TestAPI2.Extensiones;

public static class WebApplicationExtensions
{
    public static WebApplication MapSwagger(this WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }
        return app;
    }
}
