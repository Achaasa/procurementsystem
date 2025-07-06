using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using procurementsystem.enums;

namespace procurementsystem.models.ProcurementItem
{
   public class UpdateProcurementItemDto
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? Department { get; set; }
        public DateTime? DateRecieved { get; set; }
        public StageCategory? Stage { get; set; }
        public StageStatus? Status { get; set; }
        public Guid? UpdatedById { get; set; }
    }
}