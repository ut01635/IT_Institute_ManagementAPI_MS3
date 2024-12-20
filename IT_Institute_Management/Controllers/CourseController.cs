﻿using IT_Institute_Management.DTO.RequestDTO;
using IT_Institute_Management.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace IT_Institute_Management.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CourseController : ControllerBase
    {
        private readonly ICourseService _courseService;

        public CourseController(ICourseService courseService)
        {
            _courseService = courseService;
        }


        [HttpGet]
        public async Task<IActionResult> GetAllCourses()
        {
            try
            {
                var courses = await _courseService.GetAllCoursesAsync();
                return Ok(courses);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> GetCourseById(Guid id)
        {
            try
            {
                var course = await _courseService.GetCourseByIdAsync(id);
                return Ok(course);
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { message = "Course not found." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }


        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateCourse(
                   [FromForm] CourseRequestDTO courseRequest,
                   [FromForm] List<IFormFile> images)
        {
            try
            {
                
                await _courseService.CreateCourseAsync(courseRequest, images);
                return CreatedAtAction(nameof(GetCourseById), new { id = courseRequest.CourseName }, courseRequest);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }


        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateCourse(
                   Guid id,
                   [FromForm] CourseRequestDTO courseRequest,
                   [FromForm] List<IFormFile> images)
        {
            try
            {
               
                await _courseService.UpdateCourseAsync(id, courseRequest, images);
                return Ok("Course updated successfully");
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { message = "Course not found." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }



        
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteCourse(Guid id)
        {
            try
            {
                await _courseService.DeleteCourseAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { message = "Course not found." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
