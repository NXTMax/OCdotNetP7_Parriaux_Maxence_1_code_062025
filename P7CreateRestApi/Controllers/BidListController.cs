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
    public class BidListController : ControllerBase
    {
        private readonly LocalDbContext _context;

        public BidListController(LocalDbContext context)
        {
            _context = context;
        }

        // GET: /BidList
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BidList>>> GetBidLists()
        {
            return await _context.BidLists.ToListAsync();
        }

        // GET: /BidList/5
        [HttpGet("{id}")]
        public async Task<ActionResult<BidList>> GetBidList(int id)
        {
            var bidList = await _context.BidLists.FindAsync(id);

            if (bidList == null)
            {
                return NotFound();
            }

            return bidList;
        }

        // PUT: /BidList/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutBidList(int id, BidList bidList)
        {
            if (id != bidList.BidListId)
            {
                return BadRequest();
            }

            _context.Entry(bidList).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BidListExists(id))
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

        // POST: /BidList
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<BidList>> PostBidList(BidList bidList)
        {
            _context.BidLists.Add(bidList);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetBidList", new { id = bidList.BidListId }, bidList);
        }

        // DELETE: /BidList/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBidList(int id)
        {
            var bidList = await _context.BidLists.FindAsync(id);
            if (bidList == null)
            {
                return NotFound();
            }

            _context.BidLists.Remove(bidList);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool BidListExists(int id)
        {
            return _context.BidLists.Any(e => e.BidListId == id);
        }
    }
}
