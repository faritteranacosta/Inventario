using Microsoft.AspNetCore.Mvc;
using Unimag.Inventario.Api.Products.Mappers;
using Unimag.Inventario.Api.Products.Requests;
using Unimag.Inventario.Application.Common.Interfaces;
using Unimag.Inventario.Application.Products.Commands.CreateProduct;
using Unimag.Inventario.Application.Products.Commands.DeleteProduct;
using Unimag.Inventario.Application.Products.Commands.SetProductImage;
using Unimag.Inventario.Application.Products.Commands.UpdateProduct;
using Unimag.Inventario.Application.Products.Dtos;
using Unimag.Inventario.Application.Products.Queries.GetProductById;
using Unimag.Inventario.Application.Products.Queries.ListProducts;

namespace Unimag.Inventario.Api.Products;

[ApiController]
[Route("api/[controller]")]
public sealed class ProductsController : ControllerBase
{
    private readonly IUseCase<CreateProductCommand, ProductResponse> _create;
    private readonly IUseCase<UpdateProductCommand, ProductResponse> _update;
    private readonly IUseCase<DeleteProductCommand, bool> _delete;
    private readonly IUseCase<SetProductImageCommand, ProductResponse> _setImage;
    private readonly IUseCase<GetProductByIdQuery, ProductResponse> _getById;
    private readonly IUseCase<ListProductsQuery, IReadOnlyList<ProductResponse>> _list;

    public ProductsController(
        IUseCase<CreateProductCommand, ProductResponse> create,
        IUseCase<UpdateProductCommand, ProductResponse> update,
        IUseCase<DeleteProductCommand, bool> delete,
        IUseCase<SetProductImageCommand, ProductResponse> setImage,
        IUseCase<GetProductByIdQuery, ProductResponse> getById,
        IUseCase<ListProductsQuery, IReadOnlyList<ProductResponse>> list)
    {
        _create = create;
        _update = update;
        _delete = delete;
        _setImage = setImage;
        _getById = getById;
        _list = list;
    }

    [HttpPost]
    public async Task<ActionResult<ProductResponse>> Create([FromBody] CreateProductRequest request, CancellationToken cancellationToken)
    {
        var result = await _create.ExecuteAsync(ProductMapper.ToCreateCommand(request), cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<ProductResponse>>> List([FromQuery] Guid? categoryId, [FromQuery] string? search, CancellationToken cancellationToken)
    {
        var result = await _list.ExecuteAsync(ProductMapper.ToListQuery(categoryId, search), cancellationToken);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ProductResponse>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _getById.ExecuteAsync(ProductMapper.ToGetQuery(id), cancellationToken);
        return Ok(result);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ProductResponse>> Update(Guid id, [FromBody] UpdateProductRequest request, CancellationToken cancellationToken)
    {
        var result = await _update.ExecuteAsync(ProductMapper.ToUpdateCommand(id, request), cancellationToken);
        return Ok(result);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        await _delete.ExecuteAsync(ProductMapper.ToDeleteCommand(id), cancellationToken);
        return NoContent();
    }

    [HttpPost("{id:guid}/image")]
    public async Task<ActionResult<ProductResponse>> SetImage(Guid id, [FromBody] SetProductImageRequest request, CancellationToken cancellationToken)
    {
        var result = await _setImage.ExecuteAsync(ProductMapper.ToSetImageCommand(id, request), cancellationToken);
        return Ok(result);
    }
}