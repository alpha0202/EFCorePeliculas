using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Reflection.Emit;

namespace EFCorePeliculas.Entidades.Configuraciones
{
    public class GeneroConfig : IEntityTypeConfiguration<Genero>
    {
        public void Configure(EntityTypeBuilder<Genero> builder)
        {
            builder.HasKey(prop => prop.Identificador);
            builder.Property(prop => prop.Nombre)
                .HasMaxLength(150)
                .IsRequired();
            
            builder.HasQueryFilter(g => !g.EstaBorrado); //filtro a nivel de modelo 
        }
    }
}
