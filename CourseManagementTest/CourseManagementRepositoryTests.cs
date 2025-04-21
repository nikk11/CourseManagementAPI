using AutoMapper;
using CourseManagementAPI.Repository;
using CourseManagementServiceLayer.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using SharedDTO.DTO;
using SharedDTO.ExceptionBase;
using StackExchange.Redis;

namespace CourseManagementTest
{
    [TestFixture]
    public class CourseManagementRepositoryTests
    {
        private CoursemanagementsqldbngContext _context;
        private CourseManagementRepository _repository;
        private Mock<DbSet<CourseTable>> _courseTableMock;
        private Mock<IMapper> _mapperMock;
        private Mock<ILogger<CourseManagementRepository>> _loggerMock;
        private Mock<IConnectionMultiplexer> _redisMock;
        private Mock<IDatabase> _cacheMock;
        private Mock<CoursemanagementsqldbngContext> _dbContextMock;

        [TearDown]
        public void TearDown()
        {
            _context.Dispose();
        }

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<CoursemanagementsqldbngContext>()
         .UseInMemoryDatabase(Guid.NewGuid().ToString()) // unique DB for each test
         .Options;

            _context = new CoursemanagementsqldbngContext(options);

            // Mocks
            _dbContextMock = new Mock<CoursemanagementsqldbngContext>();
            _courseTableMock = new Mock<DbSet<CourseTable>>();
            _mapperMock = new Mock<IMapper>();
            _loggerMock = new Mock<ILogger<CourseManagementRepository>>();
            _redisMock = new Mock<IConnectionMultiplexer>();
            _cacheMock = new Mock<IDatabase>();

            _redisMock.Setup(x => x.GetDatabase(It.IsAny<int>(), It.IsAny<object>()))
                      .Returns(_cacheMock.Object);

            _repository = new CourseManagementRepository(_context, _mapperMock.Object, _loggerMock.Object, _redisMock.Object);
        }

        [Test]
        public async Task GetAllCourses_WhenCoursesExist_ReturnsMappedCourses()
        {
            // Arrange
            _context.CourseTables.Add(new CourseTable
            {
                CourseId = 1,
                CourseName = "Test Course",
                CourseCost = 200,
                CourseDuration = DateTime.Now
            });
            _context.SaveChanges();

            _mapperMock.Setup(m => m.Map<IEnumerable<Course>>(It.IsAny<IEnumerable<CourseTable>>()))
                .Returns(new List<Course> { new Course { CourseId = 1, CourseName = "Test Course" } });

            _cacheMock.Setup(x => x.StringGetAsync(It.IsAny<RedisKey>(), It.IsAny<CommandFlags>()))
                .ReturnsAsync(RedisValue.Null);

            // Act
            var result = await _repository.GetAllCourses();

            // Assert
            Assert.IsNotNull(result);
            Assert.That(result.Count(), Is.EqualTo(1));
        }

        [Test]
        public void GetAllCourses_WhenNoCourses_ThrowsNotFoundException()
        {
            // Arrange
            _cacheMock.Setup(x => x.StringGetAsync(It.IsAny<RedisKey>(), It.IsAny<CommandFlags>()))
                .ReturnsAsync(RedisValue.Null);

            // Act & Assert
            var ex = Assert.ThrowsAsync<NotFoundException>(async () => await _repository.GetAllCourses());
            Assert.That(ex.Message, Is.EqualTo("No courses found"));
        }

        [Test]
        public async Task AddCourse_ValidCourse_ReturnsTrue()
        {
            // Arrange
            var course = new Course
            {
                CourseName = "Test Course",
                CourseDuration = DateTime.Now,
                CourseCost = 1000
            };

            var mockSet = new Mock<DbSet<CourseTable>>();
            _dbContextMock.Setup(x => x.CourseTables).Returns(mockSet.Object);
            _dbContextMock.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
                          .ReturnsAsync(1);

            // Act
            var result = await _repository.AddCourse(course);

            // Assert
            Assert.IsTrue(result);
        }

        [Test]
        public void UpdateCourse_CourseNotFound_ThrowsBadRequestException()
        {
            // Arrange
            var course = new Course { CourseId = 99 };

            _dbContextMock.Setup(x => x.CourseTables.FindAsync(course.CourseId))
                          .ReturnsAsync((CourseTable)null!);

            // Act & Assert
            var ex = Assert.ThrowsAsync<BadRequestException>(() => _repository.UpdateCourse(course));
            Assert.That(ex!.Message, Does.Contain("Course with ID 99 not found."));
        }

        [Test]
        public void UpdateCourse_DbError_ThrowsApplicationException()
        {
            // Arrange
            var course = new Course { CourseId = 1 };

            _dbContextMock.Setup(x => x.CourseTables.FindAsync(course.CourseId))
                          .ThrowsAsync(new Exception("Could not update course."));

            // Act & Assert
            var ex = Assert.ThrowsAsync<BadRequestException>(() => _repository.UpdateCourse(course));
        }
    }
}
