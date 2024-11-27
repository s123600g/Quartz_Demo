namespace Quartz_Demo_Api;

using System.Collections.Specialized;
using NLog;
using NLog.Web;
using Quartz_Demo_Api.Jobs;
using Quartz_Demo_Api.Listener;
using Quartz;
using Quartz.AspNetCore;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        var config = builder.Configuration;

        var logger = CreateLoger(builder);

        logger.Info("App Start.");

        var services = builder.Services;

        // Add services to the container.
        // services.AddAuthorization();
        services.AddControllers();

        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();

        // base configuration for DI, read from appSettings.json
        services.Configure<QuartzOptions>(config.GetSection("Quartz"));

        // if you are using persistent job store, you might want to alter some options
        services.Configure<QuartzOptions>(options =>
        {
            options.Scheduling.IgnoreDuplicates = true; // default: false
            options.Scheduling.OverWriteExistingData = true; // default: true
        });
        
        services.AddQuartz(q =>
        {
            // base Quartz scheduler, job and trigger configuration

            q.UsePersistentStore(store =>
            {
                store.UseProperties = true;

                store.UseSystemTextJsonSerializer();
            });

            q.AddJobListener<MyJobListener>();
        });

        // ASP.NET Core hosting
        services.AddQuartzServer(options =>
        {
            // when shutting down we want jobs to complete gracefully
            options.WaitForJobsToComplete = true;
        });

        var app = builder.Build();


        app.UseRouting();
        app.MapControllers();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.Run();
    }

    private static Logger CreateLoger(WebApplicationBuilder builder)
    {
        builder.Logging.ClearProviders();

        builder.Host.UseNLog();

        return LogManager.Setup().LoadConfigurationFromFile($"nlog.config").GetCurrentClassLogger();
    }
}