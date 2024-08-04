using EFCorePeliculas.Entidades;
using EFCorePeliculas.Entidades.Seeding;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using System.Reflection;

namespace EFCorePeliculas
{
    public class ApplicationDBContext : DbContext
    {
        public ApplicationDBContext(DbContextOptions options) : base(options)
        {
            ////en caso que no exista la db, esta se construirá 
            //try
            //{
            //    var dbCreator = Database.GetService<IDatabaseCreator>() as RelationalDatabaseCreator;
            //    if (dbCreator != null)
            //    {
            //        if (!dbCreator.CanConnect())
            //            dbCreator.Create();
            //        if (!dbCreator.HasTables())
            //            dbCreator.CreateTables();

            //    }
            //}
            //catch (Exception e)
            //{

            //    Console.WriteLine(e.Message);
            //}
           
        }

        protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
        {
            configurationBuilder.Properties<DateTime>().HaveColumnType("date");
        }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            

            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

            SeedingModuloConsulta.Seed(modelBuilder);

         

        }


        public DbSet<Genero> Generos{ get; set; }
        public DbSet<Actor> Actores { get; set; }
        public DbSet<Cine> Cines { get; set; }
        public DbSet<Pelicula> Peliculas { get; set; }
        public DbSet<CineOferta> CinesOfertas { get; set; }
        public DbSet<SalaDeCine> SalasDeCines { get; set; }
        public DbSet<PeliculaActor> PeliculasActores { get; set; }

    }
}
