using System;
using System.Collections.Generic;

namespace TestAPI2.Models;

public partial class Detalle
{
    public int IdDetalle { get; set; }

    public int? FkFactura { get; set; }

    public double? Cantidad { get; set; }

    public int? FkProducto { get; set; }

    public virtual Factura? FkFacturaNavigation { get; set; }

    public virtual Producto? FkProductoNavigation { get; set; }
}
