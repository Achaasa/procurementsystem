using AutoMapper;
using procurementsystem.Entities;
using procurementsystem.models.ProcurementHistory;
namespace procurementsystem.Automapper
{
    public class ProcurementHistoryProfile : Profile
    {
        public ProcurementHistoryProfile()
        {
            CreateMap<CreateProcurementHistoryDto, ProcurementHistory>()
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore()); // Set manually
        }
    }
}