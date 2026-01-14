using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using ProdutosAPI.Context;
using ProdutosAPI.Models;
using ProdutosAPI.Repositories;

namespace ProdutosAPI.Controllers
{
    [Route("api/[controller]")] //Define a rota base do controlador como o nome do controlador (Produtos)
    [ApiController] //Indica que este é um controlador de API
    public class ProdutosController : ControllerBase
    {
        private readonly IUnitOfWork _uof;

        public ProdutosController(IUnitOfWork unitOfWork)
        {
            _uof = unitOfWork;
        }

        [HttpGet("produtos/{id}")]
        public ActionResult<IEnumerable<Produto>> GetProdutosCategoria(int id)
        {
            var produtos = _uof.ProdutoRepository.GetProdutosPorCategoria(id);
            if (produtos is null)
            {
                return NotFound("Produtos não encontrados para a categoria informada!");
            }
            return Ok(produtos);
        }


        [HttpGet]
        public ActionResult<IEnumerable<Produto>> GetProdutos() //ActionResult permite retornar ou um status ou um objeto (Lista de produtos)
        {
            var produtos = _uof.ProdutoRepository.GetAll(); //Pega os produtos do contexto do banco e converte para lista
            if (produtos is null)
            {
                return NotFound("Produtos não encontrados!"); //Retorna 404 caso a lista esteja nula
            }
            return Ok(produtos); //Retorna a lista de produtos com status 200 OK
        }

        [HttpGet("{id:int:min(1)}/{param2=Default}", Name = "ObterProduto")] //Rota recebe o ID do produto como parâmetro e define um nome para a rota. O segundo parâmetro é opcional com valor padrão "Default". "min" é usado para definir o valor minimo esperado
        public ActionResult<Produto> GetProdutoAsync([FromQuery]int id, string param2,[BindRequired] string nome) //O atributo BindRequired torna o parâmetro obrigatório na rota. FromQuery indica que o valor do parâmetro virá da query string
        {
            var produto = _uof.ProdutoRepository.Get(c => c.ProdutoId == id);
            if (produto is null)
            {
                return NotFound("Produto não encontrado!"); //Retorna 404 caso o produto não seja encontrado
            }
            return Ok(produto); //Retorna o produto com status 200 OK
        }

        [HttpPost]
        public ActionResult PostProduto(Produto produto)
        {
            if(produto is null)
            {
                return BadRequest(); //Retorna 400 caso o produto seja nulo
            }

            var novoProduto = _uof.ProdutoRepository.Create(produto); //Adiciona o produto ao contexto do banco
            _uof.Commit(); //Salva as alterações no banco de dados
            return new CreatedAtRouteResult("ObterProduto", //Retorna 201 Created com a rota para obter o produto criado
                new { id = novoProduto.ProdutoId }, novoProduto);
        }

        [HttpPut("{id:int}")] //Rota recebe o ID do produto como parâmetro
        public ActionResult PutProduto(int id, Produto produto)
        {
            if(id != produto.ProdutoId) //Verifica se o ID da rota é igual ao ID do produto
            {
                return BadRequest();
            }

            var produtoAtualizado = _uof.ProdutoRepository.Update(produto); //Atualiza o produto no contexto do banco
            _uof.Commit(); //Salva as alterações no banco de dados
            return Ok(produtoAtualizado); //Retorna o produto atualizado com status 200 OK
        }

        [HttpDelete("{id:int}")] //Rota recebe o ID do produto como parâmetro
        public ActionResult DeleteProduto(int id)
        {
            var produto = _uof.ProdutoRepository.Get(p => p.ProdutoId == id);
            _uof.Commit(); //Salva as alterações no banco de dados
            if (produto is null)
            {
                return NotFound("Produto não encontrado!"); //Retorna 404 caso o produto não seja encontrado
            }

            _uof.ProdutoRepository.Delete(produto); //Remove o produto do contexto do banco
            _uof.Commit(); //Salva as alterações no banco de dados
            return Ok();
        }
    }
}
