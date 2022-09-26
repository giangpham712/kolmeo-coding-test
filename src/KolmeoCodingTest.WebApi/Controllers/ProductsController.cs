using KolmeoCodingTest.Application.Contracts.DTOs;
using KolmeoCodingTest.Application.Contracts.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace KolmeoCodingTest.WebApi.Controllers;

[ApiController]
[Route("products")]
public class ProductsController : ControllerBase
{
    private readonly IProductsService _productsService;

    public ProductsController(IProductsService productsService)
    {
        _productsService = productsService;
    }

    [HttpGet]
    [Route("")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<ProductDTO>>> ListProducts()
    {
        return Ok(await _productsService.ListProducts());
    }
    
    [HttpGet]
    [Route("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IEnumerable<ProductDTO>>> GetProduct([FromRoute] int id)
    {
        return Ok(await _productsService.GetProduct(id));
    }
    
    [HttpPost]
    [Route("")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ProductDTO>> CreateProduct([FromBody] CreateOrUpdateProductDTO product)
    {
        var created = await _productsService.CreateProduct(product);
        return CreatedAtAction(nameof(GetProduct), new { id = created.Id }, created);
    }
    
    [HttpPut]
    [Route("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProductDTO>> UpdateProduct([FromRoute] int id, [FromBody] CreateOrUpdateProductDTO product)
    {
        return Ok(await _productsService.UpdateProduct(id, product));
    }
    
    [HttpDelete]
    [Route("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> DeleteProduct([FromRoute] int id)
    {
        await _productsService.DeleteProduct(id);
        return NoContent();
    }
}