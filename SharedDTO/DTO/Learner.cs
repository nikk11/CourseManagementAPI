using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedDTO.DTO
{
    public class Learner
    {
        public int LearnerId { get; set; }

        public string LearnerName { get; set; } = null!;

        public int? LearnerAge { get; set; }

        public string? LearnerContact { get; set; }

        public string? LearnerEmail { get; set; }
    }
}
