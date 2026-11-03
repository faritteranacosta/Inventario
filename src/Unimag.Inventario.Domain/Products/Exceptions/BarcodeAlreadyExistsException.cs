using Unimag.Inventario.Domain.Common;

namespace Unimag.Inventario.Domain.Products.Exceptions;

public sealed class BarcodeAlreadyExistsException : DomainException
{
    public BarcodeAlreadyExistsException(string barcode) : base($"Barcode '{barcode}' already exists.")
    {
    }
}
