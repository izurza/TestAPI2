namespace TestAPI2.Models.DTOs;

public class DetalleDto
{

    public FacturaDto? Factura { get; set; }

    public double? Cantidad { get; set; }

    public ProductoDto Producto { get; set; }

}
