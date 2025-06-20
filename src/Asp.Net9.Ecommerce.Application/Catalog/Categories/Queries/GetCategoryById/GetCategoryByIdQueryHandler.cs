using MediatR;
using Asp.Net9.Ecommerce.Application.Catalog.Categories.DTOs;
using Asp.Net9.Ecommerce.Application.Common.Interfaces.RepositoryInterfaces;
using AutoMapper;

namespace Asp.Net9.Ecommerce.Application.Catalog.Categories.Queries.GetCategoryById
{
    public class GetCategoryByIdQueryHandler : IRequestHandler<GetCategoryByIdQuery, CategoryNestedDto>
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IMapper _mapper;

        public GetCategoryByIdQueryHandler(ICategoryRepository categoryRepository, IMapper mapper)
        {
            _categoryRepository = categoryRepository;
            _mapper = mapper;
        }

        public async Task<CategoryNestedDto> Handle(GetCategoryByIdQuery request, CancellationToken cancellationToken)
        {
            var category = await _categoryRepository.GetCategoryByIdAsync(request.Id, cancellationToken);
            return _mapper.Map<CategoryNestedDto>(category);
        }
    }
} 