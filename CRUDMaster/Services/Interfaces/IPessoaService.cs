using CRUDMaster.Models;

namespace CRUDMaster.Services.Interfaces
{
    public interface IPessoaService
    {
        List<Pessoa> ObterTodos();
        Pessoa ObterPorId(int id);
        void Adicionar(Pessoa pessoa);
        void Atualizar(Pessoa pessoa);
        void Remover(int id);
    }
}

