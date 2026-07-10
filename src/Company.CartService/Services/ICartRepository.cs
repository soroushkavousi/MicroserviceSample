using Company.CartService.Models.Entities;

namespace Company.CartService.Services;

public interface ICartRepository
{
    Cart GetOrCreate(long userId);
}