namespace Quartz_Demo_Api;

using NLog;
using NLog.Web;
using Quartz_Demo_Api.Jobs;
using Quartz;
using Quartz.AspNetCore;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        var logger = CreateLoger(builder);

        logger.Info("App Start.");

        var services = builder.Services;

        // Add services to the container.
        services.AddAuthorization();

        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();

        services.AddQuartz(q =>
        {
            // base Quartz scheduler, job and trigger configuration

            // Just use the name of your job that you created in the Jobs folder.
            var jobKey = new JobKey("SampleJob");
            q.AddJob<SampleJob>(opts => opts.WithIdentity(jobKey));

            q.AddTrigger(opts => opts
                                 .ForJob(jobKey)
                                 .WithIdentity("SampleJob-trigger")
                                 //This Cron interval can be described as "run every minute" (when second is zero)
                                 .WithCronSchedule("0 * * ? * *")
            );
        });

        // ASP.NET Core hosting
        services.AddQuartzServer(options =>
        {
            // when shutting down we want jobs to complete gracefully
            options.WaitForJobsToComplete = true;
        });

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.UseAuthorization();

        var summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        app.MapGet("/weatherforecast", (HttpContext httpContext) =>
           {
               var forecast = Enumerable.Range(1, 5).Select(index =>
                                            new WeatherForecast
                                            {
                                                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                                                TemperatureC = Random.Shared.Next(-20, 55),
                                                Summary = summaries[Random.Shared.Next(summaries.Length)]
                                            })
                                        .ToArray();
               return forecast;
           })
           .WithName("GetWeatherForecast")
           .WithOpenApi();

        app.Run();
    }

    private static Logger CreateLoger(WebApplicationBuilder builder)
    {
        builder.Logging.ClearProviders();

        builder.Host.UseNLog();

        return LogManager.Setup().LoadConfigurationFromFile($"nlog.config").GetCurrentClassLogger();
    }
}