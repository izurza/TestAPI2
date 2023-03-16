using Carter;
using Microsoft.AspNetCore.Mvc;
using TestAPI2.Models.DTOs;
using TestAPI2.Models;
using TestAPI2.Services.Interfaces;
using TestAPI2.Services;
using Microsoft.Identity.Client;

namespace TestAPI2.Modulos;

public class ProductModule : CarterModule
{
    public ProductModule()
    {

    }
    public override void AddRoutes(IEndpointRouteBuilder app)
    {
        #region Productos

        var productos = app.MapGroup("/Productos").WithTags("Productos");

        productos.MapGet("/", async(IProductService productService) =>
        {
            return await productService.GetProductosAsync()
                is List<ProductoDto> products
                    ? Results.Ok(products)
                    : Results.NotFound();
        })
            .Produces<List<ProductoDto>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status204NoContent);

        productos.MapGet("/{id:int}", async([FromRoute] int id, IProductService productService) =>
        {
            return await productService.GetProductoAsync(id)
                is ProductoDto productoDto
                    ? Results.Ok(productoDto)
                    : Results.NotFound();
        })
            .Produces<ProductoDto>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);

        productos.MapPost("", async (Producto product, IProductService productService) =>
        {
            return await productService.AddProductoAsync(product)
                is ProductoDto productoDto
                    ? Results.Ok(productoDto)
                    : Results.BadRequest();
        })
            .Produces<ProductoDto>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest);

        productos.MapPut("/{id:int}", async(Producto product, IProductService productService) =>
    {
            return await productService.UpdateProductoAsync(product)
                is ProductoDto productoDto
                    ? Results.Ok(productoDto)
                    : Results.BadRequest();
        })
            .Produces<ProductoDto>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest);

        productos.MapDelete("/{id:int}", async ([FromRoute] int id, IProductService productService) =>
    {
            var result = await productService.DeleteProductoAsync(id);
            return result.Item1 ? Results.Ok(result.Item2) : Results.BadRequest(result.Item2);
        })
            .Produces<(bool, string)>(StatusCodes.Status200OK)
            .Produces<(bool, string)>(StatusCodes.Status400BadRequest);
        
        productos.MapDelete("/{nombre}", async ([FromRoute] string nombre, IProductService productService) =>
    {
            var result = await productService.DeleteProductoAsync(nombre);
            return result.Item1 ? Results.Ok(result.Item2) : Results.BadRequest(result.Item2);
        })
            .Produces<(bool, string)>(StatusCodes.Status200OK)
            .Produces<(bool, string)>(StatusCodes.Status400BadRequest);
        #endregion Productos
    }
}
