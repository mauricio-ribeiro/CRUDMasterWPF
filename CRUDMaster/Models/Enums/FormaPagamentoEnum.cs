using System.ComponentModel;

namespace CRUDMaster.Models.Enums
{
    public enum FormaPagamentoEnum
    {
        [Description("Dinheiro")]
        Dinheiro,
        [Description("Cartão")]
        Cartao,
        [Description("Boleto")]
        Boleto
    }
}
