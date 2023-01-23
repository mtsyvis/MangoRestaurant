using AutoMapper;
using Mango.Services.OrderAPI.Messages;
using Mango.Services.OrderAPI.Models;

namespace Mango.Services.OrderAPI
{
    public class MappingConfig
    {
        public static MapperConfiguration RegisterMaps()
        {
            return new MapperConfiguration(config =>
            {
                config.CreateMap<CheckoutHeaderDto, OrderHeader>()
                    .ForMember(orderHeader => orderHeader.OrderTime, opt => opt.MapFrom(o => DateTime.Now));
            });
        }
    }
}
