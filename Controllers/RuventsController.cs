using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
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

        [HttpGet("{month}/{year}")]
        public async Task<ActionResult<IEnumerable<Ruvent>>> GetRuvents(int month, int year)
        {
            return await _context.Ruvents
                .Where(r => r.StartDate.Value.Month == month && r.StartDate.Value.Year == year)
                .OrderBy(r => r.StartDate)
                .ToListAsync();
        }

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

        [Authorize]
        [HttpPost("{id}")]
        public async Task<IActionResult> UpdateRuvent(int id, Ruvent ruvent)
        {
            if (id != ruvent.RuventId)
            {
                return BadRequest();
            }

            _context.Entry(ruvent).State = EntityState.Modified;
            ruvent.ModifyDate = DateTime.Now;

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

        [Authorize]
        [HttpPost]
        public async Task<ActionResult<Ruvent>> PostRuvent(Ruvent ruvent)
        {
            ruvent.CreateDate = DateTime.Now;

            _context.Ruvents.Add(ruvent);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetRuvent", new { id = ruvent.RuventId }, ruvent);
        }

        [Authorize]
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
