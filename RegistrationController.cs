using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceRegistration.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace RaceRegistration.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RegistrationController : ControllerBase
    {
        private readonly RaceDbContext _context;

        public RegistrationController(RaceDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> Register([FromBody] RegistrationRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Sprawdzenie dostępności miejsc
            var currentParticipants = await _context.Registrations
                .CountAsync(r => r.Distance == request.Distance);

            var maxParticipants = GetMaxParticipants(request.Distance);
            if (currentParticipants >= maxParticipants)
                return BadRequest("Przepraszamy, wszystkie miejsca na ten dystans zostały zajęte");

            var registration = new Registration
            {
                UserId = request.UserId,
                FirstName = request.FirstName,
                LastName = request.LastName,
                AgeGroup = request.AgeGroup,
                Gender = request.Gender,
                Distance = request.Distance,
                TshirtOrdered = request.TshirtOrdered,
                RegistrationDate = DateTime.UtcNow
            };

            _context.Registrations.Add(registration);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Rejestracja zakończona pomyślnie", registrationId = registration.Id });
        }

        [HttpGet("stats")]
        public async Task<IActionResult> GetStats()
        {
            var stats = await _context.Registrations
                .GroupBy(r => r.Distance)
                .Select(g => new
                {
                    Distance = g.Key,
                    Count = g.Count(),
                    MaxParticipants = GetMaxParticipants(g.Key)
                })
                .ToListAsync();

            return Ok(stats);
        }

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetUserRegistrations(int userId)
        {
            var registrations = await _context.Registrations
                .Where(r => r.UserId == userId)
                .OrderByDescending(r => r.RegistrationDate)
                .ToListAsync();

            return Ok(registrations);
        }

        private int GetMaxParticipants(string distance)
        {
            return distance switch
            {
                "5km" => 200,
                "10km" => 300,
                "21km" => 250,
                "42km" => 200,
                _ => throw new ArgumentException("Nieprawidłowy dystans")
            };
        }
    }

    public class RegistrationRequest
    {
        public int UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string AgeGroup { get; set; }
        public string Gender { get; set; }
        public string Distance { get; set; }
        public bool TshirtOrdered { get; set; }
    }
} 