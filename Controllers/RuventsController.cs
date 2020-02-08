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
        public async Task<ActionResult<IEnumerable<RuventAttendanceVM>>> GetRuvents(int month, int year)
        {
            var vms = await (from r in _context.Ruvents
                            where r.StartDate.Value.Month == month && r.StartDate.Value.Year == year
                            select new RuventAttendanceVM
                            {
                                RuventId = r.RuventId,
                                Title = r.Title,
                                Description = r.Description,
                                Address = r.Address,
                                StartDate = r.StartDate,
                                StartTime = r.StartTime,
                                EndDate = r.EndDate,
                                EndTime = r.EndTime,
                                CreateDate = r.CreateDate,
                                CreatedBy = r.CreatedBy,
                                ModifyDate = r.ModifyDate,
                                ModifyBy = r.ModifyBy
                            }).ToListAsync();

            foreach (var vm in vms)
            {
                vm.Attending = _context.RuventToUser.Where(r => r.RuventId == vm.RuventId && r.IsAttending == true).Count();
                vm.NotAttending = _context.RuventToUser.Where(r => r.RuventId == vm.RuventId && r.IsAttending == false).Count();
            }

            return vms.OrderBy(r => r.StartDate).ToList();
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
