using System.Collections.Generic;

namespace Asp.Net9.Ecommerce.Application.Orders.DTOs
{
    public class AdminOrderListResultDto
    {
        public int TotalCount { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public List<AdminOrderListItemDto> Orders { get; set; }
    }
}
