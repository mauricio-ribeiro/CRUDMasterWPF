using System.ComponentModel;

namespace CRUDMaster.Models.Enums
{
    public enum StatusPedidoEnum
    {
        [Description("Pendente")]
        Pendente,
        [Description("Pago")]
        Pago,
        [Description("Enviado")]
        Enviado,
        [Description("Recebido")]
        Recebido
    }
}
