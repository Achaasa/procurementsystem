using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using procurementsystem.enums;

namespace procurementsystem.Entities
{
    public class ProcurementItem
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Department { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime DateRecieved { get; set; }
        public StageCategory Stage { get; set; } = StageCategory.INVITATION;
        public StageStatus Status { get; set; } = StageStatus.PENDING;


        // Added properties
        public Guid CreatedById { get; set; }
        public User CreatedBy { get; set; }= new User(); // Navigation property to User who created this item

        public Guid? UpdatedById { get; set; }
        public User? UpdatedBy { get; set; }
        public bool delFlag { get; set; } = false;
    }
}
