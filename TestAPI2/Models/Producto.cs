using System;
using System.Collections.Generic;

namespace TestAPI2.Models;

public partial class Producto
{
    public int IdProducto { get; set; }

    public string? NombreProducto { get; set; }

    public string? DescripcionProducto { get; set; }

    public double? PrecioProducto { get; set; }

    public virtual ICollection<Detalle> Detalles { get; } = new List<Detalle>();
}
