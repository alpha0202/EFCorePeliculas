using EFCorePeliculas.DTOs;
using EFCorePeliculas.Entidades;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EFCorePeliculas.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AutoresController : ControllerBase
    {
        private readonly ApplicationDBContext _context;

        public AutoresController(ApplicationDBContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IEnumerable<ActorDTO>> Get()
        {
            var actors = await _context.Actores
                                .Select(a => new ActorDTO {Id = a.Id, Nombre = a.Nombre}).ToListAsync();   //se utiliza la clase de transporte (DTO)

            return actors;
        }


    }
}
