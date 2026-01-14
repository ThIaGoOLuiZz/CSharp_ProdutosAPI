using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProdutosAPI.Context;
using ProdutosAPI.Filters;
using ProdutosAPI.Models;
using ProdutosAPI.Repositories;

namespace ProdutosAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriasController : ControllerBase
    {
        private readonly IUnitOfWork _uof;
        private readonly ILogger _logger;
        public CategoriasController(ILogger<CategoriasController> logger,IUnitOfWork uof)
        {
            _logger = logger;
            _uof = uof;
        }

        [HttpGet]
        [ServiceFilter(typeof(ApiLoggingFilter))] // Applying the logging filter
        public ActionResult<IEnumerable<Categoria>> GetCategorias()
        {
            var categorias = _uof.CategoriaRepository.GetAll();
            return Ok(categorias);
        }

        [HttpGet("{id:int}", Name = "ObterCategoria")]
        public ActionResult<Categoria> GetCategoria(int id)
        {
            var categoria = _uof.CategoriaRepository.Get(c => c.CategoriaId == id);
            if(categoria is null)
            {
                return NotFound("Categoria não encontrada!");
            }
            return Ok(categoria);
        }

        [HttpPost]
        public ActionResult<Categoria> PostCategoria(Categoria categoria)
        {
            if(categoria is null)
            {
                return BadRequest("Categoria inválida");
            }

            var categoriaCriada = _uof.CategoriaRepository.Create(categoria);
            _uof.Commit();

            return new CreatedAtRouteResult("ObterCategoria", new { id = categoriaCriada.CategoriaId }, categoriaCriada);
        }

        [HttpPut("{id:int}")]
        public ActionResult<Categoria> PutCategoria(int id, Categoria categoria)
        {
            if(id != categoria.CategoriaId)
            {
                return BadRequest();
            }

            _uof.CategoriaRepository.Update(categoria);
            _uof.Commit();
            return Ok(categoria);
        }

        [HttpDelete("{id:int}")]
        public ActionResult<Categoria> DeleteCategoria(int id)
        {
            var categoria = _uof.CategoriaRepository.Get(c => c.CategoriaId == id);

            if (categoria is null)
            {
                return NotFound("Categoria não encontrada!");
            }

            var categoriaExcluida = _uof.CategoriaRepository.Delete(categoria);
            _uof.Commit();

            return Ok(categoria);
        }
    }
}
