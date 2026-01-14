using ProdutosAPI.Models;
using ProdutosAPI.Pagination;

namespace ProdutosAPI.Repositories
{
    public interface IProdutoRepository : IRepository<Produto>
    {
        IEnumerable<Produto> GetProdutos(ProdutosParamenters produtosParams);
        IEnumerable<Produto> GetProdutosPorCategoria(int id);
    }
}
