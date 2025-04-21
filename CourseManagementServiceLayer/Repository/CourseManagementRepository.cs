using AutoMapper;
using CourseManagementAPI.Interfaces;
using CourseManagementServiceLayer.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SharedDTO.DTO;
using SharedDTO.ExceptionBase;
using StackExchange.Redis;
using System.Text.Json;

namespace CourseManagementAPI.Repository
{
    public class CourseManagementRepository : ICourseManagement
    {
        public readonly CoursemanagementsqldbngContext _dbContext;

        private readonly ILogger<CourseManagementRepository> _logger;

        private readonly IConnectionMultiplexer _redis;

        private readonly IDatabase _cache;

        public CourseManagementRepository(CoursemanagementsqldbngContext dbContext, IMapper mapper, ILogger<CourseManagementRepository> logger, IConnectionMultiplexer redis)
        {
            _dbContext = dbContext;
            _logger = logger;
            _redis = redis;
            _cache = _redis.GetDatabase();
        }

        public async Task<IEnumerable<Course>> GetAllCourses()
        {
            try
            {
                _logger.LogInformation("GetAllCourses Fetching All Courses.....");

                string cacheKey = "GetAllData";

                // Check if data exists in Redis cache
                var cachedData = await _cache.StringGetAsync(cacheKey);
                var testCourseTable = new CourseTable { CourseId = 1, CourseName = "Test", CourseCost = 100, Duration = 4 };
                // return the cached data
                if (!cachedData.IsNullOrEmpty)
                {
                    _logger.LogInformation("GetAllCourses: Data retrieved from cache.");
                    var cachedCourses = JsonSerializer.Deserialize<List<CourseTable>>(cachedData!);
                    return cachedCourses!.Select(course => new Course
                    {
                        CourseId = course.CourseId,
                        CourseName = course.CourseName,
                        CourseCost = course.CourseCost,
                        Duration = course.Duration
                    }).ToList();
                }

                var allCourses = await _dbContext.CourseTables.ToListAsync();

                if (!allCourses.Any())
                {
                    _logger.LogWarning("GetAllCourses No Course Found.....");
                    throw new NotFoundException("No courses found");
                }

                await _cache.StringSetAsync(cacheKey, JsonSerializer.Serialize(allCourses), TimeSpan.FromSeconds(0.1));

                return allCourses.Select(course => new Course
                {
                    CourseId = course.CourseId,
                    CourseName = course.CourseName,
                    CourseCost = course.CourseCost,
                    Duration = course.Duration
                }).ToList();
            }
            catch (NotFoundException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error Fetching Course.");
                throw new ApplicationException("Could not fetch courses.", ex);
            }

        }

        public async Task<bool> AddCourse(Course course)
        {
            try
            {
                _logger.LogInformation("AddCourses Adding Course.....");

                var entity = new CourseTable
                {
                    CourseName = course.CourseName,
                    Duration = course.Duration,
                    CourseCost = course.CourseCost
                };

                _dbContext.CourseTables.Add(entity);
                await _dbContext.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding course.");
                throw new ApplicationException("Could not add course.", ex);
            }
        }

        public async Task<Course> UpdateCourse(Course course)
        {
            try
            {
                _logger.LogInformation("UpdateCourses Updating Course.....");

                var existing = await _dbContext.CourseTables.FindAsync(course.CourseId);

                if (existing == null)
                {
                    _logger.LogWarning("Course with ID {CourseId} not found.", course.CourseId);
                    throw new BadRequestException($"Course with ID {course.CourseId} not found.");
                }

                existing.CourseName = course.CourseName;
                existing.Duration = course.Duration;
                existing.CourseCost = course.CourseCost;

                await _dbContext.SaveChangesAsync();

                return course;
            }
            catch (BadRequestException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating course with ID {CourseId}", course.CourseId);
                throw new ApplicationException("Could not update course.", ex);
            }
        }


    }
}
