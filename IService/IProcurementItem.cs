using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using procurementsystem.enums;
using procurementsystem.models.ProcurementItem;

namespace procurementsystem.IService
{
    public interface IProcurementItem
    {
        Task<ProcurementItemDto> CreateProcurementItemAsync(CreateProcurementItemDto procurementItemDto);

        Task<ProcurementItemDto> GetProcurementItemByIdAsync(Guid id);

        Task<List<ProcurementItemDto>> GetAllProcurementItemsAsync();

        Task<ProcurementItemDto> UpdateProcurementItemAsync(Guid id, UpdateProcurementItemDto procurementItemDto);

        Task<bool> DeleteProcurementItemAsync(Guid id);

        Task<List<ProcurementItemDto>> GetItemsByStageAndStatusAsync(StageCategory stage, StageStatus status);

        Task<List<ProcurementItemDto>> GetItemsByDepartmentAsync(string department);
    }
}