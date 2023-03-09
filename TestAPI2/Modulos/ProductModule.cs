using Carter;
using Microsoft.AspNetCore.Mvc;
using TestAPI2.Models.DTOs;
using TestAPI2.Models;
using TestAPI2.Services.Interfaces;

namespace TestAPI2.Modulos;

public class ProductModule : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        #region Productos

        var productos = app.MapGroup("/Productos");

        productos.MapGet("/", GetProductos)
            .Produces<List<ProductoDto>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status204NoContent);

        productos.MapGet("/{id:int}", GetProducto)
            .Produces<ProductoDto>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);

        productos.MapPost("", AddProducto)
            .Produces<ProductoDto>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest);

        productos.MapPut("/{id:int}", UpdateProducto)
            .Produces<ProductoDto>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest);

        productos.MapDelete("/{id:int}", DeleteProducto)
            .Produces<(bool, string)>(StatusCodes.Status200OK)
            .Produces<(bool, string)>(StatusCodes.Status400BadRequest);
        #endregion Productos
    }
    #region Product Functions
    static async Task<IResult> GetProductos(IProductService productService)
    {
        return await productService.GetProductosAsync()
            is List<ProductoDto> products
                ? Results.Ok(products)
                : Results.NotFound();
    }
    static async Task<IResult> GetProducto([FromRoute] int id, IProductService productService)
    {
        return await productService.GetProductoAsync(id)
            is ProductoDto productoDto
                ? Results.Ok(productoDto)
                : Results.NotFound();
    }
    static async Task<IResult> AddProducto(Producto product, IProductService productService)
    {
        return await productService.AddProductoAsync(product)
            is ProductoDto productoDto
                ? Results.Ok(productoDto)
                : Results.BadRequest();
    }
    static async Task<IResult> UpdateProducto(Producto product, IProductService productService)
    {
        return await productService.UpdateProductoAsync(product)
            is ProductoDto productoDto
                ? Results.Ok(productoDto)
                : Results.BadRequest();
    }
    static async Task<IResult> DeleteProducto([FromRoute] int id, IProductService productService)
    {
        var result = await productService.DeleteProductoAsync(id);
        return result.Item1 ? Results.Ok(result.Item2) : Results.BadRequest(result.Item2);
    }
    #endregion Product Functions
}
