using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OMS_Backend.Data;
using OMS_Backend.Entities;
using OMS_Backend.Enums;

namespace OMS_Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : BaseApiController<Product>
    {
        public ProductController(ApplicationDbContext context) : base(context)
        {
        }

        // GET: api/Product
        // Override to return only non-deleted products
        [HttpGet]
        public override async Task<ActionResult<IEnumerable<Product>>> GetAll()
        {
            var products = await _context.Product
                .Where(p => !p.IsDeleted)
                .ToListAsync();

            return Ok(products);
        }

        // GET: api/Product/id
        [HttpGet("{id}")]
        public override async Task<ActionResult<Product>> GetById(int id)
        {
            var product = await _context.Product
                .FirstOrDefaultAsync(p => p.ProductId == id && !p.IsDeleted);

            if (product == null)
                return NotFound("Product not found");

            return Ok(product);
        }

        // POST: api/Product
        [HttpPost]
        public override async Task<ActionResult<Product>> Create(Product product)
        {
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
                
                // Update product status if needed (e.g., if quantity was 0 and now has stock)
                if (existingProduct.Quantity > 0 && existingProduct.ProductStatus == ProductStatus.OutOfStock)
                {
                    existingProduct.ProductStatus = ProductStatus.Available;
                }

                _context.Product.Update(existingProduct);
                await _context.SaveChangesAsync();

                return Ok(existingProduct);
            }

            // Create new product if no matching product found
            product.CreatedAt = DateTime.Now;
            product.IsDeleted = false;

            _context.Product.Add(product);
            await _context.SaveChangesAsync();

            return Ok(product);
        }

        // PUT: api/Product/id
        [HttpPut("{id}")]
        public override async Task<IActionResult> Update(int id, Product product)
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

        // DELETE: api/Product/5 (Soft Delete)
        [HttpDelete("{id}")]
        public override async Task<IActionResult> Delete(int id)
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
