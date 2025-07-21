using CRUDMaster.Models;
using CRUDMaster.Services.Interfaces;

namespace CRUDMaster.Services
{
    public class PessoaService : IPessoaService
    {
        private readonly DataService _dataService;
        private List<Pessoa> _pessoas;

        public PessoaService(DataService dataService)
        {
            _dataService = dataService;
            _pessoas = _dataService.Carregar<Pessoa>("pessoas.json") ?? new List<Pessoa>();
        }

        public List<Pessoa> ObterTodos() => _pessoas;

        public Pessoa ObterPorId(int id) => _pessoas.FirstOrDefault(p => p.Id == id);

        public void Adicionar(Pessoa pessoa)
        {
            pessoa.Id = _pessoas.Any() ? _pessoas.Max(p => p.Id) + 1 : 1;
            _pessoas.Add(pessoa);
            _dataService.Salvar("pessoas.json", _pessoas);
        }

        public void Atualizar(Pessoa pessoa)
        {
            var index = _pessoas.FindIndex(p => p.Id == pessoa.Id);
            if (index >= 0)
            {
                _pessoas[index] = pessoa;
                _dataService.Salvar("pessoas.json", _pessoas);
            }
        }

        public void Remover(int id)
        {
            var pessoa = ObterPorId(id);
            if (pessoa != null)
            {
                _pessoas.Remove(pessoa);
                _dataService.Salvar("pessoas.json", _pessoas);
            }
        }
    }
}