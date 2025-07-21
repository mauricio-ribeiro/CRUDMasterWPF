using CRUDMaster.Commands;
using CRUDMaster.Extensions;
using CRUDMaster.Models;
using CRUDMaster.Models.Enums;
using CRUDMaster.Services.Interfaces;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;

namespace CRUDMaster.ViewModels
{
    public class PedidoViewModel : ViewModelBase
    {
        private readonly IPedidoService _pedidoService;
        private readonly IPessoaService _pessoaService;
        private readonly IProdutoService _produtoService;

        private List<Pedido> _pedidos;
        private List<Pedido> _pedidosFiltrados;
        private Pedido _pedidoAtual;
        private List<Pessoa> _pessoas;
        private Pessoa _pessoaSelecionada;
        private List<Produto> _produtosDisponiveis;
        private Produto _produtoSelecionado;
        private int _quantidade = 1;
        private bool _pedidoFinalizado;
        private string _filtroCliente;
        private StatusPedidoEnum? _filtroStatus;
        private DateTime? _filtroDataInicio;
        private DateTime? _filtroDataFim;
        private bool _mostrarCamposNovoPedido;
        private Pedido _pedidoSelecionadoNaLista;

        public bool MostrarCamposNovoPedido
        {
            get => _mostrarCamposNovoPedido;
            set => SetField(ref _mostrarCamposNovoPedido, value);
        }

        public List<Pedido> Pedidos
        {
            get => _pedidos;
            private set
            {
                SetField(ref _pedidos, value);
                FiltrarPedidos();
            }
        }

        public List<Pedido> PedidosFiltrados
        {
            get => _pedidosFiltrados;
            private set => SetField(ref _pedidosFiltrados, value);
        }
        public Pedido PedidoAtual
        {
            get => _pedidoAtual;
            set
            {
                if (SetField(ref _pedidoAtual, value))
                {
                    if (value != null)
                    {
                        value.PropertyChanged += (sender, args) =>
                        {
                            if (args.PropertyName == nameof(Pedido.FormaPagamento))
                            {
                                FinalizarPedidoCommand.RaiseCanExecuteChanged();
                                OnPropertyChanged(nameof(PodeFinalizarPedido));
                            }
                        };
                    }
                    FinalizarPedidoCommand.RaiseCanExecuteChanged();
                    OnPropertyChanged(nameof(PodeFinalizarPedido));
                }
            }
        }

        public List<Pessoa> Pessoas
        {
            get => _pessoas;
            set => SetField(ref _pessoas, value);
        }

        public Pessoa PessoaSelecionada
        {
            get => _pessoaSelecionada;
            set
            {
                if (SetField(ref _pessoaSelecionada, value) && value != null)
                {
                    PedidoAtual.PessoaId = value.Id;
                    // Adicione esta linha para atualizar os comandos
                    AdicionarProdutoCommand.RaiseCanExecuteChanged();
                }
            }
        }

        public List<Produto> ProdutosDisponiveis
        {
            get => _produtosDisponiveis;
            set => SetField(ref _produtosDisponiveis, value);
        }

        public Produto ProdutoSelecionado
        {
            get => _produtoSelecionado;
            set => SetField(ref _produtoSelecionado, value);
        }

        public int Quantidade
        {
            get => _quantidade;
            set => SetField(ref _quantidade, value);
        }

        public bool PedidoFinalizado
        {
            get => _pedidoFinalizado;
            set => SetField(ref _pedidoFinalizado, value);
        }

        public string FiltroCliente
        {
            get => _filtroCliente;
            set
            {
                if (SetField(ref _filtroCliente, value))
                {
                    FiltrarPedidos();
                }
            }
        }

        public StatusPedidoEnum? FiltroStatus
        {
            get => _filtroStatus;
            set
            {
                if (SetField(ref _filtroStatus, value))
                {
                    FiltrarPedidos();
                }
            }
        }

        public DateTime? FiltroDataInicio
        {
            get => _filtroDataInicio;
            set
            {
                if (SetField(ref _filtroDataInicio, value))
                {
                    FiltrarPedidos();
                }
            }
        }

        public DateTime? FiltroDataFim
        {
            get => _filtroDataFim;
            set
            {
                if (SetField(ref _filtroDataFim, value))
                {
                    FiltrarPedidos();
                }
            }
        }

        public IEnumerable<FormaPagamentoEnum> FormasPagamento => Enum.GetValues(typeof(FormaPagamentoEnum)).Cast<FormaPagamentoEnum>();
        public IEnumerable<KeyValuePair<StatusPedidoEnum?, string>> StatusDisponiveis
        {
            get
            {
                yield return new KeyValuePair<StatusPedidoEnum?, string>(null, "Todos");

                foreach (StatusPedidoEnum status in Enum.GetValues(typeof(StatusPedidoEnum)))
                {
                    yield return new KeyValuePair<StatusPedidoEnum?, string>(
                        status,
                        status.GetDisplayName() ?? status.ToString()
                    );
                }
            }
        }

        public RelayCommand AdicionarProdutoCommand { get; }
        public ICommand RemoverProdutoCommand { get; }
        public RelayCommand FinalizarPedidoCommand { get; }
        public ICommand NovoPedidoCommand { get; }
        public ICommand LimparFiltrosCommand { get; }
        public ICommand AtualizarStatusCommand { get; }

        public PedidoViewModel(IPedidoService pedidoService, IPessoaService pessoaService, IProdutoService produtoService)
        {
            _pedidoService = pedidoService;
            _pessoaService = pessoaService;
            _produtoService = produtoService;

            AdicionarProdutoCommand = new RelayCommand(AdicionarProduto, CanAdicionarProduto);
            RemoverProdutoCommand = new RelayCommand<ItemPedido>(RemoverProduto);
            FinalizarPedidoCommand = new RelayCommand(FinalizarPedido, CanFinalizarPedido);
            NovoPedidoCommand = new RelayCommand(() => NovoPedido());
            LimparFiltrosCommand = new RelayCommand(LimparFiltros);
            AtualizarStatusCommand = new RelayCommand<StatusPedidoEnum>(AtualizarStatus);

            PropertyChanged += (sender, args) =>
            {
                // Verificação para habilitar o botão Adicionar
                if (args.PropertyName == nameof(MostrarCamposNovoPedido) ||
                    args.PropertyName == nameof(PessoaSelecionada) ||
                    args.PropertyName == nameof(ProdutoSelecionado) ||
                    args.PropertyName == nameof(Quantidade) ||
                    args.PropertyName == nameof(PedidoFinalizado))
                {
                    AdicionarProdutoCommand.RaiseCanExecuteChanged();
                }

                // Verificação existente para o PedidoAtual
                if (args.PropertyName == nameof(PedidoAtual) && PedidoAtual != null)
                {
                    PedidoAtual.PropertyChanged += (pedidoSender, pedidoArgs) =>
                    {
                        if (pedidoArgs.PropertyName == nameof(Pedido.FormaPagamento))
                        {
                            FinalizarPedidoCommand.RaiseCanExecuteChanged();
                            OnPropertyChanged(nameof(PodeFinalizarPedido));
                        }
                    };
                }
            };

            // Inicia com campos ocultos
            MostrarCamposNovoPedido = false;

            CarregarPessoas();
            CarregarProdutosDisponiveis();
            CarregarTodosPedidos();

            // Inicializa um pedido vazio (mas não mostra os campos)
            NovoPedido(silent: true);
            OnPropertyChanged(nameof(StatusDisponiveis));
        }

        public bool PodeFinalizarPedido => CanFinalizarPedido();

        // Modifique o método NovoPedido para notificar a mudança no comando
        public void NovoPedido(bool silent = false)
        {
            PedidoSelecionadoNaLista = null;

            PedidoAtual = new Pedido
            {
                Produtos = new ObservableCollection<ItemPedido>(),
                DataVenda = DateTime.Now,
                Status = StatusPedidoEnum.Pendente,
                FormaPagamento = null
            };

            PedidoFinalizado = false;
            PessoaSelecionada = null;
            ProdutoSelecionado = null;
            Quantidade = 1;

            if (!silent)
            {
                MostrarCamposNovoPedido = true;
            }

            // Notifique a mudança nos comandos
            AdicionarProdutoCommand.RaiseCanExecuteChanged();
            FinalizarPedidoCommand.RaiseCanExecuteChanged();
        }

        private void CarregarTodosPedidos()
        {
            try
            {
                Pedidos = _pedidoService.ObterTodos();
                PedidosFiltrados = new List<Pedido>(Pedidos); // Inicializa com todos os pedidos
                OnPropertyChanged(nameof(PedidosFiltrados));
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao carregar pedidos: {ex.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void CarregarPedidosPorPessoa(int pessoaId)
        {
            try
            {
                Pedidos = _pedidoService.ObterPorPessoa(pessoaId);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao carregar pedidos: {ex.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CarregarPessoas()
        {
            try
            {
                Pessoas = _pessoaService.ObterTodos();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao carregar pessoas: {ex.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CarregarProdutosDisponiveis()
        {
            try
            {
                ProdutosDisponiveis = _produtoService.ObterTodos();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao carregar produtos: {ex.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void FiltrarPedidos()
        {
            if (Pedidos == null) return;

            var query = Pedidos.AsQueryable();

            if (!string.IsNullOrWhiteSpace(FiltroCliente))
            {
                query = query.Where(p =>
                    p.PessoaNome != null &&
                    p.PessoaNome.Contains(FiltroCliente, StringComparison.OrdinalIgnoreCase));
            }

            if (FiltroStatus.HasValue)
            {
                query = query.Where(p => p.Status == FiltroStatus.Value);
            }

            if (FiltroDataInicio.HasValue)
            {
                query = query.Where(p => p.DataVenda.Date >= FiltroDataInicio.Value.Date);
            }

            if (FiltroDataFim.HasValue)
            {
                query = query.Where(p => p.DataVenda.Date <= FiltroDataFim.Value.Date);
            }

            PedidosFiltrados = query.ToList();

            // Limpa os itens do pedido atual quando um filtro é aplicado
            if (PedidoAtual != null && PedidoAtual.Id == 0) // Só limpa se não for um pedido novo
            {
                PedidoAtual = new Pedido
                {
                    Produtos = new ObservableCollection<ItemPedido>()
                };
            }

            // Limpa a seleção
            PedidoSelecionadoNaLista = null;

            OnPropertyChanged(nameof(PedidosFiltrados));
            OnPropertyChanged(nameof(PedidoAtual));
        }

        private void LimparFiltros()
        {
            FiltroCliente = string.Empty;
            FiltroStatus = null;
            FiltroDataInicio = null;
            FiltroDataFim = null;

            // Recarrega a lista completa
            PedidosFiltrados = new List<Pedido>(Pedidos);

            // Limpa os itens do pedido atual
            if (PedidoAtual != null && PedidoAtual.Id == 0) // Só limpa se não for um pedido novo
            {
                PedidoAtual = new Pedido
                {
                    Produtos = new ObservableCollection<ItemPedido>()
                };
            }

            // Limpa a seleção
            PedidoSelecionadoNaLista = null;

            OnPropertyChanged(nameof(PedidosFiltrados));
            OnPropertyChanged(nameof(PedidoAtual));
        }

        private bool CanAdicionarProduto() =>
            ProdutoSelecionado != null &&
            Quantidade > 0 &&
            !PedidoFinalizado &&
            MostrarCamposNovoPedido &&
            PessoaSelecionada != null;

        public void AdicionarProduto()
        {
            if (ProdutoSelecionado == null || Quantidade <= 0 || PedidoAtual == null)
                return;

            // Verifica se o produto já existe no pedido
            var itemExistente = PedidoAtual.Produtos
                .FirstOrDefault(p => p.ProdutoId == ProdutoSelecionado.Id && p.ProdutoId > 0);

            if (itemExistente != null)
            {
                itemExistente.Quantidade += Quantidade;
            }
            else
            {
                var novoItem = new ItemPedido
                {
                    ProdutoId = ProdutoSelecionado.Id,
                    ProdutoNome = ProdutoSelecionado.Nome,
                    ValorUnitario = ProdutoSelecionado.Valor,
                    Quantidade = Quantidade
                };

                PedidoAtual.Produtos.Add(novoItem);
            }

            CalcularValorTotal();
            ProdutoSelecionado = null;
            Quantidade = 1;
        }

        public void RemoverProduto(ItemPedido item)
        {
            if (item == null || 
                PedidoAtual?.Produtos == null ||
                !PedidoAtual.Produtos.Contains(item))
                return;

            try
            {
                PedidoAtual.Produtos.Remove(item);
                CalcularValorTotal();
                OnPropertyChanged(nameof(PedidoAtual));
                OnPropertyChanged(nameof(PedidoAtual.Produtos));
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao remover produto: {ex.Message}", "Erro",
                               MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CalcularValorTotal()
        {
            PedidoAtual.ValorTotal = PedidoAtual.Produtos.Sum(p => p.ValorTotal);
            OnPropertyChanged(nameof(PedidoAtual));
        }

        private bool CanFinalizarPedido()
        {
            if (PedidoAtual == null || PedidoFinalizado)
                return false;

            bool clienteValido = PedidoAtual.PessoaId > 0;
            bool temProdutos = PedidoAtual.Produtos?.Count > 0;
            bool formaPagamentoSelecionada = PedidoAtual.FormaPagamento != null;

            return clienteValido && temProdutos && formaPagamentoSelecionada;
        }

        public void FinalizarPedido()
        {
            if (!CanFinalizarPedido())
            {
                MessageBox.Show("Preencha todos os dados obrigatórios do pedido antes de finalizar.",
                              "Atenção", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                // Validação explícita da forma de pagamento
                if (PedidoAtual.FormaPagamento == null)
                {
                    MessageBox.Show("Selecione uma forma de pagamento antes de finalizar o pedido.",
                                  "Forma de Pagamento Obrigatória", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Garante que todos os dados estão preenchidos
                if (PessoaSelecionada != null)
                {
                    PedidoAtual.PessoaId = PessoaSelecionada.Id;
                    PedidoAtual.PessoaNome = PessoaSelecionada.Nome;
                }

                // Completa os dados do pedido
                PedidoAtual.DataVenda = DateTime.Now;
                PedidoAtual.ValorTotal = PedidoAtual.Produtos.Sum(p => p.ValorTotal);

                // Persiste o pedido (já validamos que FormaPagamento não é null)
                _pedidoService.Adicionar(PedidoAtual);

                // Atualiza a UI
                PedidoFinalizado = true;
                MostrarCamposNovoPedido = false;
                CarregarTodosPedidos();

                MessageBox.Show($"Pedido #{PedidoAtual.Id} finalizado com sucesso!\n" +
                               $"Cliente: {PedidoAtual.PessoaNome}\n" +
                               $"Forma de Pagamento: {PedidoAtual.FormaPagamento.GetValueOrDefault().ToString()}",
                              "Sucesso", MessageBoxButton.OK, MessageBoxImage.Information);

                // Prepara para novo pedido
                NovoPedido();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao finalizar pedido: {ex.Message}",
                               "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void AtualizarStatus(StatusPedidoEnum novoStatus)
        {
            if (PedidoAtual == null || PedidoAtual.Id == 0) return;

            try
            {
                PedidoAtual.Status = novoStatus;
                _pedidoService.Atualizar(PedidoAtual);
                CarregarTodosPedidos();
                MessageBox.Show("Status atualizado com sucesso!", "Sucesso", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao atualizar status: {ex.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public Pedido PedidoSelecionadoNaLista
        {
            get => _pedidoSelecionadoNaLista;
            set
            {
                if (SetField(ref _pedidoSelecionadoNaLista, value))
                {
                    // Quando um pedido é selecionado, carrega seus itens
                    CarregarItensPedidoSelecionado();
                }
            }
        }

        private void CarregarItensPedidoSelecionado()
        {
            if (PedidoSelecionadoNaLista == null)
            {
                // Limpa os itens se nenhum pedido estiver selecionado
                PedidoAtual = new Pedido
                {
                    Produtos = new ObservableCollection<ItemPedido>()
                };
                return;
            }

            // Atualiza o pedido atual para visualização
            PedidoAtual = new Pedido
            {
                Id = PedidoSelecionadoNaLista.Id,
                PessoaId = PedidoSelecionadoNaLista.PessoaId,
                PessoaNome = PedidoSelecionadoNaLista.PessoaNome,
                DataVenda = PedidoSelecionadoNaLista.DataVenda,
                ValorTotal = PedidoSelecionadoNaLista.ValorTotal,
                Status = PedidoSelecionadoNaLista.Status,
                FormaPagamento = PedidoSelecionadoNaLista.FormaPagamento,
                Produtos = new ObservableCollection<ItemPedido>(PedidoSelecionadoNaLista.Produtos)
            };

            // Esconde os campos de novo pedido quando visualizando um existente
            MostrarCamposNovoPedido = false;
            // Desabilita edição para pedidos já finalizados
            PedidoFinalizado = true;

            OnPropertyChanged(nameof(PedidoAtual));
        }
    }
}