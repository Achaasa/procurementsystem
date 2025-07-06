using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using procurementsystem.enums;

namespace procurementsystem.models.ProcurementItem
{
     public class CreateProcurementItemDto
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Department { get; set; } = string.Empty;
        public DateTime DateRecieved { get; set; }
        public StageCategory Stage { get; set; } = StageCategory.INVITATION;
        public StageStatus Status { get; set; } = StageStatus.PENDING;
        public Guid CreatedById { get; set; }
    }
}