using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using ProdutosAPI.Context;
using ProdutosAPI.DTOs;
using ProdutosAPI.Models;
using ProdutosAPI.Repositories;

namespace ProdutosAPI.Controllers
{
    [Route("api/[controller]")] //Define a rota base do controlador como o nome do controlador (Produtos)
    [ApiController] //Indica que este é um controlador de API
    public class ProdutosController : ControllerBase
    {
        private readonly IUnitOfWork _uof;
        private readonly IMapper _mapper;

        public ProdutosController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _uof = unitOfWork;
            _mapper = mapper;
        }

        [HttpGet("produtos/{id}")]
        public ActionResult<IEnumerable<ProdutoDTO>> GetProdutosCategoria(int id)
        {
            var produtos = _uof.ProdutoRepository.GetProdutosPorCategoria(id);
            if (produtos is null)
            {
                return NotFound("Produtos não encontrados para a categoria informada!");
            }

            var produtosDto = _mapper.Map<IEnumerable<ProdutoDTO>>(produtos);
            return Ok(produtosDto);
        }


        [HttpGet]
        public ActionResult<IEnumerable<ProdutoDTO>> GetProdutos() //ActionResult permite retornar ou um status ou um objeto (Lista de produtos)
        {
            var produtos = _uof.ProdutoRepository.GetAll(); //Pega os produtos do contexto do banco e converte para lista
            if (produtos is null)
            {
                return NotFound("Produtos não encontrados!"); //Retorna 404 caso a lista esteja nula
            }
            var produtosDto = _mapper.Map<IEnumerable<ProdutoDTO>>(produtos);
            return Ok(produtosDto);
        }

        [HttpGet("{id:int:min(1)}/{param2=Default}", Name = "ObterProduto")] //Rota recebe o ID do produto como parâmetro e define um nome para a rota. O segundo parâmetro é opcional com valor padrão "Default". "min" é usado para definir o valor minimo esperado
        public ActionResult<ProdutoDTO> GetProdutoAsync([FromQuery]int id, string param2,[BindRequired] string nome) //O atributo BindRequired torna o parâmetro obrigatório na rota. FromQuery indica que o valor do parâmetro virá da query string
        {
            var produto = _uof.ProdutoRepository.Get(c => c.ProdutoId == id);
            if (produto is null)
            {
                return NotFound("Produto não encontrado!"); //Retorna 404 caso o produto não seja encontrado
            }

            var produtoDto = _mapper.Map<ProdutoDTO>(produto);
            return Ok(produto); //Retorna o produto com status 200 OK
        }

        [HttpPost]
        public ActionResult<ProdutoDTO> PostProduto(ProdutoDTO produtoDto)
        {
            if(produtoDto is null)
            {
                return BadRequest(); //Retorna 400 caso o produto seja nulo
            }

            var produto = _mapper.Map<Produto>(produtoDto);

            var novoProduto = _uof.ProdutoRepository.Create(produto); //Adiciona o produto ao contexto do banco
            _uof.Commit(); //Salva as alterações no banco de dados

            var novoProdutoDto = _mapper.Map<ProdutoDTO>(novoProduto);

            return new CreatedAtRouteResult("ObterProduto", //Retorna 201 Created com a rota para obter o produto criado
                new { id = novoProdutoDto.ProdutoId }, novoProdutoDto);
        }

        [HttpPatch("{id}/UpdatePartial")]
        public ActionResult<ProdutoDTOUpdateResponse> Patch(int id, JsonPatchDocument<ProdutoDTOUpdateRequest> patchProdutoDTO)
        {
            if (patchProdutoDTO is null || id <= 0)
            {
                return BadRequest();
            }

            var produto = _uof.ProdutoRepository.Get(p => p.ProdutoId == id);
            if (produto is null)
            {
                return NotFound("Produto não encontrado!");
            }

            var produtoUpdateRequest = _mapper.Map<ProdutoDTOUpdateRequest>(produto);

            patchProdutoDTO.ApplyTo(produtoUpdateRequest, ModelState);

            if (!ModelState.IsValid || !TryValidateModel(produtoUpdateRequest))
            {
                return BadRequest(ModelState);
            }

            _mapper.Map(produtoUpdateRequest, produto);

            _uof.ProdutoRepository.Update(produto);
            _uof.Commit();

            return Ok(_mapper.Map<ProdutoDTOUpdateResponse>(produto));
        }

        [HttpPut("{id:int}")] //Rota recebe o ID do produto como parâmetro
        public ActionResult<ProdutoDTO> PutProduto(int id, ProdutoDTO produtoDto)
        {
            if(id != produtoDto.ProdutoId) //Verifica se o ID da rota é igual ao ID do produto
            {
                return BadRequest();
            }

            var produto = _mapper.Map<Produto>(produtoDto);

            var produtoAtualizado = _uof.ProdutoRepository.Update(produto); //Atualiza o produto no contexto do banco
            _uof.Commit(); //Salva as alterações no banco de dados

            var produtoAtualizadoDto = _mapper.Map<ProdutoDTO>(produtoAtualizado);

            return Ok(produtoAtualizado); //Retorna o produto atualizado com status 200 OK
        }

        [HttpDelete("{id:int}")] //Rota recebe o ID do produto como parâmetro
        public ActionResult<ProdutoDTO> DeleteProduto(int id)
        {
            var produto = _uof.ProdutoRepository.Get(p => p.ProdutoId == id);
            _uof.Commit(); //Salva as alterações no banco de dados
            if (produto is null)
            {
                return NotFound("Produto não encontrado!"); //Retorna 404 caso o produto não seja encontrado
            }

            var produtoDeletado = _uof.ProdutoRepository.Delete(produto); //Remove o produto do contexto do banco
            _uof.Commit(); //Salva as alterações no banco de dados

            var produtoDeletadoDto = _mapper.Map<ProdutoDTO>(produtoDeletado);

            return Ok(produtoDeletadoDto);
        }
    }
}
