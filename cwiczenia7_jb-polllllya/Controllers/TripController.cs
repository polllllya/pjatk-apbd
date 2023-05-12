using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Zadanie7.Models;

namespace Zadanie7.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TripController : ControllerBase
    {
        private readonly MasterContext _context;

        public TripController(MasterContext context)
        {
            _context = context;
        }
        
        [HttpGet]
        public ActionResult<IEnumerable<object>> GetTrips()
        {
            var trips = _context.Trips
                .OrderByDescending(t => t.DateFrom)
                .Include(t => t.IdCountries)
                .Include(t => t.ClientTrips)
                .ThenInclude(ct => ct.IdClientNavigation)
                .Select(t => new
                {
                    t.Name,
                    t.Description,
                    DateFrom = t.DateFrom.ToString("yyyy-MM-dd"),
                    DateTo = t.DateTo.ToString("yyyy-MM-dd"),
                    t.MaxPeople,
                    Countries = t.IdCountries.Select(c => new { c.Name }),
                    Clients = t.ClientTrips.Select(ct => new { ct.IdClientNavigation.FirstName, ct.IdClientNavigation.LastName })
                });

            return Ok(trips.ToList());
        }
        
        [HttpDelete("{id}")]
        public IActionResult DeleteClient(int id)
        {
            var client = _context.Clients
                .Include(c => c.ClientTrips)
                .FirstOrDefault(c => c.IdClient == id);

            if (client == null)
            {
                return NotFound();
            }

            if (client.ClientTrips.Any())
            {
                return BadRequest("Cannot delete client because they have assigned trips.");
            }

            _context.Clients.Remove(client);
            _context.SaveChanges();

            return Ok("The client was successfully deleted");
        }
        
        [HttpPost("/api/trips/{idTrip}/clients")]
        public IActionResult AddClientToTrip(int idTrip, ClientTripDto clientTripDto)
        {
            var client = _context.Clients.SingleOrDefault(c => c.Pesel == clientTripDto.Pesel);
            if (client == null)
            {
                client = new Client
                {
                    FirstName = clientTripDto.FirstName,
                    LastName = clientTripDto.LastName,
                    Email = clientTripDto.Email,
                    Telephone = clientTripDto.Telephone,
                    Pesel = clientTripDto.Pesel
                };

                _context.Clients.Add(client);
                _context.SaveChanges();
            }

            var clientTrip = _context.ClientTrips.SingleOrDefault(ct => ct.IdClient == client.IdClient && ct.IdTrip == idTrip);
            if (clientTrip != null)
            {
                return BadRequest("This client is already registered for this trip");
            }

            var trip = _context.Trips.SingleOrDefault(t => t.IdTrip == idTrip && t.Name == clientTripDto.TripName && t.IdTrip == clientTripDto.IdTrip);
            if (trip == null)
            {
                return NotFound("Trip not found");
            }

            var paymentDate = clientTripDto.PaymentDate != null ? DateTime.Parse(clientTripDto.PaymentDate.ToString()) : (DateTime?)null;

            var registeredAt = DateTime.Now;

            clientTrip = new ClientTrip
            {
                IdClient = client.IdClient,
                IdTrip = idTrip,
                PaymentDate = paymentDate,
                RegisteredAt = registeredAt
            };

            _context.ClientTrips.Add(clientTrip);
            _context.SaveChanges();

            return Ok("Client added to trip successfully");
        }
    }
}