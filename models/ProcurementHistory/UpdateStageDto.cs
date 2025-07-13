using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using procurementsystem.enums;

namespace procurementsystem.models.ProcurementHistory
{
    public class UpdateStageDto
    {
        public StageCategory? Stage { get; set; }
        public StageStatus? Status { get; set; }
        public string? Comments { get; set; } = string.Empty;
        public Guid UpdatedById { get; set; }
    }

}