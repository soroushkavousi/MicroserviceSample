using Company.Shared.Dtos;
using Company.Shared.ValueObjects;

namespace Company.Shared.Mappers;

public static class PaginationDtoMappers
{
    public static Pagination ToPagination(this PaginationDto pagination)
        => new(pagination.PageNumber, pagination.PageSize, pagination.TotalItems);

    public static PaginationDto ToDto(this Pagination pagination)
        => new()
        {
            PageNumber = pagination.PageNumber,
            PageSize = pagination.PageSize,
            TotalItems = pagination.TotalItems ?? 0,
            TotalPages = pagination.TotalPages ?? 0
        };
}
