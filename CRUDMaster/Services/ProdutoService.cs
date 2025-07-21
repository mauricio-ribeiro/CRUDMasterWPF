using CRUDMaster.Models;
using CRUDMaster.Services.Interfaces;

namespace CRUDMaster.Services
{
    public class ProdutoService : IProdutoService
    {
        private readonly DataService _dataService;
        private List<Produto> _produtos;

        public ProdutoService(DataService dataService)
        {
            _dataService = dataService;
            _produtos = _dataService.Carregar<Produto>("produtos.json") ?? new List<Produto>();
        }

        public List<Produto> ObterTodos() => _produtos;

        public Produto ObterPorId(int id) => _produtos.FirstOrDefault(p => p.Id == id);

        public void Adicionar(Produto produto)
        {
            produto.Id = _produtos.Any() ? _produtos.Max(p => p.Id) + 1 : 1;
            _produtos.Add(produto);
            _dataService.Salvar("produtos.json", _produtos);
        }

        public void Atualizar(Produto produto)
        {
            var index = _produtos.FindIndex(p => p.Id == produto.Id);
            if (index >= 0)
            {
                _produtos[index] = produto;
                _dataService.Salvar("produtos.json", _produtos);
            }
        }

        public void Remover(int id)
        {
            var produto = ObterPorId(id);
            if (produto != null)
            {
                _produtos.Remove(produto);
                _dataService.Salvar("produtos.json", _produtos);
            }
        }
    }
}