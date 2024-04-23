using API.Data;
using API.DTOs;
using API.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
     [ApiController]
     [Route("api/[controller]")]
     public class EntryController : ControllerBase
     {
          private readonly DataContext _context;
          public EntryController(DataContext context)
          {
               _context = context;
          }

          [HttpGet]
          public async Task<ActionResult<IEnumerable<Entry>>> GetEntries()
          {
               return await _context.Entries.ToListAsync();
          }

          [HttpPost("add")]
          public async Task<ActionResult<EntryDTO>> AddEntry(EntryDTO entry)
          {
               _context.Entries.Add(new Entry {
                    Longitude = entry.Longitude,
                    Latitude = entry.Latitude,
                    Value = entry.Value
               });

               if (await _context.SaveChangesAsync() > 0)
               {
                   return Ok(":D");
                     
               }
               else return BadRequest("Failed to add entry");

          }

          [HttpPost("getNearby")]
          public async Task<ActionResult<List<EntryDTO>>> GetNearbyEntries(EntryDTO entry)
          {
               var lowerLatitude = entry.Latitude - 0.5;
               var lowerLongitude = entry.Longitude - 0.5;
               var upperLatitude = entry.Latitude + 0.5;
               var upperLongitude = entry.Longitude + 0.5;

               var entries = _context.Entries.AsQueryable();

               entries = entries.Where
               (e => e.Latitude > lowerLatitude && e.Latitude < upperLatitude && e.Longitude > lowerLongitude && e.Longitude< upperLongitude);

               var computedEntries = entries.Select(entry => new EntryDTO
               {
                  Longitude = entry.Longitude,
                  Latitude = entry.Latitude,
                  Value = entry.Value
               });

               return  new List<EntryDTO>(computedEntries);
          }
     }
}