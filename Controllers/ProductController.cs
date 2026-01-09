using Microsoft.AspNetCore.Authorization;

using Microsoft.AspNetCore.Mvc;

using Microsoft.EntityFrameworkCore;

using OMS_Backend.Data;

using OMS_Backend.Models;

using OMS_Backend.Enums;
 
namespace OMS_Backend.Controllers

{

    [Route("api/[controller]")]

    [ApiController]

    [AllowAnonymous] // Allow anonymous access to product endpoints

    public class ProductController : ControllerBase

    {

        private readonly ApplicationDbContext _context;
 
        public ProductController(ApplicationDbContext context)

        {

            _context = context;

        }
 
        // GET: api/Product

        // Returns only non-deleted products

        [HttpGet]

        public async Task<ActionResult<IEnumerable<Product>>> GetAll()

        {

            var products = await _context.Product

                .Where(p => !p.IsDeleted)

                .ToListAsync();
 
            return Ok(products);

        }
 
        // GET: api/Product/id

        [HttpGet("{id}")]

        public async Task<ActionResult<Product>> GetById(int id)

        {

            var product = await _context.Product

                .FirstOrDefaultAsync(p => p.ProductId == id && !p.IsDeleted);
 
            if (product == null)

                return NotFound("Product not found");
 
            return Ok(product);

        }
 
        // POST: api/Product

        [HttpPost]

        public async Task<ActionResult<Product>> Create([FromBody] Product product)

        {

            // Validate input
            if (product == null || string.IsNullOrWhiteSpace(product.ProductName))
            {
                return BadRequest(new { message = "Product name is required" });
            }

            if (product.Price <= 0)
            {
                return BadRequest(new { message = "Price must be greater than 0" });
            }

            if (product.Quantity < 0)
            {
                return BadRequest(new { message = "Quantity cannot be negative" });
            }

            // Check if a product with the same name and price already exists

            var existingProduct = await _context.Product

                .FirstOrDefaultAsync(p => 

                    p.ProductName.ToLower().Trim() == product.ProductName.ToLower().Trim() &&

                    p.Price == product.Price &&

                    !p.IsDeleted);
 
            if (existingProduct != null)

            {

                // Increase the quantity of the existing product

                existingProduct.Quantity += product.Quantity;

                existingProduct.UpdatedAt = DateTime.Now;

                // Update product status

                if (existingProduct.Quantity > 0 && existingProduct.ProductStatus == ProductStatus.OutOfStock)

                {

                    existingProduct.ProductStatus = ProductStatus.Available;

                }
 
                _context.Product.Update(existingProduct);

                await _context.SaveChangesAsync();
 
                return Ok(existingProduct);

            }
 
            // Create new product if no matching product found
            product.ProductId = 0; // Ensure ProductId is 0 for new products
            product.CreatedAt = DateTime.Now;
            product.IsDeleted = false;

            // Set default ProductStatus if not provided
            if (product.ProductStatus == 0)
            {
                product.ProductStatus = product.Quantity > 0 ? ProductStatus.Available : ProductStatus.OutOfStock;
            }

            try
            {
                _context.Product.Add(product);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetById), new { id = product.ProductId }, product);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error creating product", error = ex.Message });
            }

        }
 
        // PUT: api/Product/id

        [HttpPut("{id}")]

        public async Task<IActionResult> Update(int id, [FromBody] Product product)

        {

            if (id != product.ProductId)

                return BadRequest("Product ID mismatch");
 
            var existingProduct = await _context.Product

                .FirstOrDefaultAsync(p => p.ProductId == id && !p.IsDeleted);
 
            if (existingProduct == null)

                return NotFound("Product not found");
 
            // Update properties

            existingProduct.ProductName = product.ProductName;

            existingProduct.Description = product.Description;

            existingProduct.Price = product.Price;

            existingProduct.Quantity = product.Quantity;

            existingProduct.ProductStatus = product.ProductStatus;

            existingProduct.UpdatedAt = DateTime.Now;
 
            _context.Product.Update(existingProduct);

            await _context.SaveChangesAsync();
 
            return Ok(existingProduct);

        }
 
        // DELETE: api/Product/id

        [HttpDelete("{id}")]

        public async Task<IActionResult> Delete(int id)

        {

            var product = await _context.Product.FindAsync(id);
 
            if (product == null)

                return NotFound("Product not found");
 
            product.IsDeleted = true;

            product.UpdatedAt = DateTime.Now;
 
            _context.Product.Update(product);

            await _context.SaveChangesAsync();
 
            return Ok(new { message = "Product deleted successfully", productId = id });

        }

    }

}

 