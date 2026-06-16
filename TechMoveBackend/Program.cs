using Microsoft.EntityFrameworkCore;
using TechMove.Data;
using TechMove.Interfaces;
using TechMove.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen();

//database configuration
builder.Services.AddDbContext<TechMoveDbContext>(options =>
options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

//file upload
builder.Services.AddScoped<FileUploadValidationService>();

//service request validations
builder.Services.AddScoped<IServiceRequestService, ServiceRequestService>();

//currency conversion
builder.Services.AddHttpClient<ICurrencyService, CurrencyApiAdapterService>();

//observer status checker
builder.Services.AddScoped<IContractObserver, ContractStatusObserverService>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
