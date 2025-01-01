using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace CRUDOperations.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ConfigController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        //private readonly AttachmentOptions _attachmentOptions;
        private readonly IOptionsSnapshot<AttachmentOptions> _attachmentOptions;


        // IOptions<AttachmentOptions>
        // IOptionsSnapshot<AttachmentOptions>
        // IOptionsMonitor<AttachmentOptions>

        public ConfigController(IConfiguration configuration,
            //AttachmentOptions attachmentOptions
            IOptionsSnapshot<AttachmentOptions> attachmentOptions)
        {
            _configuration = configuration;
            _attachmentOptions = attachmentOptions;
        }

        [HttpGet]
        [Route("")]
        public IActionResult GetConfig()
        {
            //Thread.Sleep(500);
            var congig = new
            {
                EnvName = _configuration["ASPNETCORE_ENVIRONMENT"],
                TestConfig = _configuration["TestConfig"],
                DefaulConnection = _configuration.GetConnectionString("DefaultConnection"),
                //MaxSizeInMegaBytes = _attachmentOptions.MaxSizeInMegaBytes
                attachmentOptions = _attachmentOptions.Value

            };

            return Ok(congig);
        }
    }
}
