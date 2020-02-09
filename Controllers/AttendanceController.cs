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
    public class AttendanceController : ControllerBase
    {
        private readonly RuventsContext _context;

        public AttendanceController(RuventsContext context)
        {
            _context = context;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<IEnumerable<Object>>> GetAttendance(int id)
        {
            var attendance = await _context.RuventToUser.Where(x => x.RuventId == id).Include("User").ToListAsync();

            return (from a in attendance
                    select new
                    {
                        a.RuventToUserId,
                        a.IsAttending,
                        a.RuventId,
                        a.UserId,
                        a.User.PhoneNumber,
                        a.User.FirstName,
                        a.User.LastName,
                        a.User.NickName
                    }).ToList();
        }

        [HttpPost]
        public async Task<ActionResult<IEnumerable<Object>>> PostAttendance(RuventToUser attendance)
        {
            _context.RuventToUser.Add(attendance);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPost("{id}")]
        public async Task<IActionResult> UpdateAttendance(int id, RuventToUser attendance)
        {
            if (id != attendance.RuventToUserId)
            {
                return BadRequest();
            }

            _context.Entry(attendance).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (RuventToUserExists(id))
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

        [HttpDelete("{id}")]
        public async Task<ActionResult<RuventToUser>> DeleteAttendance(int id)
        {
            var attendance = await _context.RuventToUser.FindAsync(id);
            if (attendance == null)
            {
                return NotFound();
            }

            _context.RuventToUser.Remove(attendance);
            await _context.SaveChangesAsync();

            return attendance;
        }

        private bool RuventToUserExists(int id)
        {
            return _context.RuventToUser.Any(r => r.RuventToUserId == id);
        }
    }
}