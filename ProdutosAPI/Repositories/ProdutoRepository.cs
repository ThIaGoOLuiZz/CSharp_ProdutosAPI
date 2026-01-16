using ProdutosAPI.Context;
using ProdutosAPI.Models;
using ProdutosAPI.Pagination;

namespace ProdutosAPI.Repositories
{
    public class ProdutoRepository : Repository<Produto>, IProdutoRepository
    {
        public ProdutoRepository(AppDbContext context) : base(context)
        {
        }

        //public IEnumerable<Produto> GetProdutos(ProdutosParamenters produtosParams)
        //{
        //    return GetAll().OrderBy(p => p.Nome)
        //                   .Skip((produtosParams.PageNumber - 1) * produtosParams.PageSize)
        //                   .Take(produtosParams.PageSize).ToList();
        //}

        public PagedList<Produto> GetProdutos(ProdutosParamenters produtosParams)
        {
            var produtos = GetAll().OrderBy(p => p.ProdutoId).AsQueryable();
            var produtosOrdenados = PagedList<Produto>.ToPagedList(produtos, produtosParams.PageNumber, produtosParams.PageSize);
            return produtosOrdenados;
        }

        public IEnumerable<Produto> GetProdutosPorCategoria(int id)
        {
            return GetAll().Where(c => c.CategoriaId == id);
        }
    }
}
