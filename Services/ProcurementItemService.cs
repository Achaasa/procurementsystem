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
using AutoMapper;
using procurementsystem.models.ProcurementHistory;

namespace procurementsystem.Services
{
    public class ProcurementItemService : IProcurementItem
    {
        private readonly ApplicationDBContext _context;
        private readonly IMapper _mapper;
        public ProcurementItemService(ApplicationDBContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public Task<List<UpdateStageDto>> ChangeStageAsync(Guid procurementItemId, UpdateStageDto updateStageDto)
        {
            throw new NotImplementedException();
        }

        public async Task<ProcurementItemDto> CreateProcurementItemAsync(CreateProcurementItemDto procurementItemDto)
        {
          
            var procurementItem=_mapper.Map<ProcurementItem>(procurementItemDto);

            _context.ProcurementItems.Add(procurementItem);
            procurementItem.DateRecieved = DateTime.SpecifyKind(procurementItem.DateRecieved, DateTimeKind.Utc);
            procurementItem.CreatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            var procurementItemDtos=_mapper.Map<ProcurementItemDto>(procurementItem);

            return procurementItemDtos;
            
        }


        public async Task<bool> DeleteProcurementItemAsync(Guid id)
        {
            var procurementItem=await _context.ProcurementItems.FindAsync(id);
            if(procurementItem==null)
            {
                return false;
            }
            procurementItem.delFlag=true;
            await _context.SaveChangesAsync();
           return true;
        }

        public async Task<List<ProcurementItemDto>> GetAllProcurementItemsAsync()
        {
            var procurementItems=await _context.ProcurementItems.Where(p=>p.delFlag==false).ToListAsync();
            var procurementItemDtos=_mapper.Map<List<ProcurementItemDto>>(procurementItems);
            return procurementItemDtos;
        }

        public async Task<List<ProcurementItemDto>> GetItemsByDepartmentAsync(string department)
        {
            var procurementItems=await _context.ProcurementItems.Where(p=>p.delFlag==false && p.Department==department).ToListAsync();
            var procurementItemDtos=_mapper.Map<List<ProcurementItemDto>>(procurementItems);
            return procurementItemDtos;
        }

        public async Task<List<ProcurementItemDto>> GetItemsByStageAndStatusAsync(StageCategory stage, StageStatus status)
        {
            var procurementItems=await _context.ProcurementItems.Where(p=>p.delFlag==false && p.Stage==stage && p.Status==status).ToListAsync();
            var procurementItemDtos=_mapper.Map<List<ProcurementItemDto>>(procurementItems);
            return procurementItemDtos;
        }

        public async Task<ProcurementItemDto> GetProcurementItemByIdAsync(Guid id)
        {
            var procurementItem=await _context.ProcurementItems.FindAsync(id);
            if(procurementItem==null)
            {
                throw new Exception("Procurement item not found");
            }
            var procurementItemDto=_mapper.Map<ProcurementItemDto>(procurementItem);
            return procurementItemDto;
        }

        public async Task<List<ProcurementHistoryDto>> GetProcurmentHistoryAsync(Guid procurementItemId)
        {
            var procurementItem=await _context.ProcurementItems.FindAsync(procurementItemId);
            if(procurementItem==null)
            {
                throw new Exception("Procurement item not found");
            }

            var procuementHistory= await _context.ProcurementHistories.Where(ph=>ph.ProcurementItemId==procurementItemId && ph.ProcurementItem.delFlag==false).ToListAsync();
           var  ProcurementHistoryDtos= _mapper.Map<List<ProcurementHistoryDto>>(procuementHistory);
           return ProcurementHistoryDtos;

           
        }

        public Task<ProcurementItemDto> UpdateProcurementItemAsync(Guid id, UpdateProcurementItemDto procurementItemDto)
        {
            throw new NotImplementedException();
        }
    }
}
