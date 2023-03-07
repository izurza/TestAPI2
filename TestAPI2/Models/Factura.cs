using System;
using System.Collections.Generic;

namespace TestAPI2.Models;

public partial class Factura
{
    public int IdFactura { get; set; }

    public int? FkCliente { get; set; }

    public DateTime? Fecha { get; set; }

    public virtual ICollection<Detalle> Detalles { get; } = new List<Detalle>();

    public virtual Cliente? FkClienteNavigation { get; set; }
}
