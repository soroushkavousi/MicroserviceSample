using Company.CartService.Models.Dtos;
using Company.Shared.ValueObjects;

namespace Company.CartService.Services;

public interface ICartService
{
    Task<Result<CartDto>> GetCartAsync(long userId, CancellationToken cancellationToken);

    Task<Result<CartDto>> AddItemAsync(long userId, long productId, int quantity,
        CancellationToken cancellationToken);

    Task<Result<CartDto>> RemoveItemAsync(long userId, long productId,
        CancellationToken cancellationToken);

    Task<Result<CartDto>> ClearCartAsync(long userId, CancellationToken cancellationToken);
}