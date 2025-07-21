using CRUDMaster.Models;

namespace CRUDMaster.Services.Interfaces
{
    public interface IPedidoService
    {
        List<Pedido> ObterTodos();
        List<Pedido> ObterPorPessoa(int pessoaId);
        Pedido ObterPorId(int id);
        void Adicionar(Pedido pedido);
        void Atualizar(Pedido pedido);
        void Remover(int id);
    }
}