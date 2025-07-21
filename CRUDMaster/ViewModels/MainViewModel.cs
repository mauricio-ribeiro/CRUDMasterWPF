using CRUDMaster.Commands;
using CRUDMaster.Views;
using System.Windows.Input;

namespace CRUDMaster.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        public ICommand AbrirPessoasCommand { get; }
        public ICommand AbrirProdutosCommand { get; }
        public ICommand AbrirPedidosCommand { get; }

        private readonly PessoaViewModel _pessoaVM;
        private readonly ProdutoViewModel _produtoVM;
        private readonly PedidoViewModel _pedidoVM;

        public MainViewModel(
            PessoaViewModel pessoaVM,
            ProdutoViewModel produtoVM,
            PedidoViewModel pedidoVM)
        {
            _pessoaVM = pessoaVM;
            _produtoVM = produtoVM;
            _pedidoVM = pedidoVM;

            AbrirPessoasCommand = new RelayCommand(AbrirPessoas);
            AbrirProdutosCommand = new RelayCommand(AbrirProdutos);
            AbrirPedidosCommand = new RelayCommand(AbrirPedidos);
        }

        private void AbrirPessoas()
        {
            var window = new PessoaView { DataContext = _pessoaVM };
            window.Show();
        }

        private void AbrirProdutos()
        {
            var window = new ProdutoView { DataContext = _produtoVM };
            window.Show();
        }

        private void AbrirPedidos()
        {
            var window = new PedidoView { DataContext = _pedidoVM };
            window.Show();
        }
    }
}