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
    public class AutoresController : ControllerBase
    {
        private readonly ApplicationDBContext _context;
        private readonly IMapper _mapper;

        public AutoresController(ApplicationDBContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IEnumerable<ActorDTO>> Get()
        {
            //var actors = await _context.Actores
            //                    .Select(a => new ActorDTO {Id = a.Id, Nombre = a.Nombre}).ToListAsync();   //se utiliza la clase de transporte (DTO)


            var actors = await _context.Actores
                                .ProjectTo<ActorDTO>(_mapper.ConfigurationProvider).ToListAsync();   //se utiliza la clase de transporte (DTO)

            return actors;
        }


    }
}
