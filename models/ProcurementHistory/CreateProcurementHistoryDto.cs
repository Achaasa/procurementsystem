using System;
using procurementsystem.enums;
namespace procurementsystem.models.ProcurementHistory
{
    public class CreateProcurementHistoryDto
    {
        public Guid ProcurementItemId { get; set; }
        public StageCategory Stage { get; set; }
        public StageStatus Status { get; set; }
        public Guid UpdatedById { get; set; }
        public string? Comments { get; set; }
    }
}