using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using procurementsystem.enums;

namespace procurementsystem.models.ProcurementItem
{
    public class ProcurementItemDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Department { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime DateRecieved { get; set; }
        public StageCategory Stage { get; set; }
        public StageStatus Status { get; set; }
        public Guid CreatedById { get; set; }
        public string CreatedByName { get; set; } = string.Empty; // Avoid exposing full User object
        public Guid? UpdatedById { get; set; }
        public string UpdatedByName { get; set; } = string.Empty; // Avoid exposing full User object
    }
}