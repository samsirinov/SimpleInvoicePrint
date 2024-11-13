using iText.Forms;
using iText.Forms.Fields;
using iText.IO.Font;
using iText.Kernel.Font;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Borders;
using iText.Layout.Element;
using iText.Layout.Layout;
using iText.Layout.Properties;
using iText.Layout.Renderer;
using WebApplication1.Enums;
using WebApplication1.Models;

namespace WebApplication1.Services
{
    public class InvoiceService
    {
        private readonly IWebHostEnvironment _env;

        public InvoiceService(IWebHostEnvironment env)
        {
            _env = env;
        }

        public InvoiceCreateAndDownloadResponse GenerateInvoice(Languages language, string senderTo, int productCount)
        {
            var newguid = Guid.NewGuid();
            string newnm = $"GeneratedInvoice{newguid}.pdf";
            InvoiceCreateAndDownloadResponse response = new();
            string inputdirectoryPath = Path.Combine(_env.WebRootPath, "pdf-themplates/", "invoice/");
            string OutputdirectoryPath = Path.Combine(_env.WebRootPath, "downloads/", "invoices/");
            string inputFilePath = string.Format("{0}{1}", inputdirectoryPath, "Test invoice[2].pdf");
            string outputFilePath = string.Format("{0}{1}", OutputdirectoryPath, newnm);

            string halvetica = Path.Combine(_env.WebRootPath, "fonts", "Helvetica.ttf");
            string halveticaBold = Path.Combine(_env.WebRootPath, "fonts", "Helvetica-Bold.ttf");

            using (PdfReader reader = new PdfReader(inputFilePath))
            using (PdfWriter writer = new PdfWriter(outputFilePath))
            using (PdfDocument pdfDoc = new PdfDocument(reader, writer))
            {
                PdfFont font = PdfFontFactory.CreateFont(halvetica, PdfEncodings.IDENTITY_H);
                PdfFont Boldfont = PdfFontFactory.CreateFont(halveticaBold, PdfEncodings.IDENTITY_H);
                PdfAcroForm form = PdfAcroForm.GetAcroForm(pdfDoc, true);

                PdfFormField DateTitle = form.GetField("Date");
                if (DateTitle != null)
                {
                    DateTitle.SetFont(Boldfont);
                    DateTitle.SetFontSize(10.08F);
                    DateTitle.SetValue("Invoice_DateTitle".GetLocalizedText(language));
                }

                PdfFormField QquotationNumberField = form.GetField("DateValue");
                if (QquotationNumberField != null)
                {
                    QquotationNumberField.SetFont(font);
                    QquotationNumberField.SetFontSize(10.08F);
                    QquotationNumberField.SetValue("13.05.2025");
                }
                PdfFormField Quotation = form.GetField("Quotation");
                if (Quotation != null)
                {
                    Quotation.SetFont(Boldfont);
                    Quotation.SetFontSize(10.08F);
                    Quotation.SetValue("Invoice_QuotationTitle".GetLocalizedText(language));
                }
                PdfFormField QuotationNumber = form.GetField("QuotationNumber");
                if (DateTitle != null)
                {
                    QuotationNumber.SetFont(Boldfont);
                    QuotationNumber.SetFontSize(10.08F);
                    QuotationNumber.SetValue("Invoice_QuotationNumberTitle".GetLocalizedText(language));
                }

                PdfFormField quotationNumberField = form.GetField("QuotationNumberValue");
                if (quotationNumberField != null)
                {
                    quotationNumberField.SetFont(font);
                    quotationNumberField.SetFontSize(10.08F);
                    quotationNumberField.SetValue("PTMS1905-24");
                }

                PdfFormField DeliveryTerms = form.GetField("DeliveryTerms");
                if (DateTitle != null)
                {
                    DeliveryTerms.SetFont(Boldfont);
                    DeliveryTerms.SetFontSize(10.08F);
                    DeliveryTerms.SetValue("Invoice_DeliveryTermsTitle".GetLocalizedText(language));
                }
                PdfFormField deliveryTermsField = form.GetField("DeliveryTermsValue");
                if (deliveryTermsField != null)
                {
                    deliveryTermsField.SetFont(font);
                    deliveryTermsField.SetFontSize(10.08F);
                    deliveryTermsField.SetValue("FCA Istanbul");
                }

                PdfFormField DeliveryTime = form.GetField("DeliveryTime");
                if (DateTitle != null)
                {
                    DeliveryTime.SetFont(Boldfont);
                    DeliveryTime.SetFontSize(10.08F);
                    DeliveryTime.SetValue("Invoice_DeliveryTimeTitle".GetLocalizedText(language));
                }
                PdfFormField deliveryTimeField = form.GetField("DeliveryTimeValue");
                if (deliveryTimeField != null)
                {
                    deliveryTimeField.SetFont(font);
                    deliveryTimeField.SetFontSize(9.78F);
                    deliveryTimeField.SetValue("As agreed in terms below");
                }
                form.GetField("To")?.SetValue(string.Format("{0}: {1}", "Invoice_To".GetLocalizedText(language), senderTo))
                    .SetFont(Boldfont).SetFontSize(10.08F);


                Document doc = new Document(pdfDoc);



                CreateInvoiceTable(pdfDoc, doc, productCount, language);

                form.FlattenFields();
            }

            response.InvoiceUrl = "http://artiqaz-001-site3.qtempurl.com/downloads/invoices/" + newnm;
            response.Succeeded = true;
            return response;

        }


        public void CreateInvoiceTable(PdfDocument pdfDoc, Document doc, int elements, Languages language)
        {
            string helveticaPath = Path.Combine(_env.WebRootPath, "fonts", "Helvetica.ttf");
            string helveticaBoldPath = Path.Combine(_env.WebRootPath, "fonts", "Helvetica-Bold.ttf");
            PdfFont font = PdfFontFactory.CreateFont(helveticaPath, PdfEncodings.IDENTITY_H);
            PdfFont boldFont = PdfFontFactory.CreateFont(helveticaBoldPath, PdfEncodings.IDENTITY_H);

            float tableX = 63.27f;
            float tableY = 472.43f;

            tableY = tableY - (elements * 43f);

            Table table = new Table(UnitValue.CreatePercentArray(new float[] { 3, 1.5f, 1.5f, 2 }));
            table.SetWidth(UnitValue.CreatePercentValue(150));

            // Başlık satırı
            table.AddHeaderCell(CreateHeaderCell("InvoiceTable_ProductName".GetLocalizedText(language), boldFont));
            table.AddHeaderCell(CreateHeaderCell("InvoiceTable_Quantity".GetLocalizedText(language), boldFont));

            table.AddHeaderCell(
                new Cell()
                .Add(new Paragraph("InvoiceTable_UnitPrice".GetLocalizedText(language)).SetFont(boldFont).SetFontSize(9))
                .SetTextAlignment(TextAlignment.CENTER)
                .SetBorder(new SolidBorder(1))
                );
            table.AddHeaderCell(CreateHeaderCell("InvoiceTable_TotalAmount".GetLocalizedText(language), boldFont));

            for (int i = 0; i < elements; i++)
            {

                Cell productCell = new Cell(2, 1)
                    .Add(new Paragraph("Template good").SetFont(boldFont))
                    .Add(new Paragraph("Description").SetFont(font))
                            .SetBorder(new SolidBorder(1));
                productCell.SetVerticalAlignment(VerticalAlignment.MIDDLE);
                table.AddCell(productCell);

                Cell productQTYCell = new Cell(2, 1)
                    .Add(new Paragraph("10 000 pcs").SetFont(font))
                    .SetTextAlignment(TextAlignment.CENTER)
                    .SetBorder(new SolidBorder(1));
                table.AddCell(productQTYCell);

                Cell productunitPriceCell = new Cell(2, 1)
        .Add(new Paragraph("100,00").SetFont(font))
                        .SetTextAlignment(TextAlignment.CENTER)
            .SetBorder(new SolidBorder(1));
                table.AddCell(productunitPriceCell);

                Cell totalamt = new Cell(2, 1)
        .Add(new Paragraph("00 00 000,00").SetFont(font))
                        .SetTextAlignment(TextAlignment.CENTER)
        .SetBorder(new SolidBorder(1));
                table.AddCell(totalamt);

            }

            // KDV satırı
            table.AddCell(new Cell(1, 3) // İki sütunu kapsar
                .Add(new Paragraph("VAT 20%").SetFont(boldFont).SetFontSize(10))
                .SetTextAlignment(TextAlignment.RIGHT)
                .SetBorder(new SolidBorder(1)));
            table.AddCell(CreateCell("0 000 000", font, TextAlignment.CENTER));

            // Toplam satırı
            table.AddCell(new Cell(1, 3) // İki sütunu kapsar
                .Add(new Paragraph("Total:").SetFont(boldFont).SetFontSize(10))
                .SetTextAlignment(TextAlignment.RIGHT)
                .SetBorder(new SolidBorder(1)));
            table.AddCell(CreateCell("00 000 000 EURO", font, TextAlignment.CENTER));

            table.SetFixedPosition(1, tableX, tableY, pdfDoc.GetDefaultPageSize().GetWidth() - 2 * 49f);

            doc.Add(table);
        }

        // Helper method for header cells
        private Cell CreateHeaderCell(string content, PdfFont font)
        {
            return new Cell()
                .Add(new Paragraph(content).SetFont(font).SetFontSize(10))
                .SetTextAlignment(TextAlignment.CENTER)
                .SetBorder(new SolidBorder(1));
        }

        // Helper method for regular cells
        private Cell CreateCell(string content, PdfFont font, TextAlignment alignment = TextAlignment.CENTER)
        {
            return new Cell()
                .Add(new Paragraph(content).SetFont(font).SetFontSize(10))
                .SetTextAlignment(alignment)
                .SetVerticalAlignment(VerticalAlignment.MIDDLE)
                .SetBorder(new SolidBorder(1));
        }

    }
}
