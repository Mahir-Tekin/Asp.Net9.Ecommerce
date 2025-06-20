using Asp.Net9.Ecommerce.Application.Catalog.Products.Queries.GetProducts;

namespace Asp.Net9.Ecommerce.Application.Catalog.Products.DTOs
{
    public class ProductListResponse
    {
        public List<ProductListDto> Items { get; set; }
        public int TotalItems { get; set; }
        public int TotalPages { get; set; }
        public int CurrentPage { get; set; }
        public bool HasPreviousPage { get; set; }
        public bool HasNextPage { get; set; }
    }
} 