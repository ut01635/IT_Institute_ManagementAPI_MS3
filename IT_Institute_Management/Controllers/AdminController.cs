﻿using IT_Institute_Management.DTO.RequestDTO;
using IT_Institute_Management.IServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IT_Institute_Management.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly IAdminService _adminService;
        public AdminController(IAdminService adminService) {
            _adminService = adminService;
        }


        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var admins = await _adminService.GetAllAsync();
                return Ok(admins);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.Message });
            }
        }



        [HttpGet("{nic}")]
        public async Task<IActionResult> GetById(string nic)
        {
            try
            {
                var admin = await _adminService.GetByIdAsync(nic);
                return Ok(admin);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.Message });
            }
        }


        [HttpPost]
        public async Task<IActionResult> Add([FromBody] AdminRequestDto adminDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);  
            }

            try
            {
                await _adminService.AddAsync(adminDto);
                return CreatedAtAction(nameof(GetById), new { nic = adminDto.NIC }, adminDto); 
            }
            catch (ApplicationException ex)
            {
               
                return BadRequest(new { message = ex.Message });
            }
            catch (DbUpdateException ex)
            {
                
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = $"Database error: {ex.Message}" });
            }
            catch (Exception ex)
            {
                
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = $"Unexpected error: {ex.Message}" });
            }
        }


        [HttpPut("{nic}")]
        public async Task<IActionResult> UpdateAdmin(string nic, [FromBody] AdminRequestDto adminDto)
        {
            try
            {
                
                if (nic != adminDto.NIC)
                {
                    return BadRequest(new { message = "NIC in URL and body must be the same." });
                }

                
                await _adminService.UpdateAsync(adminDto);
                return Ok(new { message = "Admin Successfully Updated." });
            }
            catch (KeyNotFoundException ex)
            {
               
                return NotFound(new { message = ex.Message });
            }
            catch (ApplicationException ex)
            {
               
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.Message });
            }
            catch (Exception ex)
            {
                
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An unexpected error occurred while updating the admin." });
            }
        }



        [HttpDelete("{nic}")]

        public async Task<IActionResult> Delete([FromRoute] string nic)
        {
            try
            {
                await _adminService.DeleteAsync(nic);
                return Ok(new { message = "Admin Successfully Deleted." });
            }
            catch (ApplicationException ex)
            {
                return BadRequest(new { message = ex.Message }); 
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = $"An error occurred: {ex.Message}" }); 
            }
        }



    }
}
