using Unimag.Inventario.Domain.Common;

namespace Unimag.Inventario.Domain.Products.Exceptions;

public sealed class ProductBarcodeNotFoundException : DomainException
{
    public ProductBarcodeNotFoundException(string barcode)
        : base($"No product was found with barcode '{barcode}'.")
    {
    }
}
