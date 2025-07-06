using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using procurementsystem.enums;

namespace procurementsystem.Entities
{
    public class ProcurementHistory
    {
        public Guid Id { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public StageCategory Stage { get; set; } = StageCategory.INVITATION;
        public StageStatus Status { get; set; } = StageStatus.PENDING;
        public Guid ProcurementItemId { get; set; } // Link to ProcurementItem
        public ProcurementItem ProcurementItem { get; set; } = new ProcurementItem();
        public string? Comments { get; set; } = string.Empty;
        // Updated properties
        public Guid UpdatedById { get; set; }  // User who updated this history
        public required User UpdatedBy { get; set; }  // Navigation property to User
    }
}
