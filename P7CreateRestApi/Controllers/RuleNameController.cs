using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using P7CreateRestApi.Data;
using P7CreateRestApi.Models;

namespace P7CreateRestApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class RuleNameController : ControllerBase
    {
        private readonly LocalDbContext _context;

        public RuleNameController(LocalDbContext context)
        {
            _context = context;
        }

        // GET: /RuleName
        [HttpGet]
        public async Task<ActionResult<IEnumerable<RuleName>>> GetRuleNames()
        {
            return await _context.RuleNames.ToListAsync();
        }

        // GET: /RuleName/5
        [HttpGet("{id}")]
        public async Task<ActionResult<RuleName>> GetRuleName(int id)
        {
            var ruleName = await _context.RuleNames.FindAsync(id);

            if (ruleName == null)
            {
                return NotFound();
            }

            return ruleName;
        }

        // PUT: /RuleName/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutRuleName(int id, RuleName ruleName)
        {
            if (id != ruleName.Id)
            {
                return BadRequest();
            }

            _context.Entry(ruleName).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RuleNameExists(id))
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

        // POST: /RuleName
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<RuleName>> PostRuleName(RuleName ruleName)
        {
            _context.RuleNames.Add(ruleName);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetRuleName", new { id = ruleName.Id }, ruleName);
        }

        // DELETE: /RuleName/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRuleName(int id)
        {
            var ruleName = await _context.RuleNames.FindAsync(id);
            if (ruleName == null)
            {
                return NotFound();
            }

            _context.RuleNames.Remove(ruleName);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool RuleNameExists(int id)
        {
            return _context.RuleNames.Any(e => e.Id == id);
        }
    }
}
