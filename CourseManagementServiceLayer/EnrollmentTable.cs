using System;
using System.Collections.Generic;

namespace CourseManagementServiceLayer;

public partial class EnrollmentTable
{
    public int EnrollmentId { get; set; }

    public int LearnerId { get; set; }

    public int CourseId { get; set; }

    public DateTime? EnrollmentDate { get; set; }

    public virtual CourseTable Course { get; set; } = null!;

    public virtual LearnerTable Learner { get; set; } = null!;
}
