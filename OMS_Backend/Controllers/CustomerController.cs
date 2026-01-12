using Microsoft.AspNetCore.Mvc;
using OMS_Backend.Data;
using OMS_Backend.Entities;

namespace OMS_Backend.Controllers
{
    public class CustomerController : BaseApiController<Customer>
    {
        public CustomerController(ApplicationDbContext context) : base(context)
        {

        }
    }
}
