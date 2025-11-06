namespace Company.ProductService.Models;

public enum ProductErrorCode : short
{
    InternalServerError = 0,
    AuthenticationError = 1,
    AccessDenied = 2,
    ItemNotFound = 3,
    ItemAlreadyExists = 4,
    InvalidFormat = 5,
    InvalidValue = 6
}