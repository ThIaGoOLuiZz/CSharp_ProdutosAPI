using Microsoft.EntityFrameworkCore;
using ProdutosAPI.Context;
using ProdutosAPI.Models;

namespace ProdutosAPI.Repositories
{
    public class CategoriaRepository : Repository<Categoria>, ICategoriaRepository
    {
        public CategoriaRepository(AppDbContext context) : base(context)
        {
        }
    }
}
