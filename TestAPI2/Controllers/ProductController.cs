using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TestAPI2.Models;
using TestAPI2.Services.Interfaces;

namespace TestAPI2.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpGet("productos")]
        [AllowAnonymous]
        public async Task<IActionResult> GetProductos()
        {
            var productos = await _productService.GetProductosAsync();

            if (productos is null) 
            {
                return StatusCode(StatusCodes.Status204NoContent, "No se han encontrado productos");
            }
            return StatusCode(StatusCodes.Status200OK, productos);
        }

        [HttpGet("producto/{id:int}")]
        [Authorize]
        public async Task<IActionResult> GetProductos([FromRoute] int id)
        {
            var productos = await _productService.GetProductoAsync(id);

            if (productos is null)
            {
                return StatusCode(StatusCodes.Status204NoContent, "No se han encontrado productos");
            }
            return StatusCode(StatusCodes.Status200OK, productos);
        }

        [HttpPost("Nuevo Producto")]
        [Authorize("write:Productos")]
        public async Task<IActionResult> AddProduct([FromForm] Producto product)
        {
            //insertar producto
            throw new NotImplementedException();
            //return StatusCode(StatusCodes.Status200OK, product);
        }

        [HttpPut("Modificar Producto")]
        [Authorize("write:Productos")]
        public async Task<IActionResult> UpdateProduct([FromForm] Producto product)
        {
            //Modificar producto
            throw new NotImplementedException();
            //return StatusCode(StatusCodes.Status200OK, product);
        }

        [HttpDelete("Descatalogar Producto/{id:int}")]
        [Authorize("delete:Productos")]
        public async Task<IActionResult> DeleteProduct([FromRoute] int id)
        {
            //Modificar producto
            throw new NotImplementedException();
            //return StatusCode(StatusCodes.Status200OK, "");
        }

    }
}
