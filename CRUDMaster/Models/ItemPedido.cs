using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace CRUDMaster.Models
{
    public class ItemPedido : INotifyPropertyChanged
    {
        public int ProdutoId { get; set; }
        public string ProdutoNome { get; set; }
        
        private int _quantidade;
        
        private decimal _valorUnitario;

        public int Quantidade
        {
            get => _quantidade;
            set
            {
                _quantidade = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(ValorTotal));
            }
        }

        public decimal ValorUnitario
        {
            get => _valorUnitario;
            set
            {
                _valorUnitario = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(ValorTotal));
            }
        }

        public decimal ValorTotal => ValorUnitario * Quantidade;

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
