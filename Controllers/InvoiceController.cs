using Microsoft.AspNetCore.Mvc;
using WebApplication1.Models;
using WebApplication1.Services;

namespace WebApplication1.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class InvoiceController(InvoiceService invoiceService) : ControllerBase
    {
        private readonly InvoiceService _invoiceService = invoiceService;

        [HttpPost]
        public async Task<IActionResult> CreateAndDownloadInvoice([FromBody]InvoiceCreateAndDownloadRequest requset)
        {
            var result = _invoiceService.GenerateInvoice(requset.LanguageId, requset.To, requset.ProductCount);

            if (result.Succeeded)
                return Ok(result);
            else
                return StatusCode(StatusCodes.Status403Forbidden, result);
        }
    }
}
