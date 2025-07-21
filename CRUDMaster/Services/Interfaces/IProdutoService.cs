using CRUDMaster.Models;

namespace CRUDMaster.Services.Interfaces
{
    public interface IProdutoService
    {
        List<Produto> ObterTodos();
        Produto ObterPorId(int id);
        void Adicionar(Produto produto);
        void Atualizar(Produto produto);
        void Remover(int id);
    }
}