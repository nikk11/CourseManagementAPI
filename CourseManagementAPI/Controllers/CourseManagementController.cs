using CourseManagementAPI.Interfaces;
using CourseManagementAPI.RequestModel;
using Microsoft.AspNetCore.Mvc;
using SharedDTO.DTO;

namespace CourseManagementAPI.Controllers
{
    [Route("api")]
    [ApiController]
    public class CourseManagementController : ControllerBase
    {
        ICourseManagement _courseManagement;
        public CourseManagementController(ICourseManagement courseManagement)
        {
            _courseManagement = courseManagement;
        }

        [HttpGet]
        [Route("GetAllCourses")]
        public async Task<IActionResult> GetAllCourses()
        {
            var result = await _courseManagement.GetAllCourses();

            return Ok(result);
        }

        [HttpPost]
        [Route("AddCourse")]
        public async Task<IActionResult> AddCourse([FromBody]CourseRequestModel courseModel)
        {
            Course course = new Course
            {
                CourseName = courseModel.CourseName,
                Duration = courseModel.Duration,
                CourseCost = courseModel.CourseCost
            };

            var result = await _courseManagement.AddCourse(course);

            return Ok(result);
        }

        [HttpPut]
        [Route("UpdateCourse")]
        public async Task<IActionResult> UpdateCourse(UpdateRequestModel courseModel)
        {

            Course course = new Course
            {
                CourseId = courseModel.CourseId,
                CourseName = courseModel.CourseName,
                Duration = courseModel.Duration,
                CourseCost = courseModel.CourseCost
            };

            var result = await _courseManagement.UpdateCourse(course);

            return Ok(result);
        }
    }
}
