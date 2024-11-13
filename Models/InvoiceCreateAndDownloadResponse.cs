namespace WebApplication1.Models
{
    public class InvoiceCreateAndDownloadResponse
    {
        public bool Succeeded { get; set; }
        public string Message { get; set; }
        public string InvoiceUrl { get; set; }
    }
}
