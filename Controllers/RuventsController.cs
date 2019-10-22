using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ruvents_api.Models;

namespace ruvents_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RuventsController : ControllerBase
    {
        private readonly RuventsContext _context;

        public RuventsController(RuventsContext context)
        {
            _context = context;
        }

        [HttpGet("home/{date}")]
        public async Task<ActionResult<IEnumerable<Ruvent>>> GetRuvents(DateTime date)
        {
            return await _context.Ruvents
                .Where(r => r.Date.Value.Month == date.Month && r.Date.Value.Year == date.Year)
                .OrderBy(r => r.Date)
                .ToListAsync();
        }

        // GET: api/Ruvents/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Ruvent>> GetRuvent(int id)
        {
            var ruvent = await _context.Ruvents.FindAsync(id);

            if (ruvent == null)
            {
                return NotFound();
            }

            return ruvent;
        }

        // PUT: api/Ruvents/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPost("{id}")]
        public async Task<IActionResult> UpdateRuvent(int id, Ruvent ruvent)
        {
            if (id != ruvent.RuventId)
            {
                return BadRequest();
            }

            _context.Entry(ruvent).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RuventExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Ruvents
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPost]
        public async Task<ActionResult<Ruvent>> PostRuvent(Ruvent ruvent)
        {
            ruvent.CreateDate = DateTime.Now;

            _context.Ruvents.Add(ruvent);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetRuvent", new { id = ruvent.RuventId }, ruvent);
        }

        // DELETE: api/Ruvents/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Ruvent>> DeleteRuvent(int id)
        {
            var ruvent = await _context.Ruvents.FindAsync(id);
            if (ruvent == null)
            {
                return NotFound();
            }

            _context.Ruvents.Remove(ruvent);
            await _context.SaveChangesAsync();

            return ruvent;
        }

        private bool RuventExists(int id)
        {
            return _context.Ruvents.Any(e => e.RuventId == id);
        }
    }
}
