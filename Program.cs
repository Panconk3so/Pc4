using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Pc4.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.ML;
using Microsoft.OpenApi.Models;
using System.Threading.Tasks;
using Practica_WebApi1;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(connectionString));
builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>();
// Agrega servicios para el modelo ML.NET
builder.Services.AddPredictionEnginePool<practica.ModelInput, practica.ModelOutput>()
    .FromFile("practica.zip");

// Configura Swagger
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Description = "Docs for my API", Version = "v1" });
});


var app = builder.Build();

// Habilitar Swagger
app.UseSwagger();

app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
});

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

// Define ruta para predicciones
app.MapPost("/predict", async (PredictionEnginePool<practica.ModelInput, practica.ModelOutput> predictionEnginePool,practica.ModelInput input) =>
{
    // Realiza una predicción usando el modelo de ML.NET
    practica.ModelOutput prediction = predictionEnginePool.Predict(input);
    
    // Puedes manejar el resultado de la predicción aquí y devolverlo como corresponda.
    
    return Task.FromResult(prediction);
});

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();