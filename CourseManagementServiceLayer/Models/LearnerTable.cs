using System;
using System.Collections.Generic;

namespace CourseManagementServiceLayer.Models;

public partial class LearnerTable
{
    public int LearnerId { get; set; }

    public string LearnerName { get; set; } = null!;

    public int? LearnerAge { get; set; }

    public string? LearnerContact { get; set; }

    public string? LearnerEmail { get; set; }

    public virtual ICollection<EnrollmentTable> EnrollmentTables { get; set; } = new List<EnrollmentTable>();
}
