using Company.CartService.Errors;
using Company.CartService.Models.Dtos;
using Company.CartService.Models.Entities;
using Company.Shared.ProductService;
using Company.Shared.ProductService.Protos;
using Company.Shared.ValueObjects;

namespace Company.CartService.Services;

public sealed class CartService(ICartMetrics cartMetrics, ICartRepository cartRepository,
    IProductServiceClient productServiceClient) : ICartService
{
    public async Task<Result<CartDto>> GetCartAsync(long userId,
        CancellationToken cancellationToken)
    {
        Cart cart = cartRepository.GetOrCreate(userId);
        
        Result<CartDto> result = await BuildCartDtoAsync(cart, cancellationToken);
        if (result.IsSuccess)
            cartMetrics.CartViewed();
        
        return result;
    }

    public async Task<Result<CartDto>> AddItemAsync(long userId, long productId, int quantity,
        CancellationToken cancellationToken)
    {
        if (quantity <= 0)
            return CartErrorCode.CartInvalidQuantity;

        Result<ProductContractDto> productResult =
            await productServiceClient.GetProductAsync(productId);
        if (productResult.HasError)
            return CartErrorCode.ProductNotFound;

        Cart cart = cartRepository.GetOrCreate(userId);
        cart.AddItem(productId, quantity);
        cartMetrics.ItemAdded();
        
        return await BuildCartDtoAsync(cart, cancellationToken);
    }

    public async Task<Result<CartDto>> RemoveItemAsync(long userId, long productId,
        CancellationToken cancellationToken)
    {
        Cart cart = cartRepository.GetOrCreate(userId);
        if (!cart.RemoveItem(productId))
            return CartErrorCode.CartLineNotFound;

        return await BuildCartDtoAsync(cart, cancellationToken);
    }

    public async Task<Result<CartDto>> ClearCartAsync(long userId,
        CancellationToken cancellationToken)
    {
        Cart cart = cartRepository.GetOrCreate(userId);
        cart.Clear();
        return await BuildCartDtoAsync(cart, cancellationToken);
    }

    private async Task<Result<CartDto>> BuildCartDtoAsync(Cart cart,
        CancellationToken cancellationToken)
    {
        if (cart.Lines.Count == 0)
        {
            CartDto emptyCart = new()
            {
                Lines = [],
                Total = 0
            };
            return emptyCart;
        }

        long[] productIds = cart.Lines.Select(x => x.ProductId).Distinct().ToArray();
        Result<ProductContractDto[]> productsResult =
            await productServiceClient.ListProductsByIdsAsync(productIds);
        if (productsResult.HasError)
        {
            CartDto emptyCart = new()
            {
                Lines = [],
                Total = 0
            };
            return emptyCart;
        }

        Dictionary<long, ProductContractDto> productsById =
            productsResult.Data.ToDictionary(x => x.Id);
        List<CartLineDto> lines = [];

        foreach (CartLine line in cart.Lines)
        {
            if (!productsById.TryGetValue(line.ProductId, out ProductContractDto product))
                continue;

            lines.Add(new()
            {
                ProductId = line.ProductId,
                ProductName = product.Name,
                UnitPrice = product.Price,
                Quantity = line.Quantity
            });
        }

        CartDto cartDto = new()
        {
            Lines = lines.ToArray(),
            Total = lines.Sum(x => x.LineTotal)
        };
        return cartDto;
    }
}