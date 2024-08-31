using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EFCorePeliculas.Entidades
{

    //[Table("TablaGeneros",Schema = "peliculas")]
    public class Genero
    {
        //[Key]
        public int Identificador { get; set; }
        //[StringLength(150)]
        public string Nombre { get; set; }

        public HashSet<Pelicula> Peliculas { get; set; }
        public bool EstaBorrado { get; set; }
    }
}
