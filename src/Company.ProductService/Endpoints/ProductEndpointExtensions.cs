namespace Company.ProductService.Endpoints;

public static class ProductEndpointExtensions
{
    public static void MapProductEndpoints(this IEndpointRouteBuilder app)
    {
        RouteGroupBuilder products = app.MapGroup("/products").WithTags("Products");

        products.MapListProducts();
        products.MapGetProduct();
        products.MapCreateProduct();
        products.MapUpdateProduct();
        products.MapDeleteProduct();
    }
}