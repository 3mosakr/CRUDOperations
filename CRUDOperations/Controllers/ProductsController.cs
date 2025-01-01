using CRUDOperations.Data;
using CRUDOperations.Filters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CRUDOperations.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize]

    public class ProductsController : ControllerBase
    {
        private readonly ApplicationDbContext _dbcontext;

        public ProductsController(ApplicationDbContext dbcontext)
        {
            _dbcontext = dbcontext;
        }

        [HttpGet]
        [Route("")]
        public ActionResult<IEnumerable<Product>> Get()
        {
            // get user data in api
            var username = User.Identity.Name;
            var userId = ((ClaimsIdentity)User.Identity).FindFirst(ClaimTypes.NameIdentifier).Value;

            var records = _dbcontext.Set<Product>().ToList(); 
            return Ok(records);
        }

        [HttpGet]
        //[Route("GetById")]
        [Route("{id}")]
        [AllowAnonymous]    
        [LogSensitiveAction]
        //public ActionResult<Product> GetById([FromQuery(Name = "Key")] int id)
        public ActionResult<Product> GetById(int id)
        {
            var record = _dbcontext.Set<Product>().Find(id);
            if(record is null) 
                return NotFound();

            return Ok(record);
        }

        [HttpPost]
        [Route("")] // default post Method
        public ActionResult<int> CreateProduct(Product product, [FromHeader(Name = "Accept-Language")] string language)
        {
            product.Id = 0;  // defult value use for insert in db - can remove it
            _dbcontext.Set<Product>().Add(product);

            _dbcontext.SaveChanges();
            return Ok(product.Id);
        }

        [HttpPut]
        [Route("")] 
        public ActionResult<int> UpdateProduct(Product product)
        {
            var existingProduct = _dbcontext.Set<Product>().Find(product.Id);
            if (existingProduct == null)
                return NotFound();
            existingProduct.Name = product.Name;
            existingProduct.Sku = product.Sku;

            _dbcontext.Set<Product>().Update(existingProduct);
            _dbcontext.SaveChanges();
            return Ok();
        }

        [HttpDelete]
        [Route("{id}")]  // must send id
        public ActionResult<int> DeleteProduct(int id)
        {
            var existingProduct = _dbcontext.Set<Product>().Find(id);
            if (existingProduct == null)
                return NotFound();

            _dbcontext.Set<Product>().Remove(existingProduct);
            _dbcontext.SaveChanges();
            return Ok();
        }
    }
}
