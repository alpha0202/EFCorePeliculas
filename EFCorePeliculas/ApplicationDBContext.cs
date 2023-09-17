using EFCorePeliculas.Entidades;
using Microsoft.EntityFrameworkCore;

namespace EFCorePeliculas
{
    public class ApplicationDBContext : DbContext
    {
        public ApplicationDBContext(DbContextOptions options) : base(options)
        {

           
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Genero>().HasKey(prop => prop.Identificador);
            modelBuilder.Entity<Genero>().Property(prop => prop.Nombre)
                .HasMaxLength(150)
                .IsRequired();

            modelBuilder.Entity<Actor>().Property(prop => prop.Nombre)
                 .HasMaxLength(150)
                 .IsRequired()
                 ;
            modelBuilder.Entity<Actor>().Property(prop => prop.FechaNacimiento)
               .HasColumnType("date");


            modelBuilder.Entity<Cine>().Property(prop => prop.Nombre)
                .HasMaxLength(150)
                .IsRequired();
            modelBuilder.Entity<Cine>().Property(prop => prop.Precio)
                .HasPrecision(precision: 9, scale: 2);


            modelBuilder.Entity<Pelicula>().Property(prop => prop.Titulo)
                .HasMaxLength(250)
                .IsRequired();

            modelBuilder.Entity<Pelicula>().Property(prop => prop.FechaEstreno)
                .HasColumnType("date");

            modelBuilder.Entity<Pelicula>().Property(prop => prop.PosterURL)
                .HasMaxLength(500)
                .IsUnicode(false);

        }


        public DbSet<Genero> Generos{ get; set; }
        public DbSet<Actor> Actores { get; set; }
        public DbSet<Cine> Cines { get; set; }
        public DbSet<Pelicula> Peliculas { get; set; }

    }
}
