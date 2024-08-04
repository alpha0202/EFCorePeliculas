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
    public class ActoresController : ControllerBase
    {
        private readonly ApplicationDBContext _context;
        private readonly IMapper _mapper;

        public ActoresController(ApplicationDBContext context, IMapper mapper)
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
                                .ProjectTo<ActorDTO>(_mapper.ConfigurationProvider).ToListAsync();   //se utiliza la clase de transporte (DTO) con automapper

            return actors;
        }


        [HttpPost]               //mapeo flexible
        public async Task<ActionResult> Post(ActorCreacionDTO actorCreacionDTO)
        {
            var actor = _mapper.Map<Actor>(actorCreacionDTO);
            _context.Add(actor);
            await _context.SaveChangesAsync();

            return Ok();
        }

        [HttpPut("{id:int}")]      //actualización con modelo conectado
        public async Task<ActionResult> Put(ActorCreacionDTO actorCreacionDTO, int id)
        {
            var actorDB = await _context.Actores.AsTracking().SingleOrDefaultAsync(a => a.Id == id);

            if (actorDB is null)
            {
                return NotFound();
            }
            actorDB = _mapper.Map(actorCreacionDTO, actorDB);
            await _context.SaveChangesAsync();
            return Ok();

        }

        [HttpPut("desconectado/{id:int}")]  //actualización con modelo desconectado
        public async Task<ActionResult> PutDesconectado(ActorCreacionDTO actorCreacionDTO, int id)
        {
            var existeActor = await _context.Actores.AnyAsync(a => a.Id == id);

            if(!existeActor)
                return NotFound();

            var actor = _mapper.Map<Actor>(actorCreacionDTO);
            actor.Id = id;

            _context.Update(actor);
            await _context.SaveChangesAsync();
            return Ok();
        }
        

    }
}
