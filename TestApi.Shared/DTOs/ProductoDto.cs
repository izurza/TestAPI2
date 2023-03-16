using System.ComponentModel.DataAnnotations;

namespace TestAPI2.Models.DTOs;

public class ProductoDto
{
    [Required(ErrorMessage = "Indique el numero de tecla para este producto")]
    public int IdProducto { get; set; }
    [Required(ErrorMessage = "Indica el nombre de este producto")]
    [RegularExpression("[a-zA-Z]{0,10}")]
    public string? NombreProducto { get; set; }

    public string? DescripcionProducto { get; set; }
    [Required(ErrorMessage = "Indique el precio del producto")]
    public double? PrecioProducto { get; set; }

}
