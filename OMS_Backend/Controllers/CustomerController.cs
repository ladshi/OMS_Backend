using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OMS_Backend.Data;
using OMS_Backend.Models;

namespace OMS_Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CustomerController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Customer
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetCustomers()
        {
            var customers = await _context.Users
                .Where(u => u.Role == "Customer" && u.IsActive)
                .ToListAsync();
            return Ok(customers);
        }
    }
}
