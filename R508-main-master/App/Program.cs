using App.Mapping;
using App.Models;
using App.Models.DataManager;
using App.Models.EntityFramework;
using App.Models.Repository;

namespace App;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddCors(options =>
        {
            options.AddDefaultPolicy(policy =>
            {
                policy.WithOrigins("http://localhost:5063")
                      .AllowAnyHeader()
                      .AllowAnyMethod();
            });
        });

        builder.Services.AddAutoMapper(typeof(ProduitMapper));
        builder.Services.AddAutoMapper(typeof(MarqueMapper));
        builder.Services.AddAutoMapper(typeof(TypeProduitMapper));

        // Add services to the container.

        builder.Services.AddControllers();
        
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        builder.Services.AddDbContext<AppDbContext>();
        builder.Services.AddScoped<IDataRepository<Produit>, ProductManager>();
        builder.Services.AddScoped<IDataRepository<Marque>, MarqueManager>();
        builder.Services.AddScoped<IDataRepository<TypeProduit>, TypeProduitManager>();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        // Utiliser CORS
        app.UseCors();

        app.UseAuthorization();


        app.MapControllers();

        app.Run();
        //9.0.301
    }
}