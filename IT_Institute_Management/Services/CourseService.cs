﻿using IT_Institute_Management.Database;
using IT_Institute_Management.DTO.RequestDTO;
using IT_Institute_Management.DTO.ResponseDTO;
using IT_Institute_Management.EmailSerivice;
using IT_Institute_Management.Entity;
using IT_Institute_Management.IRepositories;
using IT_Institute_Management.IServices;

namespace IT_Institute_Management.Services
{
    public class CourseService : ICourseService
    {
        private readonly ICourseRepository _courseRepository;
        private readonly IStudentRepository _studentRepository;
        private readonly IAnnouncementRepository _announcementRepository;
        private readonly IEmailService _emailService;
        private readonly IHostEnvironment _hostEnvironment;
        private readonly InstituteDbContext _context;

        public CourseService(ICourseRepository courseRepository, IStudentRepository studentRepository, IAnnouncementRepository announcementRepository, IEmailService emailService, IHostEnvironment hostEnvironment,InstituteDbContext context)
        {
            _courseRepository = courseRepository;
            _studentRepository = studentRepository;
            _announcementRepository = announcementRepository;
            _emailService = emailService;
            _hostEnvironment = hostEnvironment;
            _context = context;
        }

        public async Task<IEnumerable<CourseResponseDTO>> GetAllCoursesAsync()
        {
            var courses = await _courseRepository.GetAllCoursesAsync();
            return courses.Select(course => new CourseResponseDTO
            {
                Id = course.Id,
                CourseName = course.CourseName,
                Level = course.Level,
                Duration = course.Duration,
                Fees = course.Fees,
                ImagePaths = course.ImagePaths.Split(',').ToList() // Split the stored comma-separated string into a list
            });
        }


        public async Task<CourseResponseDTO> GetCourseByIdAsync(Guid id)
        {
            var course = await _courseRepository.GetCourseByIdAsync(id);
            if (course == null)
                throw new KeyNotFoundException("Course not found.");

            return new CourseResponseDTO
            {
                Id = course.Id,
                CourseName = course.CourseName,
                Level = course.Level,
                Duration = course.Duration,
                Fees = course.Fees,
                ImagePath = course.ImagePath
            };
        }


        public async Task CreateCourseAsync(CourseRequestDTO courseRequest)
        {
            var course = new Course
            {
                CourseName = courseRequest.CourseName,
                Level = courseRequest.Level,
                Duration = courseRequest.Duration,
                Fees = courseRequest.Fees,
                ImagePath = courseRequest.ImagePath
            };

            await _courseRepository.AddCourseAsync(course);

            // Create an Announcement
            var announcement = new Announcement
            {
                Title = $"New Course: {course.CourseName}",
                Body = $"A new course has been added: {course.CourseName}. Level: {course.Level}, Duration: {course.Duration} months, Fees: {course.Fees}.",
                Date = DateTime.UtcNow
            };
            await _announcementRepository.AddAsync(announcement);

            // Send Email to All Students
            var students = await _studentRepository.GetAllAsync();
            foreach (var student in students)
            {
                var body = $"Dear {student.FirstName} {student.LastName},\n\n" +
                           $"We are pleased to inform you that a new course has been added:\n" +
                           $"Course Name: {course.CourseName}\n" +
                           $"Level: {course.Level}\n" +
                           $"Duration: {course.Duration} months\n" +
                           $"Fees: {course.Fees}\n\n" +
                           "Best Regards,\nIT Institute Management";
                await _emailService.SendEmailAsync(student.Email, "New Course Available", body);
            }
        }



        public async Task UpdateCourseAsync(Guid id, CourseRequestDTO courseRequest)
        {
            var course = await _courseRepository.GetCourseByIdAsync(id);
            if (course == null)
                throw new KeyNotFoundException("Course not found.");

            course.CourseName = courseRequest.CourseName;
            course.Level = courseRequest.Level;
            course.Duration = courseRequest.Duration;
            course.Fees = courseRequest.Fees;
            course.ImagePath = courseRequest.ImagePath;

            await _courseRepository.UpdateCourseAsync(course);

            // Create an Announcement for the update
            var announcement = new Announcement
            {
                Title = $"Updated Course: {course.CourseName}",
                Body = $"The course has been updated: {course.CourseName}. Level: {course.Level}, Duration: {course.Duration} months, Fees: {course.Fees}.",
                Date = DateTime.UtcNow
            };
            await _announcementRepository.AddAsync(announcement);

            // Send Email to All Students about the update
            var students = await _studentRepository.GetAllAsync();
            foreach (var student in students)
            {
                var body = $"Dear {student.FirstName} {student.LastName},\n\n" +
                           $"The following course has been updated:\n" +
                           $"Course Name: {course.CourseName}\n" +
                           $"Level: {course.Level}\n" +
                           $"Duration: {course.Duration} months\n" +
                           $"Fees: {course.Fees}\n\n" +
                           "Best Regards,\nIT Institute Management";
                await _emailService.SendEmailAsync(student.Email, "Course Update", body);
            }
        }

        public async Task DeleteCourseAsync(Guid id)
        {
            var courseExists = await _courseRepository.CourseExistsAsync(id);
            if (!courseExists)
                throw new KeyNotFoundException("Course not found.");

            await _courseRepository.DeleteCourseAsync(id);

            // Create an Announcement for course deletion
            var announcement = new Announcement
            {
                Title = "Course Deleted",
                Body = "A course has been deleted. Please check the course list for updates.",
                Date = DateTime.UtcNow
            };
            await _announcementRepository.AddAsync(announcement);

            // Send Email to All Students about the deletion
            var students = await _studentRepository.GetAllAsync();
            foreach (var student in students)
            {
                var body = $"Dear {student.FirstName} {student.LastName},\n\n" +
                           $"We regret to inform you that a course has been deleted from the system.\n\n" +
                           "Best Regards,\nIT Institute Management";
                await _emailService.SendEmailAsync(student.Email, "Course Deleted", body);
            }
        }


    }
}
