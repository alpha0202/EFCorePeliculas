using AutoMapper;
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
                .Include(p => p.PeliculasActores)
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


    }
}
