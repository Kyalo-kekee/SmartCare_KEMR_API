using DataAccessLayer.Data;
using DataAccessLayer.MessageTypes;
using Hangfire;
using Hangfire.SqlServer;
using SmartCare;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<IAbstractPatientMessage, AbstractPatientMessage>();
builder.Services.AddSingleton<IPatientInformation, PatientInformation>();
builder.Services.AddSingleton<IPharmarcyOrders, PharmarcyOrders>();
builder.Services.AddSingleton<ICustomerCreateFromCcc, CustomerCreateFromCcc>();
builder.Services.AddSingleton<IProcessMessage, ProcessMessage>();
builder.Services.AddHangfire(x => x.SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
                .UseColouredConsoleLogProvider()
                .UseSimpleAssemblyNameTypeSerializer()
                .UseRecommendedSerializerSettings()
                .UseSqlServerStorage(builder.Configuration.GetConnectionString("Default"), new SqlServerStorageOptions
                {
                    CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
                    SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
                    QueuePollInterval = TimeSpan.Zero,
                    UseRecommendedIsolationLevel = true
                }));
builder.Services.AddHangfireServer();
IServiceCollection serviceCollection = builder.Services.Configure<AppSettingsMessageTypes>(builder.Configuration.GetSection("MessageType"));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwaggerUI();
}
app.UseSwagger();

app.ConfigureApi();
app.UseHangfireDashboard();
app.UseHttpsRedirection();


app.Run();
//call api.