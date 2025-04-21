using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedDTO.DTO
{
    public class Enrollment
    {
        public int EnrollmentId { get; set; }

        public int LearnerId { get; set; }

        public int CourseId { get; set; }

        public DateTime? EnrollmentDate { get; set; }

    }
}
