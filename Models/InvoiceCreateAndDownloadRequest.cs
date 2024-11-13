using WebApplication1.Enums;

namespace WebApplication1.Models
{
    public class InvoiceCreateAndDownloadRequest
    {
        public Languages LanguageId { get; set; }
        public string To { get; set; }
        public int ProductCount { get; set; }

    }
}
