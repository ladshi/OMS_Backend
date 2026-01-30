using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OMS_Backend.Data;
using OMS_Backend.DTOs;
using OMS_Backend.Models;

namespace OMS_Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomersController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CustomersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // FR-05: View customer list
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CustomerResponse>>> GetAll()
        {
            var customers = await _context.Customers
                .Where(c => c.IsActive)
                .OrderBy(c => c.Name)
                .ToListAsync();

            var response = customers.Select(c => new CustomerResponse(
                c.Id,
                c.CustomerCode,
                c.Name,
                c.Email,
                c.Phone,
                c.Address,
                c.IsActive,
                c.CreatedAt
            ));

            return Ok(response);
        }

        // Get customer by ID
        [HttpGet("{id}")]
        public async Task<ActionResult<CustomerResponse>> GetById(int id)
        {
            var customer = await _context.Customers.FindAsync(id);
            
            if (customer == null || !customer.IsActive)
            {
                return NotFound("Customer not found");
            }

            var response = new CustomerResponse(
                customer.Id,
                customer.CustomerCode,
                customer.Name,
                customer.Email,
                customer.Phone,
                customer.Address,
                customer.IsActive,
                customer.CreatedAt
            );

            return Ok(response);
        }

        // FR-03: Add new customer
        [HttpPost]
        public async Task<ActionResult<CustomerResponse>> Create(CustomerRequest request)
        {
            // Validate required fields
            if (string.IsNullOrWhiteSpace(request.CustomerCode) || string.IsNullOrWhiteSpace(request.Name))
            {
                return BadRequest("CustomerCode and Name are required");
            }

            // Check if customer code already exists
            var existingCustomer = await _context.Customers
                .FirstOrDefaultAsync(c => c.CustomerCode == request.CustomerCode && c.IsActive);
            
            if (existingCustomer != null)
            {
                return BadRequest("Customer code already exists");
            }

            var customer = new Customer
            {
                CustomerCode = request.CustomerCode,
                Name = request.Name,
                Email = request.Email,
                Phone = request.Phone,
                Address = request.Address,
                IsActive = true,
                CreatedAt = DateTime.Now
            };

            _context.Customers.Add(customer);
            await _context.SaveChangesAsync();

            var response = new CustomerResponse(
                customer.Id,
                customer.CustomerCode,
                customer.Name,
                customer.Email,
                customer.Phone,
                customer.Address,
                customer.IsActive,
                customer.CreatedAt
            );

            return CreatedAtAction(nameof(GetById), new { id = customer.Id }, response);
        }

        // FR-04: Edit customer details
        [HttpPut("{id}")]
        public async Task<ActionResult<CustomerResponse>> Update(int id, CustomerRequest request)
        {
            var customer = await _context.Customers.FindAsync(id);
            
            if (customer == null || !customer.IsActive)
            {
                return NotFound("Customer not found");
            }

            // Validate required fields
            if (string.IsNullOrWhiteSpace(request.CustomerCode) || string.IsNullOrWhiteSpace(request.Name))
            {
                return BadRequest("CustomerCode and Name are required");
            }

            // Check if customer code already exists for another customer
            var existingCustomer = await _context.Customers
                .FirstOrDefaultAsync(c => c.CustomerCode == request.CustomerCode && c.Id != id && c.IsActive);
            
            if (existingCustomer != null)
            {
                return BadRequest("Customer code already exists");
            }

            customer.CustomerCode = request.CustomerCode;
            customer.Name = request.Name;
            customer.Email = request.Email;
            customer.Phone = request.Phone;
            customer.Address = request.Address;

            await _context.SaveChangesAsync();

            var response = new CustomerResponse(
                customer.Id,
                customer.CustomerCode,
                customer.Name,
                customer.Email,
                customer.Phone,
                customer.Address,
                customer.IsActive,
                customer.CreatedAt
            );

            return Ok(response);
        }

        // FR-06: Delete customer (soft delete)
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var customer = await _context.Customers.FindAsync(id);
            
            if (customer == null || !customer.IsActive)
            {
                return NotFound("Customer not found");
            }

            // Soft delete - set IsActive to false
            customer.IsActive = false;
            await _context.SaveChangesAsync();

            return Ok(new { message = "Customer deleted successfully" });
        }
    }
}