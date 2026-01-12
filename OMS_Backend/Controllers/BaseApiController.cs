using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OMS_Backend.Data;

namespace OMS_Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BaseApiController<T> : ControllerBase where T : class
    {
        protected readonly ApplicationDbContext _context;

        public BaseApiController(ApplicationDbContext context)
        {
            _context = context;
        }
        [HttpGet]
        public virtual async Task<ActionResult<IEnumerable<T>>> GetAll()
        {
            return await _context.Set<T>().ToListAsync();
        }

        [HttpGet("{id}")]
        public virtual async Task<ActionResult<T>> GetById(int id)
        {
            var entity = await _context.Set<T>().FindAsync(id);
            if (entity == null) return NotFound("Not Found!");
            return entity;
        }

        [HttpPost]
        public virtual async Task<ActionResult<T>> Create(T entity)
        {
            _context.Set<T>().Add(entity);
            await _context.SaveChangesAsync();
            return Ok(entity);
        }

        [HttpPut("{id}")]
        public virtual async Task<IActionResult> Update(int id, T entity)
        {
            _context.Entry(entity).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await EntityExists(id)) return NotFound();
                else throw;
            }

            return Ok("Added sucessfully!");
        }

        [HttpDelete("{id}")]
        public virtual async Task<IActionResult> Delete(int id)
        {
            var entity = await _context.Set<T>().FindAsync(id);
            if (entity == null) return NotFound();

            _context.Set<T>().Remove(entity);
            await _context.SaveChangesAsync();

            return Ok("Deleted successfully!");
        }

        // an helper method to check there is an entity with given id
        private async Task<bool> EntityExists(int id)
        {
            var entity = await _context.Set<T>().FindAsync(id);
            return entity != null;
        }
    }
}