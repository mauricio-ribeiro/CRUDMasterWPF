using CRUDMaster.Models.Enums;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace CRUDMaster.Models
{
    public class Pedido : INotifyPropertyChanged
    {
        public int Id { get; set; }
        public int PessoaId { get; set; }

        private string _pessoaNome;
        public string PessoaNome
        {
            get => _pessoaNome;
            set
            {
                _pessoaNome = value;
                OnPropertyChanged();
            }
        }

        private FormaPagamentoEnum? _formaPagamento;
        public FormaPagamentoEnum? FormaPagamento
        {
            get => _formaPagamento;
            set
            {
                if (_formaPagamento != value)
                {
                    _formaPagamento = value;
                    OnPropertyChanged();
                    // Notifica também sobre o valor total, caso precise atualizar na UI
                    OnPropertyChanged(nameof(ValorTotal));
                }
            }
        }

        public DateTime DataVenda { get; set; }
        public decimal ValorTotal { get; set; }
        public StatusPedidoEnum Status { get; set; }

        public ObservableCollection<ItemPedido> Produtos { get; set; } = new ObservableCollection<ItemPedido>();

        // Implementação INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
