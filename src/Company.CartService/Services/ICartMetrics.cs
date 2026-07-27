namespace Company.CartService.Services;

public interface ICartMetrics
{
    void ItemAdded();

    void CartViewed();
}