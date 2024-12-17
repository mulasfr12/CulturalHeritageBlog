using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebAPI.DTOs;
using WebAPI.Services.Interfaces;

namespace WebAPI.Controllers
{
    [Authorize(Roles = "Admin")]
    [ApiController]
    [Route("api/[controller]")]
    public class NationalMinorityController : ControllerBase
    {
        private readonly INationalMinorityService _nationalMinorityService;

        public NationalMinorityController(INationalMinorityService nationalMinorityService)
        {
            _nationalMinorityService = nationalMinorityService;
        }

        // GET: api/NationalMinority
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var minorities = await _nationalMinorityService.GetAllNationalMinorities();
            return Ok(minorities);
        }

        // GET: api/NationalMinority/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var minority = await _nationalMinorityService.GetNationalMinorityById(id);
            if (minority == null)
            {
                return NotFound(new { message = "National Minority not found." });
            }

            return Ok(minority);
        }

        // POST: api/NationalMinority
        [HttpPost]
        public async Task<IActionResult> Create(NationalMinorityDto minorityDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var id = await _nationalMinorityService.CreateNationalMinority(minorityDto);
            return CreatedAtAction(nameof(GetById), new { id }, minorityDto);
        }

        // PUT: api/NationalMinority/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, NationalMinorityDto minorityDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var updated = await _nationalMinorityService.UpdateNationalMinority(id, minorityDto);
            if (!updated)
            {
                return NotFound(new { message = "National Minority not found." });
            }

            return NoContent();
        }

        // DELETE: api/NationalMinority/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var deleted = await _nationalMinorityService.DeleteNationalMinority(id);
                if (!deleted)
                    return NotFound(new { message = "National Minority not found." });

                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

    }
}
