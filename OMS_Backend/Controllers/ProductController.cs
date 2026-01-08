using OMS_Backend.Data;
using OMS_Backend.Entities;

namespace OMS_Backend.Controllers
{
    public class ProductController : BaseApiController<Product>
    {
        public ProductController(ApplicationDbContext context) : base(context)
        {

        }
    }
}
