using KolokwiumAPBD1_s31774.Exceptions;
using KolokwiumAPBD1_s31774.Models.DTOs;
using KolokwiumAPBD1_s31774.Services;
using Microsoft.AspNetCore.Mvc;

namespace KolokwiumAPBD1_s31774.Controllers;

    [Microsoft.AspNetCore.Components.Route("api/[controller]")]
    [ApiController]
    public class AppointmentController : ControllerBase
    {
        private readonly IDbService _dbService;
        public AppointmentController(IDbService dbService)
        {
            _dbService = dbService;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetAppointmentById(int AppointmentId)
        {
            try
            {
                var res = await _dbService.GetAppointmentById(AppointmentId);
                return Ok(res);
            }
            catch (NotFoundException e)
            {
                return NotFound(e.Message);
            }
        }

        [HttpPost("{id}")]
        public async Task<IActionResult> AddNewAppointment(int PatientId, RegisterAppointmentDTO createRentalRequest)
        {
            if (!createRentalRequest.Services.Any())
            {
                return BadRequest("At least one item is required.");
            }

            try
            {
                await _dbService.AddNewAppointment(PatientId, createRentalRequest);
            }
            catch (ConflictException e)
            {
                return Conflict(e.Message);
            }
            catch (NotFoundException e)
            {
                return NotFound(e.Message);
            }
            
            return CreatedAtAction(nameof(GetAppointmentById), new { PatientId }, createRentalRequest);
        }    
    }
