using CoreService.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.ConfigureKestrel(options =>
{
  options.ListenAnyIP(5101);
});

builder.Services.AddCors(options =>
{
  options.AddPolicy("AllowFrontend", policy =>
  {
    policy.WithOrigins(
        "http://localhost:5173",  // react
        "http://localhost:3000",  // next
        "https://localhost:8443", // nginx
        "http://localhost:3001"   // next test
      )
      .AllowAnyHeader()
      .AllowAnyMethod();
  });
});

builder.Services.AddControllers();

builder.Services.AddDbContext<AuthDbContext>(options =>
  options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

app.UseCors("AllowFrontend");

if (app.Environment.IsDevelopment())
{
  app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
