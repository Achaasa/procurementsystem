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

        public async Task<List<UpdateStageDto>> ChangeStageAsync(Guid procurementItemId, UpdateStageDto updateStageDto)
        {
            var procurementItem = await _context.ProcurementItems.FindAsync(procurementItemId);
            if (procurementItem == null)
            {
                throw new Exception("Procurement item not found");
            }

            // Validate stage progression
            var currentStage = procurementItem.Stage;
            var newStage = updateStageDto.Stage ?? currentStage; // Use current stage if null
            var newStatus = updateStageDto.Status ?? procurementItem.Status; // Use current status if null

            // Check if the stage change is valid (forward-only progression)
            if (!IsValidStageTransition(currentStage, newStage))
            {
                throw new InvalidOperationException($"Invalid stage transition from {currentStage} to {newStage}. Only forward progression is allowed.");
            }

            // Update the procurement item
            procurementItem.Stage = newStage;
            procurementItem.Status = newStatus;
            procurementItem.UpdatedAt = DateTime.UtcNow;
            procurementItem.UpdatedById = updateStageDto.UpdatedById;

            // Create procurement history record using AutoMapper
            var createHistoryDto = new CreateProcurementHistoryDto
            {
                ProcurementItemId = procurementItemId,
                Stage = newStage,
                Status = newStatus,
                UpdatedById = updateStageDto.UpdatedById,
                Comments = updateStageDto.Comments ?? $"Stage changed from {currentStage} to {newStage}"
            };

            var procurementHistory = _mapper.Map<ProcurementHistory>(createHistoryDto);


            _context.ProcurementHistories.Add(procurementHistory);
            _context.ProcurementItems.Update(procurementItem);
            await _context.SaveChangesAsync();

            // Return the updated procurement item
            var updatedProcurementItem = _mapper.Map<ProcurementItemDto>(procurementItem);
            return new List<UpdateStageDto> { updateStageDto };
        }

        public async Task<ProcurementItemDto> CreateProcurementItemAsync(CreateProcurementItemDto procurementItemDto)
        {

            var procurementItem = _mapper.Map<ProcurementItem>(procurementItemDto);

            _context.ProcurementItems.Add(procurementItem);
            procurementItem.DateRecieved = DateTime.SpecifyKind(procurementItem.DateRecieved, DateTimeKind.Utc);
            procurementItem.CreatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            var procurementItemDtos = _mapper.Map<ProcurementItemDto>(procurementItem);

            return procurementItemDtos;

        }


        public async Task<bool> DeleteProcurementItemAsync(Guid id)
        {
            var procurementItem = await _context.ProcurementItems.FindAsync(id);
            if (procurementItem == null)
            {
                return false;
            }
            procurementItem.delFlag = true;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<ProcurementItemDto>> GetAllProcurementItemsAsync()
        {
            var procurementItems = await _context.ProcurementItems.Where(p => p.delFlag == false).ToListAsync();
            var procurementItemDtos = _mapper.Map<List<ProcurementItemDto>>(procurementItems);
            return procurementItemDtos;
        }

        public async Task<List<ProcurementItemDto>> GetItemsByDepartmentAsync(string department)
        {
            var procurementItems = await _context.ProcurementItems.Where(p => p.delFlag == false && p.Department == department).ToListAsync();
            var procurementItemDtos = _mapper.Map<List<ProcurementItemDto>>(procurementItems);
            return procurementItemDtos;
        }

        public async Task<List<ProcurementItemDto>> GetItemsByStageAndStatusAsync(StageCategory stage, StageStatus status)
        {
            var procurementItems = await _context.ProcurementItems.Where(p => p.delFlag == false && p.Stage == stage && p.Status == status).ToListAsync();
            var procurementItemDtos = _mapper.Map<List<ProcurementItemDto>>(procurementItems);
            return procurementItemDtos;
        }

        public async Task<ProcurementItemDto> GetProcurementItemByIdAsync(Guid id)
        {
            var procurementItem = await _context.ProcurementItems.FindAsync(id);
            if (procurementItem == null)
            {
                throw new Exception("Procurement item not found");
            }
            var procurementItemDto = _mapper.Map<ProcurementItemDto>(procurementItem);
            return procurementItemDto;
        }

        public async Task<List<ProcurementHistoryDto>> GetProcurmentHistoryAsync(Guid procurementItemId)
        {
            var procurementItem = await _context.ProcurementItems.FindAsync(procurementItemId);
            if (procurementItem == null)
            {
                throw new Exception("Procurement item not found");
            }

            var procuementHistory = await _context.ProcurementHistories.Where(ph => ph.ProcurementItemId == procurementItemId && ph.ProcurementItem.delFlag == false).ToListAsync();
            var ProcurementHistoryDtos = _mapper.Map<List<ProcurementHistoryDto>>(procuementHistory);
            return ProcurementHistoryDtos;


        }

        public async Task<ProcurementItemDto> UpdateProcurementItemAsync(Guid id, UpdateProcurementItemDto procurementItemDto)
        {
            var procurementItem = await _context.ProcurementItems.FindAsync(id);
            if (procurementItem == null)
            {
                throw new Exception("Procurement item not found");
            }

            // Update only allowed fields (excluding Stage and Status)
            if (!string.IsNullOrWhiteSpace(procurementItemDto.Name))
                procurementItem.Name = procurementItemDto.Name;

            if (!string.IsNullOrWhiteSpace(procurementItemDto.Description))
                procurementItem.Description = procurementItemDto.Description;

            if (!string.IsNullOrWhiteSpace(procurementItemDto.Department))
                procurementItem.Department = procurementItemDto.Department;


            // Update DateRecieved if provided
            if (procurementItemDto.DateRecieved.HasValue)
                procurementItem.DateRecieved = DateTime.SpecifyKind(procurementItemDto.DateRecieved.Value, DateTimeKind.Utc);

            // Update timestamps
            procurementItem.UpdatedAt = DateTime.UtcNow;

            // Update the user who made the change (if provided)
            if (procurementItemDto.UpdatedById != default)
                procurementItem.UpdatedById = procurementItemDto.UpdatedById;

            _context.ProcurementItems.Update(procurementItem);
            await _context.SaveChangesAsync();

            var updatedProcurementItemDto = _mapper.Map<ProcurementItemDto>(procurementItem);
            return updatedProcurementItemDto;
        }
        private bool IsValidStageTransition(StageCategory currentStage, StageCategory newStage)
        {
            // Define forward-only stage progression
            var stageOrder = new[]
            {
        StageCategory.INVITATION,
        StageCategory.EVALUATION,
        StageCategory.APPROVAL,
        StageCategory.CONTRACT_AWARD,
        StageCategory.DELIVERY,
        StageCategory.PAYMENT
    };

            var currentIndex = Array.IndexOf(stageOrder, currentStage);
            var newIndex = Array.IndexOf(stageOrder, newStage);

            // Can only move forward or stay in the same stage
            return newIndex >= currentIndex;
        }
    }
}
