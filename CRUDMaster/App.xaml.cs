using CRUDMaster.Services;
using CRUDMaster.ViewModels;
using CRUDMaster.Views;
using System.Windows;

namespace CRUDMaster
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Configuração das dependências
            var dataService = new DataService();
            var pessoaService = new PessoaService(dataService);
            var produtoService = new ProdutoService(dataService);
            var pedidoService = new PedidoService(dataService);

            // Criação dos ViewModels - AGORA PASSANDO AMBOS OS SERVIÇOS NECESSÁRIOS
            var pessoaVM = new PessoaViewModel(pessoaService, pedidoService); // Corrigido aqui
            var produtoVM = new ProdutoViewModel(produtoService);
            var pedidoVM = new PedidoViewModel(pedidoService, pessoaService, produtoService);

            // Crie a MainWindow e defina o DataContext
            var mainWindow = new MainWindow
            {
                DataContext = new MainViewModel(pessoaVM, produtoVM, pedidoVM)
            };

            mainWindow.Show();
        }
    }
}