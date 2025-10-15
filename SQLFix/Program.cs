using SQLFix.Data;
using SQLFix.Jobs;
using MediatR;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Quartz;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddRazorPages();

builder.Services.AddDbContext<WriteDbContext>(opt =>
    opt.UseNpgsql(builder.Configuration.GetConnectionString("WriteDB")));

builder.Services.AddDbContext<ReadDbContext>(opt =>
    opt.UseNpgsql(builder.Configuration.GetConnectionString("ReadDb")));


builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssemblies(Assembly.GetExecutingAssembly());
});

builder.Services.AddQuartz(q =>
{
    q.UseMicrosoftDependencyInjectionJobFactory();

    var jobKey = new JobKey("DataSyncJob");

    q.AddJob<DataSyncJob>(opts => opts.WithIdentity(jobKey));


    q.AddTrigger(opts => opts
        .ForJob(jobKey)
        .WithIdentity("DataSyncJob-trigger")
        .StartNow()
        .WithSimpleSchedule(x => x
            .WithIntervalInSeconds(3)
            .RepeatForever()));
});


builder.Services.AddQuartzHostedService(options =>
{
    options.WaitForJobsToComplete = true;
});


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();

app.MapControllers();


Console.WriteLine("[Program] Uygulama başladı, Quartz yüklendi.");

app.Run();
