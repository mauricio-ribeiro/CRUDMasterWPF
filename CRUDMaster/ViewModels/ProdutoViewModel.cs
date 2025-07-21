using CRUDMaster.Commands;
using CRUDMaster.Models;
using CRUDMaster.Services.Interfaces;
using System.Windows;
using System.Windows.Input;

namespace CRUDMaster.ViewModels
{
    public class ProdutoViewModel : ViewModelBase
    {
        private readonly IProdutoService _produtoService;
        private List<Produto> _produtos;
        private List<Produto> _produtosFiltrados;
        private Produto _produtoSelecionado;
        private Produto _produtoEmEdicao;
        private string _filtroNome;
        private string _filtroCodigo;
        private decimal? _valorMinimo;
        private decimal? _valorMaximo;
        private bool _emModoEdicao;

        public List<Produto> Produtos
        {
            get => _produtos;
            private set
            {
                SetField(ref _produtos, value);
                FiltrarProdutos();
            }
        }

        public List<Produto> ProdutosFiltrados
        {
            get => _produtosFiltrados;
            private set => SetField(ref _produtosFiltrados, value);
        }

        public Produto ProdutoSelecionado
        {
            get => _produtoSelecionado;
            set => SetField(ref _produtoSelecionado, value);
        }

        public Produto ProdutoEmEdicao
        {
            get => _produtoEmEdicao;
            set => SetField(ref _produtoEmEdicao, value);
        }

        public string FiltroNome
        {
            get => _filtroNome;
            set
            {
                if (SetField(ref _filtroNome, value))
                {
                    FiltrarProdutos();
                }
            }
        }

        public string FiltroCodigo
        {
            get => _filtroCodigo;
            set
            {
                if (SetField(ref _filtroCodigo, value))
                {
                    FiltrarProdutos();
                }
            }
        }

        public decimal? ValorMinimo
        {
            get => _valorMinimo;
            set
            {
                if (SetField(ref _valorMinimo, value))
                {
                    FiltrarProdutos();
                }
            }
        }

        public decimal? ValorMaximo
        {
            get => _valorMaximo;
            set
            {
                if (SetField(ref _valorMaximo, value))
                {
                    FiltrarProdutos();
                }
            }
        }

        public bool EmModoEdicao
        {
            get => _emModoEdicao;
            set => SetField(ref _emModoEdicao, value);
        }

        public ICommand NovoCommand { get; }
        public ICommand EditarCommand { get; }
        public ICommand SalvarCommand { get; }
        public ICommand ExcluirCommand { get; }
        public ICommand CancelarCommand { get; }
        public ICommand LimparFiltrosCommand { get; }

        public ProdutoViewModel(IProdutoService produtoService)
        {
            _produtoService = produtoService;

            // Inicialização dos comandos
            NovoCommand = new RelayCommand(Novo);
            EditarCommand = new RelayCommand(Editar, CanEditarExcluir);
            SalvarCommand = new RelayCommand(Salvar, CanSalvar);
            ExcluirCommand = new RelayCommand(Excluir, CanEditarExcluir);
            CancelarCommand = new RelayCommand(Cancelar);
            LimparFiltrosCommand = new RelayCommand(LimparFiltros);

            CarregarProdutos();
        }

        private void LimparFiltros()
        {
            FiltroNome = string.Empty;
            FiltroCodigo = string.Empty;
            ValorMinimo = null;
            ValorMaximo = null;

            // Força a atualização dos filtros
            FiltrarProdutos();
        }


        private void CarregarProdutos()
        {
            try
            {
                Produtos = _produtoService.ObterTodos();
                ProdutosFiltrados = new List<Produto>(Produtos);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao carregar produtos: {ex.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void FiltrarProdutos()
        {
            if (Produtos == null) return;

            var query = Produtos.AsEnumerable();

            if (!string.IsNullOrWhiteSpace(FiltroNome))
            {
                query = query.Where(p =>
                    p.Nome.Contains(FiltroNome, StringComparison.OrdinalIgnoreCase));
            }

            if (!string.IsNullOrWhiteSpace(FiltroCodigo))
            {
                query = query.Where(p =>
                    p.Codigo.Contains(FiltroCodigo, StringComparison.OrdinalIgnoreCase));
            }

            if (ValorMinimo.HasValue)
            {
                query = query.Where(p => p.Valor >= ValorMinimo.Value);
            }

            if (ValorMaximo.HasValue)
            {
                query = query.Where(p => p.Valor <= ValorMaximo.Value);
            }

            ProdutosFiltrados = query.ToList();
        }

        private void Novo()
        {
            ProdutoEmEdicao = new Produto();
            EmModoEdicao = true;
        }

        private void Editar()
        {
            if (ProdutoSelecionado != null)
            {
                ProdutoEmEdicao = new Produto
                {
                    Id = ProdutoSelecionado.Id,
                    Nome = ProdutoSelecionado.Nome,
                    Codigo = ProdutoSelecionado.Codigo,
                    Valor = ProdutoSelecionado.Valor
                };
                EmModoEdicao = true;
            }
        }

        private bool CanEditarExcluir() => ProdutoSelecionado != null && !EmModoEdicao;

        private bool CanSalvar() => ProdutoEmEdicao != null &&
                                  !string.IsNullOrWhiteSpace(ProdutoEmEdicao.Nome) &&
                                  !string.IsNullOrWhiteSpace(ProdutoEmEdicao.Codigo) &&
                                  ProdutoEmEdicao.Valor > 0;

        private void Salvar()
        {
            try
            {
                if (ProdutoEmEdicao.Id == 0)
                {
                    _produtoService.Adicionar(ProdutoEmEdicao);
                }
                else
                {
                    _produtoService.Atualizar(ProdutoEmEdicao);
                }

                EmModoEdicao = false;
                CarregarProdutos();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao salvar produto: {ex.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Excluir()
        {
            if (ProdutoSelecionado != null)
            {
                var resultado = MessageBox.Show(
                    $"Tem certeza que deseja excluir {ProdutoSelecionado.Nome}?",
                    "Confirmação",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (resultado == MessageBoxResult.Yes)
                {
                    try
                    {
                        _produtoService.Remover(ProdutoSelecionado.Id);
                        CarregarProdutos();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Erro ao excluir produto: {ex.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }

        private void Cancelar()
        {
            EmModoEdicao = false;
            ProdutoEmEdicao = null;

            if (ProdutoSelecionado != null && Produtos != null)
            {
                ProdutoSelecionado = Produtos.FirstOrDefault(p => p.Id == ProdutoSelecionado.Id);
            }
        }
    }
}