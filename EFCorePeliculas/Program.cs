
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;

namespace EFCorePeliculas
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
                                                //ignorar ciclos repetitivos entre entidades.
            builder.Services.AddControllers().AddJsonOptions(opt =>opt.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
            builder.Services.AddDbContext<ApplicationDBContext>(opciones =>
                                                                            { 
                                                                                opciones.UseSqlServer(connectionString, sqlserver => sqlserver.UseNetTopologySuite());
                                                                                opciones.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
                                                                            }
                                                                            );


            builder.Services.AddAutoMapper(typeof(Program));

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json","EntityFrameworks API"));
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}