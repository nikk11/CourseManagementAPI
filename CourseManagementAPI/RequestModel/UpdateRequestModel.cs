namespace CourseManagementAPI.RequestModel
{
    public class UpdateRequestModel
    {
        public int CourseId { get; set; }

        public string CourseName { get; set; }

        public int Duration { get; set; }

        public decimal? CourseCost { get; set; }
    }
}
