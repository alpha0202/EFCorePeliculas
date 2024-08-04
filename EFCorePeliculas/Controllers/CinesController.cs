using AutoMapper;
using AutoMapper.QueryableExtensions;
using EFCorePeliculas.DTOs;
using EFCorePeliculas.Entidades;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite;
using NetTopologySuite.Geometries;

namespace EFCorePeliculas.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CinesController : ControllerBase
    {
        private readonly ApplicationDBContext _context;
        private readonly IMapper _mapper;

        public CinesController(ApplicationDBContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }


        [HttpGet]
        public async Task<IEnumerable<CineDTO>> Get()
        {
            return await _context.Cines.ProjectTo<CineDTO>(_mapper.ConfigurationProvider).ToListAsync();

        }

        [HttpGet("cercanos")]
        public async Task<ActionResult> Get(double latitud, double longitud)
        {
            var geometryFactory = NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326);
            var miUbicacion = geometryFactory.CreatePoint(new Coordinate(latitud, longitud));
            var distanciaMaximaMetros = 2000;

            var cines = await _context.Cines
                              .OrderBy(c => c.Ubicacion.Distance(miUbicacion))
                              .Where(c=> c.Ubicacion.IsWithinDistance(miUbicacion, distanciaMaximaMetros))
                              .Select(c => new
                              {
                                  Nombre = c.Nombre,
                                 Distancia = Math.Round(c.Ubicacion.Distance(miUbicacion))
                              }).ToListAsync();

            return Ok(cines);
        }


        [HttpPost] //Insertar cine con data relacionada
        public async Task<ActionResult> Post()
        {
            var geometryFactory = NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326);
            var ubicacionCine = geometryFactory.CreatePoint(new Coordinate(7.063569639451647, -73.087811395785));


            var cine = new Cine()
            {
                Nombre = "Cañaveral",
                Ubicacion = ubicacionCine,
                CineOferta = new CineOferta()
                {
                    PorcentajeDescuento = 5,
                    FechaInicio = DateTime.Today,
                    FechaFin = DateTime.Today.AddDays(7),


                },
                SalasDeCines = new HashSet<SalaDeCine> {
                    new SalaDeCine
                    {
                        Precio = 200,
                        TipoSalaDeCine = TipoSalaDeCine.DosDimensiones
                    },
                    new SalaDeCine
                    {
                        Precio = 350,
                        TipoSalaDeCine = TipoSalaDeCine.TresDimensiones
                    },


                }
            };


            _context.Add(cine); 
            await _context.SaveChangesAsync();
            return Ok();

         
        }


        [HttpPost("conDTO")] // Insertar cine con data relacionada utilizado automapper
        public async Task<ActionResult> Post(CineCreacionDTO cineCreacionDTO)
        {
            var cine = _mapper.Map<Cine>(cineCreacionDTO);
            _context.Add(cine);
            await  _context.SaveChangesAsync();
            return Ok();
        }



        //[HttpPut("{id:int}")]
        //public async Task<ActionResult> Put(CineCreacionDTO )


    }
}
