using CRUDMaster.Models;
using CRUDMaster.Services.Interfaces;

namespace CRUDMaster.Services
{
    public class PedidoService : IPedidoService
    {
        private readonly DataService _dataService;
        private List<Pedido> _pedidos;

        public PedidoService(DataService dataService)
        {
            _dataService = dataService;
            _pedidos = _dataService.Carregar<Pedido>("pedidos.json") ?? new List<Pedido>();
        }

        public List<Pedido> ObterTodos() => _pedidos;

        public List<Pedido> ObterPorPessoa(int pessoaId) =>
            _pedidos.Where(p => p.PessoaId == pessoaId).ToList();

        public Pedido ObterPorId(int id) => _pedidos.FirstOrDefault(p => p.Id == id);

        public void Adicionar(Pedido pedido)
        {
            pedido.Id = _pedidos.Any() ? _pedidos.Max(p => p.Id) + 1 : 1;
            _pedidos.Add(pedido);
            _dataService.Salvar("pedidos.json", _pedidos);
        }

        public void Atualizar(Pedido pedido)
        {
            var index = _pedidos.FindIndex(p => p.Id == pedido.Id);
            if (index >= 0)
            {
                _pedidos[index] = pedido;
                _dataService.Salvar("pedidos.json", _pedidos);
            }
        }

        public void Remover(int id)
        {
            var pedido = ObterPorId(id);
            if (pedido != null)
            {
                _pedidos.Remove(pedido);
                _dataService.Salvar("pedidos.json", _pedidos);
            }
        }
    }
}