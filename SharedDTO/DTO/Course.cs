using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedDTO.DTO
{
    public class Course
    {
        public int CourseId { get; set; }

        public string CourseName { get; set; }

        public decimal? CourseCost { get; set; }

        public int? Duration { get; set; }

    }
}
