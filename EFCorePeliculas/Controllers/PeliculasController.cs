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
                .Include(s=> s.SalasDeCines)
                    .ThenInclude(s => s.Cine)
                .Include(p => p.PeliculasActores.Where(pa=>pa.Actor.FechaNacimiento.Value.Year >= 1980))
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
            
            if(movie is null)
            { 
                return NotFound(); 
            }
               
            return Ok(movie);
        }

        [HttpGet("cargadoexplicito/{id:int}")]
        public async Task<ActionResult<PeliculaDTO>> GetExplicito(int id)
        {
            var movie = await _context.Peliculas.AsTracking().FirstOrDefaultAsync(p => p.Id == id);
            if(movie is null)
            {
                return NotFound();
            }
            return Ok(movie);
        }

    }
}
