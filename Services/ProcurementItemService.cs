using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using procurementsystem.Entities;

using procurementsystem.IService;
using Microsoft.EntityFrameworkCore;
using procurementsystem.Data;
using procurementsystem.models.ProcurementItem;
using procurementsystem.enums;

namespace procurementsystem.Services
{
    public class ProcurementItemService : IProcurementItem
    {
        private readonly ApplicationDBContext _context;

        public ProcurementItemService(ApplicationDBContext context)
        {
            _context = context;
        }

        public async Task<ProcurementItemDto> CreateProcurementItemAsync(CreateProcurementItemDto procurementItemDto)
        {
            var procurementItem = new ProcurementItem
            {
                Id = Guid.NewGuid(),
                Name = procurementItemDto.Name,
                Description = procurementItemDto.Description,
                Department = procurementItemDto.Department,
                DateRecieved = procurementItemDto.DateRecieved,
                Stage = procurementItemDto.Stage,
                Status = procurementItemDto.Status,
                CreatedById = procurementItemDto.CreatedById,
                CreatedAt = DateTime.UtcNow
            };

            _context.ProcurementItems.Add(procurementItem);
            await _context.SaveChangesAsync();

            return new ProcurementItemDto
            {
                Id = procurementItem.Id,
                Name = procurementItem.Name,
                Description = procurementItem.Description,
                Department = procurementItem.Department,
                DateRecieved = procurementItem.DateRecieved,
                Stage = procurementItem.Stage,
                Status = procurementItem.Status,
                CreatedById = procurementItem.CreatedById,
                CreatedByName = procurementItem.CreatedBy.Name // Assuming CreatedBy is a related User entity
            };
        }


        public Task<bool> DeleteProcurementItemAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<List<ProcurementItemDto>> GetAllProcurementItemsAsync()
        {
            throw new NotImplementedException();
        }

        public Task<List<ProcurementItemDto>> GetItemsByDepartmentAsync(string department)
        {
            throw new NotImplementedException();
        }

        public Task<List<ProcurementItemDto>> GetItemsByStageAndStatusAsync(StageCategory stage, StageStatus status)
        {
            throw new NotImplementedException();
        }

        public Task<ProcurementItemDto> GetProcurementItemByIdAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<ProcurementItemDto> UpdateProcurementItemAsync(Guid id, UpdateProcurementItemDto procurementItemDto)
        {
            throw new NotImplementedException();
        }
    }
}
