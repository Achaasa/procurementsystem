using AutoMapper;
using procurementsystem.Entities;
using procurementsystem.models.ProcurementItem;

namespace procurementsystem.Automapper
{
    public class ProcurementItemProfile : Profile
    {
        public ProcurementItemProfile()
        {
            // Mapping CreateProcurementItemDto to ProcurementItem
            CreateMap<CreateProcurementItemDto, ProcurementItem>()
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.Stage, opt => opt.MapFrom(src => src.Stage))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status));

            // Mapping UpdateProcurementItemDto to ProcurementItem
            CreateMap<UpdateProcurementItemDto, ProcurementItem>()
                .ForMember(dest => dest.Name, opt => opt.Condition(src => src.Name != null)) // Only map if Name is not null
                .ForMember(dest => dest.Description, opt => opt.Condition(src => src.Description != null)) // Only map if Description is not null
                .ForMember(dest => dest.Department, opt => opt.Condition(src => src.Department != null)) // Only map if Department is not null
                .ForMember(dest => dest.DateRecieved, opt => opt.Condition(src => src.DateRecieved != null)) // Only map if DateRecieved is not null
                .ForMember(dest => dest.Stage, opt => opt.Condition(src => src.Stage.HasValue)) // Only map if Stage is not null
                .ForMember(dest => dest.Status, opt => opt.Condition(src => src.Status.HasValue)) // Only map if Status is not null
                .ForMember(dest => dest.UpdatedById, opt => opt.MapFrom(src => src.UpdatedById))
                .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore()); // Assuming UpdatedBy is set elsewhere

                 // Mapping from ProcurementItem entity to ProcurementItemDto (for front-end)
            CreateMap<ProcurementItem, ProcurementItemDto>()
                .ForMember(dest => dest.CreatedByName, opt => opt.MapFrom(src => src.CreatedBy.Name)) // Get the Name of CreatedBy
                .ForMember(dest => dest.UpdatedByName, opt => opt.MapFrom(src => src.UpdatedBy != null ? src.UpdatedBy.Name : string.Empty)) // If UpdatedBy is null, set to empty string
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status)) // Direct mapping
                .ForMember(dest => dest.Stage, opt => opt.MapFrom(src => src.Stage)); // Direct mapping
        }
    }
}
