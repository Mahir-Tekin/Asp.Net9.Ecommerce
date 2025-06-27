using Asp.Net9.Ecommerce.Application.Orders.DTOs;
using Asp.Net9.Ecommerce.Domain.Orders;
using AutoMapper;

namespace Asp.Net9.Ecommerce.Application.Orders.Mappings
{
    public class OrderMappingProfile : Profile
    {
        public OrderMappingProfile()
        {
            CreateMap<CreateOrderDto, Order>()
                .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.Items))
                .ForMember(dest => dest.ShippingAddress, opt => opt.MapFrom(src => src.ShippingAddress))
                .ForMember(dest => dest.Status, opt => opt.Ignore())
                .ForMember(dest => dest.TotalAmount, opt => opt.Ignore())
                .ForMember(dest => dest.Id, opt => opt.Ignore());

            CreateMap<OrderItemDto, OrderItem>()
                .ForMember(dest => dest.Id, opt => opt.Ignore());

            CreateMap<OrderAddressDto, OrderAddress>();
        }
    }
}
