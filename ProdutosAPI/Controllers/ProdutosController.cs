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
        private readonly IProdutoRepository _repository;

        public ProdutosController(IProdutoRepository repository) //Construtor que recebe o contexto do banco de dados
        {
            _repository = repository;
        }

        [HttpGet("/primeiroProduto")] //Barra ignora o nome do controller na rota
        [HttpGet("teste")] //Rota alternativa para acessar o mesmo método
        [HttpGet("primeiroProduto")] //Rota alternativa para acessar o mesmo método
        [HttpGet("{valor:alpha:length(5):range(5,10)}")] //Rota que espera um valor alfabético como parâmetro. O parâmetro deve ter exatamente 5 caracteres. "range" define o tamanho minimo e maximo esperado
        public ActionResult<Produto> GetPrimeiroProdutos() //ActionResult permite retornar ou um status ou um objeto (Lista de produtos)
        {
            var produto = _context.Produtos.FirstOrDefault(); //Pega os produtos do contexto do banco e converte para lista
            if (produto is null)
            {
                return NotFound("Produto não encontrado!"); //Retorna 404 caso a lista esteja nula
            }

            return produto; //Retorna a lista de produtos com status 200 OK
        }


        [HttpGet]
        public ActionResult<IEnumerable<Produto>> GetProdutos() //ActionResult permite retornar ou um status ou um objeto (Lista de produtos)
        {
            var produtos = _repository.GetProdutos().ToList(); //Pega os produtos do contexto do banco e converte para lista
            if (produtos is null)
            {
                return NotFound("Produtos não encontrados!"); //Retorna 404 caso a lista esteja nula
            }
            return Ok(produtos); //Retorna a lista de produtos com status 200 OK
        }

        [HttpGet("{id:int:min(1)}/{param2=Default}", Name = "ObterProduto")] //Rota recebe o ID do produto como parâmetro e define um nome para a rota. O segundo parâmetro é opcional com valor padrão "Default". "min" é usado para definir o valor minimo esperado
        public ActionResult<Produto> GetProdutoAsync([FromQuery]int id, string param2,[BindRequired] string nome) //O atributo BindRequired torna o parâmetro obrigatório na rota. FromQuery indica que o valor do parâmetro virá da query string
        {
            var produto = _repository.GetProduto(id);
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

            var novoProduto = _repository.Create(produto); //Adiciona o produto ao contexto do banco
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

            bool atualizado = _repository.Update(produto); //Atualiza o produto no contexto do banco
            if (atualizado)
            {
                return Ok(produto);
            }

            return StatusCode(500, "Falha ao atualizar o produto"); //Retorna 500 caso ocorra algum erro ao atualizar o produto

        }

        [HttpDelete("{id:int}")] //Rota recebe o ID do produto como parâmetro
        public ActionResult DeleteProduto(int id)
        {
            bool deletado = _repository.Delete(id); //Remove o produto do contexto do banco
            if (deletado)
            {
                return Ok("Produto excluido");
            }
            return StatusCode(500, "Falha ao deletar o produto"); //Retorna 500 caso ocorra algum erro ao deletar o produto
        }
    }
}
