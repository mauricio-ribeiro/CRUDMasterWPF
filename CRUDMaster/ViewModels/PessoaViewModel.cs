using CRUDMaster.Commands;
using CRUDMaster.Models;
using CRUDMaster.Services.Interfaces;
using CRUDMaster.Views;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;

namespace CRUDMaster.ViewModels
{
    public class PessoaViewModel : ViewModelBase
    {
        private readonly IPessoaService _pessoaService;
        private readonly IPedidoService _pedidoService;

        // Campos privados
        private List<Pessoa> _pessoas;
        private List<Pessoa> _pessoasFiltradas;
        private ObservableCollection<Pedido> _pedidosDaPessoa;
        private Pedido _pedidoSelecionado;
        private Pessoa _pessoaSelecionada;
        private Pessoa _pessoaEmEdicao;
        private string _filtroNome;
        private string _filtroCPF;
        private bool _emModoEdicao;

        // Propriedades públicas
        public List<Pessoa> Pessoas
        {
            get => _pessoas;
            private set
            {
                SetField(ref _pessoas, value);
                FiltrarPessoas();
            }
        }

        public List<Pessoa> PessoasFiltradas
        {
            get => _pessoasFiltradas;
            private set => SetField(ref _pessoasFiltradas, value);
        }

        public Pessoa PessoaSelecionada
        {
            get => _pessoaSelecionada;
            set
            {
                if (SetField(ref _pessoaSelecionada, value))
                {
                    CarregarPedidosDaPessoaSelecionada();
                }
            }
        }

        public ObservableCollection<Pedido> PedidosDaPessoa
        {
            get => _pedidosDaPessoa;
            private set => SetField(ref _pedidosDaPessoa, value);
        }

        public Pedido PedidoSelecionado
        {
            get => _pedidoSelecionado;
            set => SetField(ref _pedidoSelecionado, value);
        }

        public Pessoa PessoaEmEdicao
        {
            get => _pessoaEmEdicao;
            set => SetField(ref _pessoaEmEdicao, value);
        }

        public string FiltroNome
        {
            get => _filtroNome;
            set
            {
                if (SetField(ref _filtroNome, value))
                {
                    FiltrarPessoas();
                }
            }
        }

        public string FiltroCPF
        {
            get => _filtroCPF;
            set
            {
                if (SetField(ref _filtroCPF, value))
                {
                    FiltrarPessoas();
                }
            }
        }

        public bool EmModoEdicao
        {
            get => _emModoEdicao;
            set => SetField(ref _emModoEdicao, value);
        }

        public decimal TotalPedidos => PedidosDaPessoa?.Sum(p => p.ValorTotal) ?? 0;

        // Comandos
        public ICommand NovoCommand { get; }
        public ICommand EditarCommand { get; }
        public ICommand SalvarCommand { get; }
        public ICommand ExcluirCommand { get; }
        public ICommand CancelarCommand { get; }
        public ICommand IncluirPedidoCommand { get; }
        public ICommand LimparFiltrosCommand { get; }

        // Construtor
        public PessoaViewModel(IPessoaService pessoaService, IPedidoService pedidoService)
        {
            _pessoaService = pessoaService;
            _pedidoService = pedidoService;

            // Inicialização dos comandos
            NovoCommand = new RelayCommand(Novo);
            EditarCommand = new RelayCommand(Editar, CanEditarExcluir);
            SalvarCommand = new RelayCommand(Salvar, CanSalvar);
            ExcluirCommand = new RelayCommand(Excluir, CanEditarExcluir);
            CancelarCommand = new RelayCommand(Cancelar);
            IncluirPedidoCommand = new RelayCommand(IncluirPedido, CanIncluirPedido);
            LimparFiltrosCommand = new RelayCommand(LimparFiltros);

            // Inicialização das coleções
            PedidosDaPessoa = new ObservableCollection<Pedido>();

            CarregarPessoas();
        }

        // Métodos privados
        private void CarregarPedidosDaPessoaSelecionada()
        {
            if (PessoaSelecionada == null || PessoaSelecionada.Id <= 0)
            {
                LimparListaPedidos();
                return;
            }

            try
            {
                var pedidos = _pedidoService.ObterPorPessoa(PessoaSelecionada.Id);
                PedidosDaPessoa = new ObservableCollection<Pedido>(pedidos);
                OnPropertyChanged(nameof(TotalPedidos));
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao carregar pedidos: {ex.Message}",
                              "Erro",
                              MessageBoxButton.OK,
                              MessageBoxImage.Error);
                LimparListaPedidos();
            }
        }

        private void LimparListaPedidos()
        {
            PedidosDaPessoa = new ObservableCollection<Pedido>();
            OnPropertyChanged(nameof(TotalPedidos));
        }

        private void LimparFiltros()
        {
            FiltroNome = string.Empty;
            FiltroCPF = string.Empty;
            FiltrarPessoas();
        }

        private void CarregarPessoas()
        {
            try
            {
                Pessoas = _pessoaService.ObterTodos();
                PessoasFiltradas = new List<Pessoa>(Pessoas);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao carregar pessoas: {ex.Message}",
                              "Erro",
                              MessageBoxButton.OK,
                              MessageBoxImage.Error);
            }
        }

        private void FiltrarPessoas()
        {
            if (Pessoas == null) return;

            var query = Pessoas.AsEnumerable();

            if (!string.IsNullOrWhiteSpace(FiltroNome))
            {
                query = query.Where(p =>
                    p.Nome.Contains(FiltroNome, StringComparison.OrdinalIgnoreCase));
            }

            if (!string.IsNullOrWhiteSpace(FiltroCPF))
            {
                query = query.Where(p =>
                    p.CPF.Contains(FiltroCPF, StringComparison.OrdinalIgnoreCase));
            }

            PessoasFiltradas = query.ToList();
        }

        private void Novo()
        {
            PessoaEmEdicao = new Pessoa();
            EmModoEdicao = true;
        }

        private void Editar()
        {
            if (PessoaSelecionada != null)
            {
                PessoaEmEdicao = new Pessoa
                {
                    Id = PessoaSelecionada.Id,
                    Nome = PessoaSelecionada.Nome,
                    CPF = PessoaSelecionada.CPF,
                    Endereco = PessoaSelecionada.Endereco
                };
                EmModoEdicao = true;
            }
        }

        private bool CanEditarExcluir() => PessoaSelecionada != null && !EmModoEdicao;

        private bool CanSalvar() => PessoaEmEdicao != null &&
                                  !string.IsNullOrWhiteSpace(PessoaEmEdicao.Nome) &&
                                  !string.IsNullOrWhiteSpace(PessoaEmEdicao.CPF);

        private void Salvar()
        {
            try
            {
                if (PessoaEmEdicao.Id == 0)
                {
                    _pessoaService.Adicionar(PessoaEmEdicao);
                }
                else
                {
                    _pessoaService.Atualizar(PessoaEmEdicao);
                }

                EmModoEdicao = false;
                CarregarPessoas();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao salvar pessoa: {ex.Message}",
                                "Erro",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);
            }
        }

        private void Excluir()
        {
            if (PessoaSelecionada != null)
            {
                var resultado = MessageBox.Show(
                    $"Tem certeza que deseja excluir {PessoaSelecionada.Nome}?",
                    "Confirmação",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (resultado == MessageBoxResult.Yes)
                {
                    try
                    {
                        _pessoaService.Remover(PessoaSelecionada.Id);
                        CarregarPessoas();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Erro ao excluir pessoa: {ex.Message}",
                                        "Erro",
                                        MessageBoxButton.OK,
                                        MessageBoxImage.Error);
                    }
                }
            }
        }

        private void Cancelar()
        {
            EmModoEdicao = false;
            PessoaEmEdicao = null;

            if (PessoaSelecionada != null && Pessoas != null)
            {
                PessoaSelecionada = Pessoas.FirstOrDefault(p => p.Id == PessoaSelecionada.Id);
            }
        }

        private bool CanIncluirPedido() => PessoaSelecionada != null && !EmModoEdicao;

        private void IncluirPedido()
        {
            if (PessoaSelecionada != null)
            {
                try
                {
                    var pedidoWindow = new PedidoView();
                    pedidoWindow.ShowDialog();
                    CarregarPedidosDaPessoaSelecionada(); // Atualiza a lista após fechar
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Erro ao abrir tela de pedidos: {ex.Message}",
                                    "Erro",
                                    MessageBoxButton.OK,
                                    MessageBoxImage.Error);
                }
            }
        }
    }
}