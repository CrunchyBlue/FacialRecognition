using Microsoft.EntityFrameworkCore;
using OrdersApi.DbContexts;
using OrdersApi.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddDbContext<OrdersContext>(options => options.UseSqlServer(
    builder.Configuration.GetConnectionString("OrdersConnection")));
builder.Services.AddScoped<IOrdersRepository, OrdersRepository>();
builder.Services.AddControllers().AddDapr();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCloudEvents();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.MapSubscribeHandler();

app.Run();