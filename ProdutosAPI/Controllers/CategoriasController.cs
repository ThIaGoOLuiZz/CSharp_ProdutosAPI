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
        private readonly IRepository<Categoria> _repository;
        private readonly ILogger _logger;
        public CategoriasController(ICategoriaRepository repository, ILogger<CategoriasController> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        [HttpGet]
        [ServiceFilter(typeof(ApiLoggingFilter))] // Applying the logging filter
        public ActionResult<IEnumerable<Categoria>> GetCategorias()
        {
            var categorias = _repository.GetAll();
            return Ok(categorias);
        }

        [HttpGet("{id:int}", Name = "ObterCategoria")]
        public ActionResult<Categoria> GetCategoria(int id)
        {
            var categoria = _repository.Get(c => c.CategoriaId == id);
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

            var categoriaCriada = _repository.Create(categoria);

            return new CreatedAtRouteResult("ObterCategoria", new { id = categoriaCriada.CategoriaId }, categoriaCriada);
        }

        [HttpPut("{id:int}")]
        public ActionResult<Categoria> PutCategoria(int id, Categoria categoria)
        {
            if(id != categoria.CategoriaId)
            {
                return BadRequest();
            }

            _repository.Update(categoria);
            return Ok(categoria);
        }

        [HttpDelete("{id:int}")]
        public ActionResult<Categoria> DeleteCategoria(int id)
        {
            var categoria = _repository.Get(c => c.CategoriaId == id);

            if (categoria is null)
            {
                return NotFound("Categoria não encontrada!");
            }

            var categoriaExcluida = _repository.Delete(categoria);

            return Ok(categoria);
        }
    }
}
