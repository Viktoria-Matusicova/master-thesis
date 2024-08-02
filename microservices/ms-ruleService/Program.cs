using ruleService.Services;
using ruleService.Interfaces;
using ruleService.Repositories;
using ruleService.Consumers;
using ruleService.Models;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using SocketIOClient;

//Create configuration from json file
IConfiguration configuration = new ConfigurationBuilder()
.SetBasePath(Directory.GetCurrentDirectory())
                            .AddJsonFile("appsettings.json")
                            .Build();
ConfigurationSettings myConfig = new ConfigurationSettings();
configuration.GetSection("AppSettings").Bind(myConfig);

var builder = WebApplication.CreateBuilder(args);

//Get environment variables from docker secrets
var sql_password = Environment.GetEnvironmentVariable("SQL_PASSWORD");
var sql_username = Environment.GetEnvironmentVariable("SQL_USERNAME");

// Add controllers
builder.Services.AddControllers();

//Get connection string to database
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// Add services to the container
builder.Services.AddDbContext<AppDbContext>(x => x.UseSqlServer(#connection#), ServiceLifetime.Transient, ServiceLifetime.Transient);
builder.Services.AddSingleton<SocketIO>(new SocketIO(#connection#));
builder.Services.AddHostedService<SocketStartupService>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddTransient<IParentRepository, ParentRepository>();
builder.Services.AddTransient<IRuleRepository, RuleRepository>();
builder.Services.AddSingleton<IRuleService, RuleService>();
builder.Services.AddTransient<IParentService, ParentService>();
builder.Services.AddTransient<IMatrixService, MatrixService>();
builder.Services.AddSingleton<RuleConsumer>();
builder.Services.AddTransient<YamlDotNet.Serialization.Deserializer>();
builder.Services.AddSingleton<IConfigurationSettings>(myConfig);

//Add custom logger
using var loggerFactory = LoggerFactory.Create(loggingBuilder => loggingBuilder
    .SetMinimumLevel(LogLevel.Trace)
    .AddSimpleConsole(c =>
                        {
                            c.TimestampFormat = "[HH:mm:ss] ";
                            c.ColorBehavior = Microsoft.Extensions.Logging.Console.LoggerColorBehavior.Enabled;
                        })
    );

//Add CORS policy
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: "AllowLocalhost",
                      policy =>
                      {
                          policy.AllowAnyOrigin()
                                .AllowAnyMethod()
                                .AllowAnyHeader();
                      });
});

//Add MassTransit
builder.Services.AddMassTransit(x =>
{
    x.AddBus(provider => Bus.Factory.CreateUsingRabbitMq(config =>
    {
        var settings = configuration.GetSection("EventBusSettings").Get<RabbitMQSettings>();
        config.Host(new Uri(settings.HostAddress), h =>
        {
            h.Username(settings.Username);
            h.Password(settings.Password);
        });
        config.ReceiveEndpoint(settings.Queue, ep =>
        {
            ep.UseRawJsonDeserializer();
            ep.PrefetchCount = 1;
            ep.SetQueueArgument("x-single-active-consumer", true);
            ep.Consumer<RuleConsumer>(provider);
        });
    }));
    x.AddConsumer<RuleConsumer>();
});

var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowLocalhost");
app.MapControllers();

app.Run("#connection#");