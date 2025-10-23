using Microsoft.EntityFrameworkCore;
using BSC.Data;
using BSC.Business.Services;
using BSC.Business.Interfaces;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

// Conexion a SQL Server
builder.Services.AddDbContext<AppDbContext>(opt => opt.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Registrar servicios de la capa de negocio
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IOrderService, OrderService>();

// Agregar controladores
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();

var app = builder.Build();

// Middleware
if(app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI(c =>
	{
		c.SwaggerEndpoint("/swagger/v1/swagger.json", "BSC API V1");
		c.RoutePrefix = string.Empty;
	});
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();


