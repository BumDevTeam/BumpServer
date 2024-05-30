using API.Data;
using API.DTOs;
using API.Entities;
using System;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
     [ApiController]
     [Route("api/[controller]")]
     public class EntryController : ControllerBase
     {
          private readonly DataContext _context;
          private readonly IMapper _mapper;
          public EntryController(DataContext context, IMapper mapper)
          {
               _context = context;
               _mapper = mapper;
          }

          [HttpGet]
          public async Task<ActionResult<IEnumerable<EntryDTO>>> GetEntries()
          {
               return await _context.Entries.ProjectTo<EntryDTO>(_mapper.ConfigurationProvider).ToListAsync();
          }

          [HttpPost("add")]
          public async Task<ActionResult<EntryDTO>> AddEntry(EntryDTO entry)
          {
               var entries = await _context.Entries.ToListAsync();
               var value = Math.Round(entry.Value,5);

               foreach(Entry e in entries)
               {
                    if(Math.Round(e.Value,5) == value)
                    {
                         return Ok("Location already in database!");
                    }
               }
               
               _context.Entries.Add(new Entry
               {
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

               return await _context.Entries.
                    Where(e => e.Latitude > lowerLatitude && e.Latitude < upperLatitude
                     && e.Longitude > lowerLongitude && e.Longitude < upperLongitude)
                     .ProjectTo<EntryDTO>(_mapper.ConfigurationProvider).ToListAsync();
          }

          [HttpDelete("delete-entry/{entryId}")]
          public async Task<ActionResult> DeleteEntry(int entryId)
          {
               _context.Entries.Remove(new Entry(){Id = entryId});

                if (await _context.SaveChangesAsync() > 0)
               {
                    return Ok("Entry removed");
               }
               else return BadRequest("Failed to remove entry");
          }
     }
}