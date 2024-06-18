using Hm.Scheduling.Core.Extensions;
using Hm.Scheduling.Core.Settings;
using Hm.Scheduling.Infrastructure.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers(options => options.SuppressAsyncSuffixInActionNames = false);
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var applicationSettings = builder
    .Configuration.GetSection(nameof(ApplicationSettings))
    .Get<ApplicationSettings>()!;

builder
    .Services.AddInfrastructure(applicationSettings)
    .AddCore();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
