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

    }
}
