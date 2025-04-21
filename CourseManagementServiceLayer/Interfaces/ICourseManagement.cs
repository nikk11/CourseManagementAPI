using SharedDTO.DTO;

namespace CourseManagementAPI.Interfaces
{
    public interface ICourseManagement
    {
        public Task<IEnumerable<Course>> GetAllCourses();

        public Task<bool> AddCourse(Course course);

        public Task<Course> UpdateCourse(Course course);
    }
}
