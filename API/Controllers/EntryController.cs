using API.Data;
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
    }
}