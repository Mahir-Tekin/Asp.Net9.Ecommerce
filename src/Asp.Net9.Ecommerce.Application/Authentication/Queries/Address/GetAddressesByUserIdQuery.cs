using Asp.Net9.Ecommerce.Application.Authentication.DTOs;
using Asp.Net9.Ecommerce.Shared.Results;
using MediatR;
using System;
using System.Collections.Generic;

namespace Asp.Net9.Ecommerce.Application.Authentication.Queries.Address
{
    /// <summary>
    /// Query to get all addresses for a user
    /// </summary>
    public class GetAddressesByUserIdQuery : IRequest<Result<List<AddressResponse>>>
    {
        public Guid UserId { get; set; }
        public GetAddressesByUserIdQuery(Guid userId)
        {
            UserId = userId;
        }
    }
}
