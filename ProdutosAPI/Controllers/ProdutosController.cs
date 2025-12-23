using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProdutosAPI.Context;
using ProdutosAPI.Models;

namespace ProdutosAPI.Controllers
{
    [Route("[controller]")] //Define a rota base do controlador como o nome do controlador (Produtos)
    [ApiController] //Indica que este é um controlador de API
    public class ProdutosController : ControllerBase
    {
        private readonly AppDbContext _context; //Injeção de dependência do contexto do banco de dados

        public ProdutosController(AppDbContext context) //Construtor que recebe o contexto do banco de dados
        {
            _context = context;
        }

        [HttpGet]
        public ActionResult<IEnumerable<Produto>> GetProdutos() //ActionResult permite retornar ou um status ou um objeto (Lista de produtos)
        {
            var produtos = _context.Produtos.ToList(); //Pega os produtos do contexto do banco e converte para lista
            if (produtos is null)
            {
                return NotFound("Produtos não encontrados!"); //Retorna 404 caso a lista esteja nula
            }

            return produtos; //Retorna a lista de produtos com status 200 OK
        }

        [HttpGet("{id:int}", Name = "ObterProduto")] //Rota recebe o ID do produto como parâmetro e define um nome para a rota
        public ActionResult<Produto> GetProduto(int id)
        {
            var produto = _context.Produtos.FirstOrDefault(p => p.ProdutoId == id); //Busca o produto pelo ID no contexto do banco
            if (produto is null)
            {
                return NotFound("Produto não encontrado!"); //Retorna 404 caso o produto não seja encontrado
            }

            return produto; //Retorna o produto encontrado com status 200 OK
        }

        [HttpPost]
        public ActionResult PostProduto(Produto produto)
        {
            if(produto is null) //Verifica se o produto é nulo
            {
                return BadRequest("Produto inválido!"); //Retorna 400 caso o produto seja nulo
            }
            _context.Produtos.Add(produto); //Adiciona o produto ao contexto do banco
            _context.SaveChanges(); //Salva as mudanças no banco de dados

            return new CreatedAtRouteResult("ObterProduto", new { id = produto.ProdutoId }, produto); // retorna sucesso, e chama a rota get ObterProduto para retornar o produto criado
        }

        [HttpPut("{id:int}")] //Rota recebe o ID do produto como parâmetro
        public ActionResult PutProduto(int id, Produto produto)
        {
            if(id != produto.ProdutoId) //Verifica se o ID da rota é igual ao ID do produto
            {
                return BadRequest();
            }

            _context.Entry(produto).State = EntityState.Modified; //Marca o produto como modificado no contexto do banco
            _context.SaveChanges(); //Salva as mudanças no banco de dados

            return Ok(produto); //Retorna o produto atualizado com status 200 OK
        }

        [HttpDelete("{id:int}")] //Rota recebe o ID do produto como parâmetro
        public ActionResult DeleteProduto(int id)
        {
            var produto = _context.Produtos.FirstOrDefault(p => p.ProdutoId == id); //Busca o produto pelo ID no contexto do banco

            if (produto is null)
            {
                return NotFound("Produto não encontrado!"); //Retorna 404 caso o produto não seja encontrado
            }

            _context.Produtos.Remove(produto); //Remove o produto do contexto do banco
            _context.SaveChanges(); //Salva as mudanças no banco de dados

            return Ok(produto); //Retorna o produto removido com status 200 OK
        }
    }
}
