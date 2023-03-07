using System;
using System.Collections.Generic;

namespace TestAPI2.Models;

public partial class Cliente
{
    public int IdCliente { get; set; }

    public string? NombreCliente { get; set; }

    public string? ApellidoCliente { get; set; }

    public string? EmailCliente { get; set; }

    public string? DireccionCliente { get; set; }

    public virtual ICollection<Factura> Facturas { get; } = new List<Factura>();
}
