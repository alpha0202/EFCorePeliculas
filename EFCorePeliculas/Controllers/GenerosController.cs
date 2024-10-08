﻿using EFCorePeliculas.Entidades;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EFCorePeliculas.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GenerosController : ControllerBase
    {
        private readonly ApplicationDBContext _dbContext;

        public GenerosController(ApplicationDBContext dBContext)
        {
            _dbContext = dBContext;
        }


        [HttpGet]
        public async Task<IEnumerable<Genero>> Get()
        {
            //ejemplo uso de log manual
            _dbContext.Logs.Add(new Log { Mensaje = "ejecutando el método GenerosControllores.Get" });
            await _dbContext.SaveChangesAsync();

            return await _dbContext.Generos.OrderBy(g => g.Nombre).ToListAsync();
        }


        [HttpGet("{id:int}")]
        public async Task<ActionResult<Genero>> Get(int id)
        {
            var genero = _dbContext.Generos.FirstOrDefaultAsync(g => g.Identificador == id);

            if (genero == null)
            {
                return NotFound();
            }
            return await genero;
        }



        [HttpGet("primero")]
        public async Task<ActionResult<Genero>> Primer()
        {
            var genero = await _dbContext.Generos.FirstOrDefaultAsync(g => g.Nombre.StartsWith("C"));

            if (genero is null)
            {
                return NotFound();
            }
            return genero;
        }

        [HttpGet("filtrar")]
        public async Task<IEnumerable<Genero>> Filtrar(string nombre)
        {
            return await _dbContext.Generos
                            .Where(g => g.Nombre.Contains(nombre))
                            .ToListAsync();                        //g.Nombre.StartsWith("C")|| g.Nombre.StartsWith("A")).ToListAsync();
                                                                   //.OrderBy(g=>g.Nombre)
        }

        [HttpGet("paginacion")]
        public async Task<ActionResult<IEnumerable<Genero>>> GetPaginacion(int pagina = 1)
        {
            var cantidadRegistrosPorPagina = 2;
            var generos = await _dbContext.Generos
                                .Skip((pagina - 1) * cantidadRegistrosPorPagina)
                                .Take(cantidadRegistrosPorPagina)
                                .ToListAsync();

            return generos;
        }

        //[HttpPost]
        //public async Task<ActionResult> Post(Genero genero)
        //{
        //    var status1 = _dbContext.Entry(genero).State;
        //    _dbContext.Add(genero);
        //    var status2 = _dbContext.Entry(genero).State;
        //    await _dbContext.SaveChangesAsync();
        //    var status3 = _dbContext.Entry(genero).State;

        //    return Ok();
        //}

        [HttpPost]
        public async Task<ActionResult> Post(Genero genero)
        {
           
            var generoExiste = await _dbContext.Generos.AnyAsync(g => g.Nombre == genero.Nombre);

            if (generoExiste)
                return BadRequest($"Ya existe un género con el nombre: {genero.Nombre}");

            _dbContext.Generos.Add(genero);
            await _dbContext.SaveChangesAsync();
            

            return Ok();
        }


        [HttpPost("varios")]
        public async Task<ActionResult> Post(Genero[] generos)
        {
            _dbContext.AddRange(generos);
            await _dbContext.SaveChangesAsync();
            return Ok();    
        }


        [HttpDelete("{id:int}")] //borrado normal
        public async Task<ActionResult> Delete(int id)
        {
            var genero = await _dbContext.Generos.FirstOrDefaultAsync(g => g.Identificador == id);

            if (genero is null)
                return NotFound();

            _dbContext.Remove(genero);
            await _dbContext.SaveChangesAsync();
            return Ok();
        }


        [HttpDelete("borradologico/{id:int}")] //borrado suave o lógico. Marcar como borrado
        public async Task<ActionResult> DeleteSoft(int id)
        {
            var genero = await _dbContext.Generos.AsTracking().FirstOrDefaultAsync(g => g.Identificador == id);


            if (genero is null)
                return NotFound();

           genero.EstaBorrado = true;
            await _dbContext.SaveChangesAsync();
            return Ok();
        }


        [HttpPost("restaurar/{id:int}")] // saltarse el QueryFilter del modelo, cuando se quiere mostrar toda la data 
        public async Task<ActionResult> Restaurar(int id)
        {
            var genero = await _dbContext.Generos.AsTracking()
                                                  .IgnoreQueryFilters()
                                                  .FirstOrDefaultAsync(g => g.Identificador == id);


            if (genero is null)
                return NotFound();

            genero.EstaBorrado = false;
            await _dbContext.SaveChangesAsync();
            return Ok();
        }
    }
}
