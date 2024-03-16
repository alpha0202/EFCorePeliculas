using AutoMapper;
using AutoMapper.QueryableExtensions;
using EFCorePeliculas.DTOs;
using EFCorePeliculas.Entidades;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EFCorePeliculas.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PeliculasController : ControllerBase
    {
        private readonly ApplicationDBContext _context;
        private readonly IMapper _mapper;

        public PeliculasController(ApplicationDBContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<PeliculaDTO>> Get(int id)
        {
            var pelicula = await _context.Peliculas
                .Include(g => g.Generos.OrderByDescending(g => g.Nombre))    //como si realizara un join entre tablas  con include
                .Include(s => s.SalasDeCines)
                    .ThenInclude(s => s.Cine)
                .Include(p => p.PeliculasActores.Where(pa => pa.Actor.FechaNacimiento.Value.Year >= 1980))
                    .ThenInclude(pa => pa.Actor)
                .FirstOrDefaultAsync(p => p.Id == id);
            if (pelicula is null)
            {
                return NotFound();
            }

            var peliculaDTO = _mapper.Map<PeliculaDTO>(pelicula);
            peliculaDTO.Cines = peliculaDTO.Cines.DistinctBy(c => c.Id).ToList();

            return Ok(peliculaDTO);
        }


        [HttpGet("projectto/{id:int}")]
        public async Task<ActionResult<PeliculaDTO>> GetProjecTo(int id)
        {
            var pelicula = await _context.Peliculas
               .ProjectTo<PeliculaDTO>(_mapper.ConfigurationProvider)
               .FirstOrDefaultAsync(p => p.Id == id);
            if (pelicula is null)
            {
                return NotFound();
            }


            pelicula.Cines = pelicula.Cines.DistinctBy(c => c.Id).ToList();

            return Ok(pelicula);
        }


        [HttpGet("cargadoselectivo/{id:int}")]
        public async Task<ActionResult> GetSelectivo(int id)
        {
            var movie = await _context.Peliculas.Select(p =>
            new
            {
                id = p.Id,
                Titulo = p.Titulo,
                Generos = p.Generos.OrderByDescending(g => g.Nombre).Select(g => g.Nombre).ToList(),
                CantidadActores = p.PeliculasActores.Count(),
                CantidadCines = p.SalasDeCines.Select(s => s.CineId).Distinct().Count(),
            }).FirstOrDefaultAsync(p => p.id == id);

            if (movie is null)
            {
                return NotFound();
            }

            return Ok(movie);
        }

        [HttpGet("cargadoexplicito/{id:int}")]
        public async Task<ActionResult<PeliculaDTO>> GetExplicito(int id)
        {
            var movie = await _context.Peliculas.AsTracking().FirstOrDefaultAsync(p => p.Id == id);

            //await _context.Entry(movie).Collection(p => p.Generos).LoadAsync();
            var cantidadGeneros = _context.Entry(movie).Collection(p => p.Generos).Query().CountAsync();

            if (movie is null)
            {
                return NotFound();
            }

            var movieDTO = _mapper.Map<PeliculaDTO>(movie);

            return movieDTO;
        }


        //group by simple
        [HttpGet("agrupadasporestreno")]
        public async Task<ActionResult> GetAgruadasPorCartelera()
        {
            var peliculasAgrupadas = await _context.Peliculas.GroupBy(p => p.EnCartelera)
                                     .Select(g => new
                                     {
                                         EnCartelera = g.Key,
                                         Conteo = g.Count(),
                                         Peliculas = g.ToList()
                                     }).ToListAsync();
            return Ok(peliculasAgrupadas);
        }



        //group by count
        [HttpGet("agrupadasporcantidadgeneros")]

        public async Task<ActionResult> GetAgrupadasPorCantidadGeneros()
        {
            var pelicuasAgrupadasGenero = await _context.Peliculas.GroupBy(p => p.Generos.Count())
                                          .Select(g => new
                                          {
                                              Conteo = g.Key,
                                              Titulos = g.Select(x => x.Titulo),
                                              Generos = g.Select(y => y.Generos)
                                              .SelectMany(gen => gen)
                                              .Select(gen => gen.Nombre).Distinct()

                                          }).ToListAsync();
            return Ok(pelicuasAgrupadasGenero);
        }



        //filtrar (filter) - ejecución diferida

        [HttpGet("filtrar")]
        public async Task<ActionResult<List<PeliculaDTO>>> Filtrar([FromQuery] PeliculasFiltroDTO peliculasFiltroDTO)
        {
            var pelicuasQueryable = _context.Peliculas.AsQueryable();

            if(!string.IsNullOrEmpty(peliculasFiltroDTO.Titulo))
                pelicuasQueryable = pelicuasQueryable.Where(p => p.Titulo.Contains(peliculasFiltroDTO.Titulo));


            if (peliculasFiltroDTO.EnCartelera)
                pelicuasQueryable = pelicuasQueryable.Where(p => p.EnCartelera);

            if (peliculasFiltroDTO.ProximosEstrenos)
            {
                var hoy = DateTime.Today;
                pelicuasQueryable = pelicuasQueryable.Where(p => p.FechaEstreno > hoy);

            }

            if (peliculasFiltroDTO.GeneroId !=0)
            {
                pelicuasQueryable = pelicuasQueryable.Where(p => p.Generos.Select(g => g.Identificador).Contains(peliculasFiltroDTO.GeneroId));
            }


            var peliculas = await pelicuasQueryable.Include(g => g.Generos).ToListAsync();

            return _mapper.Map<List<PeliculaDTO>>(peliculas);
        }



    }
}
