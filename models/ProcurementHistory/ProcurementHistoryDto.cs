using System;
using procurementsystem.enums;

namespace procurementsystem.models.ProcurementHistory
{
    public class ProcurementHistoryDto
    {
        public Guid Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public StageCategory Stage { get; set; }
        public StageStatus Status { get; set; }
        public Guid ProcurementItemId { get; set; }
        public string? Comments { get; set; }
        public Guid UpdatedById { get; set; }
       
    }
} 