using System;
using System.Collections.Generic;

namespace CourseManagementServiceLayer;

public partial class CourseTable
{
    public int CourseId { get; set; }

    public string CourseName { get; set; } = null!;

    public decimal? CourseCost { get; set; }

    public int? Duration { get; set; }

    public virtual ICollection<EnrollmentTable> EnrollmentTables { get; set; } = new List<EnrollmentTable>();
}
