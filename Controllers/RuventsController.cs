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

        [HttpGet("audit/{id}")]
        public async Task<ActionResult<Object>> GetRuventAudit(int id)
        {
            var ruvent = await _context.Ruvents.FindAsync(id);

            if (ruvent == null)
            {
                return NotFound();
            }

            var creator = _context.Users.Where(x => x.Username == ruvent.CreatedBy).Select(u => new { u.NickName, ruvent.CreateDate }).FirstOrDefault();
            var modifier = _context.Users.Where(x => x.Username == ruvent.ModifyBy).Select(u => new { u.NickName, ruvent.ModifyDate }).FirstOrDefault();

            var audit = new
            {
                CreatedBy = string.IsNullOrEmpty(creator.NickName) ? string.Empty : creator.NickName,
                CreateDate = creator.CreateDate.ToString() ?? string.Empty,
                ModifyBy = modifier == null ? string.Empty : (modifier.NickName == null ? string.Empty : modifier.NickName),
                ModifyDate = modifier == null ? string.Empty : (modifier.ModifyDate == null ? string.Empty : modifier.ModifyDate.ToString())
            };

            return audit;
        }

        [Authorize]
        [HttpPost("{id}")]
        public async Task<IActionResult> UpdateRuvent(int id, Ruvent ruvent)
        {
            if (id != ruvent.RuventId)
            {
                return BadRequest();
            }

            var currentUser = _context.Users.Where(u => u.Username == User.Identity.Name).FirstOrDefault();

            _context.Entry(ruvent).State = EntityState.Modified;
            ruvent.ModifyDate = DateTime.Now;
            ruvent.ModifyBy = currentUser.NickName == null || currentUser.NickName == string.Empty ? currentUser.Username : currentUser.NickName;

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
            var currentUser = _context.Users.Where(u => u.Username == User.Identity.Name).FirstOrDefault();

            ruvent.CreateDate = DateTime.Now;
            ruvent.CreatedBy = currentUser.NickName == null || currentUser.NickName == string.Empty ? currentUser.Username : currentUser.NickName;

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
