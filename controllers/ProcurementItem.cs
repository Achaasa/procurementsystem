using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using procurementsystem.enums;
using procurementsystem.IService;
using procurementsystem.models.ProcurementHistory;
using procurementsystem.models.ProcurementItem;

namespace procurementsystem.controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProcurementItem : ControllerBase
    {
        private readonly IProcurementItem _procurementItemService;
        public ProcurementItem(IProcurementItem procurementItemService)
        {
            _procurementItemService = procurementItemService;
        }
        [Authorize(Roles = "ADMIN,SUPER_ADMIN")]
        [HttpPost("add")]
        public async Task<ActionResult<ProcurementItemDto>> AddProcurementItem([FromBody] CreateProcurementItemDto procurementItemDto)
        {
            var procurementItem = await _procurementItemService.CreateProcurementItemAsync(procurementItemDto);
            return Ok(procurementItem);
        }

        [Authorize(Roles = "ADMIN,SUPER_ADMIN")]
        [HttpPut("delete/{id}")]
        public async Task<ActionResult<bool>> DeleteProcurement([FromBody] Guid id)
        {
            await _procurementItemService.DeleteProcurementItemAsync(id);
            return Ok(new { message = "Successfully deleted" });

        }

        [Authorize(Roles = "ADMIN,SUPER_ADMIN")]
        [HttpGet("all")]
        public async Task<ActionResult<List<ProcurementItemDto>>> GetAll()
        {
            var items = await _procurementItemService.GetAllProcurementItemsAsync();
            return Ok(items);
        }

        [Authorize(Roles = "ADMIN,SUPER_ADMIN")]
        [HttpGet("get/{id}")]
        public async Task<ActionResult<ProcurementItemDto>> GetById(Guid id)
        {

            var item = await _procurementItemService.GetProcurementItemByIdAsync(id);
            return Ok(item);

        }

        [Authorize(Roles = "ADMIN,SUPER_ADMIN")]
        [HttpGet("department/{department}")]
        public async Task<ActionResult<List<ProcurementItemDto>>> GetByDepartment(string department)
        {
            var items = await _procurementItemService.GetItemsByDepartmentAsync(department);
            return Ok(items);
        }

        [Authorize(Roles = "ADMIN,SUPER_ADMIN")]
        [HttpGet("stage-status")]
        public async Task<ActionResult<List<ProcurementItemDto>>> GetByStageAndStatus([FromQuery] StageCategory stage, [FromQuery] StageStatus status)
        {
            var items = await _procurementItemService.GetItemsByStageAndStatusAsync(stage, status);
            return Ok(items);
        }
        [Authorize(Roles = "ADMIN,SUPER_ADMIN")]

        [HttpPut("update/{id}")]
        public async Task<ActionResult<ProcurementItemDto>> Update(Guid id, [FromBody] UpdateProcurementItemDto updateDto)
        {
            var updated = await _procurementItemService.UpdateProcurementItemAsync(id, updateDto);
            if (updated == null)
                return NotFound(new { message = "Item not found or update failed" });
            return Ok(updated);
        }
        [Authorize(Roles = "ADMIN,SUPER_ADMIN")]

        [HttpGet("get/history/{id}")]
        public async Task<ActionResult<List<ProcurementHistoryDto>>> GetHistory(Guid id)
        {


            var history = await _procurementItemService.GetProcurmentHistoryAsync(id);
            return Ok(history);


        }
        [Authorize(Roles = "ADMIN,SUPER_ADMIN")]

        [HttpPut("change-stage/{id}")]
        public async Task<ActionResult<List<UpdateStageDto>>> ChangeStage(Guid id, [FromBody] UpdateStageDto updateStageDto)
        {

            var result = await _procurementItemService.ChangeStageAsync(id, updateStageDto);
            return Ok(result);

        }
    }
}