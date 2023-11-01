using FluentValidation.AspNetCore;
using Hangfire;
using Hangfire.Mongo;
using Hangfire.Mongo.Migration.Strategies;
using Hangfire.Mongo.Migration.Strategies.Backup;
using PoweredSoft.CQRS;
using PoweredSoft.CQRS.AspNetCore.Mvc;
using PoweredSoft.Data;
using PoweredSoft.Data.MongoDB;
using PoweredSoft.DynamicQuery;
using PoweredSoft.Module.Abstractions;
using VPNR.Command;
using VPNR.Dal;
using VPNR.Keycloak;
using VPNR.Linode;
using VPNR.Node;
using VPNR.Query;
using VPNRealms;

var builder = WebApplication.CreateBuilder(args);

//builder.Services.AddVPNRDal();
builder.Services.AddPoweredSoftCQRS();

builder.Services.AddHangfire(configuration =>
{
    configuration.SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
        .UseSimpleAssemblyNameTypeSerializer()
        .UseRecommendedSerializerSettings()
        .UseMongoStorage(builder.Configuration["Hangfire:ConnectionString"], builder.Configuration["Hangfire:Database"], new MongoStorageOptions()
        {
            MigrationOptions = new MongoMigrationOptions()
            {
                MigrationStrategy = new MigrateMongoMigrationStrategy(),
                BackupStrategy = new CollectionMongoBackupStrategy()
            },
            CheckConnection = true
        });
});

// add default queue
builder.Services.AddHangfireServer();
            
// add expensive_reports queue
builder.Services.AddHangfireServer(options =>
{
    options.Queues = new[] { "linode_services" };
});

var mvcBuilder = builder.Services
    .AddControllers();

builder.Services.AddPoweredSoftDataServices(); 
builder.Services.AddPoweredSoftMongoDBDataServices();
builder.Services.AddPoweredSoftDynamicQuery();

builder.Services
    .AddFluentValidation();

builder.Services.AddModule<ApiModule>();
builder.Services.AddModule<DalModule>();

builder.Services.AddModule<KeycloakModule>();

builder.Services.AddModule<LinodeModule>();
builder.Services.AddModule<NodeModule>();

builder.Services.AddModule<CommandModule>();
builder.Services.AddModule<QueryModule>();

mvcBuilder
    .AddPoweredSoftCommands();

mvcBuilder
    .AddPoweredSoftQueries();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();

app.UseCors(t =>
{
    t.AllowAnyOrigin()
        .AllowAnyMethod()
        .AllowAnyHeader();
});

GlobalConfiguration.Configuration.UseActivator(new HangfireActivator(app.Services));

app.MapControllers();
app.MapHangfireDashboard();

app.Run();