using AutoMapper;
using WebApplication4.DTOs;
using System.Linq;

public class PagedResultTypeConverter<TSource, TDestination> 
    : ITypeConverter<PagedResult<TSource>, PagedResult<TDestination>>
{
    private readonly IMapper _mapper;

    public PagedResultTypeConverter(IMapper mapper)
    {
        _mapper = mapper;
    }

    public PagedResult<TDestination> Convert(
        PagedResult<TSource> source,
        PagedResult<TDestination> destination,
        ResolutionContext context)
    {
        return new PagedResult<TDestination>
        {
            Items = source.Items.Select(x => _mapper.Map<TDestination>(x)).ToList(),
            Page = source.Page,
            PageSize = source.PageSize,
            TotalCount = source.TotalCount
        };
    }
}