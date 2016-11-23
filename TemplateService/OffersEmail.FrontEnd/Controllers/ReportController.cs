//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace OffersEmail.FrontEnd.Controllers
{
    using System;
    using System.IO;
    using System.Net;
    using System.Text;
    using System.Web.Mvc;
    using DataContracts;
    using iTextSharp.text.pdf;
    using iTextSharp.tool.xml;
    using Newtonsoft.Json;

    /// <summary>
    /// Report templates
    /// </summary>
    public class ReportController : Controller
    {
        /// <summary>
        /// Merchant report html.
        /// </summary>
        /// <returns>The Merchant report template</returns>
        [HttpPost]
        public ActionResult MerchantBillingStatement()
        {
            if (Request.ContentLength == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Nothing was submitted");
            }

            Request.InputStream.Position = 0;
            using (var reader = new StreamReader(Request.InputStream))
            {
                try
                {
                    var content = reader.ReadToEnd();
                    var model = JsonConvert.DeserializeObject<MerchantBillingStatementContract>(content);
                    if (model != null && !string.IsNullOrWhiteSpace(model.BusinessName))
                    {
                        return View(model);
                    }
                }
                catch (Exception ex)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest, ex.Message);
                }
            }

            return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Content is missing");
        }

        /// <summary>
        /// Merchant report PDF.
        /// </summary>
        /// <returns>The merchant report PDF</returns>
        [HttpPost]
        public ActionResult MerchantBillingStatementPdf()
        {
            if (Request.ContentLength == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Nothing was submitted");
            }

            Request.InputStream.Position = 0;
            using (var reader = new StreamReader(Request.InputStream))
            {
                try
                {
                    var content = reader.ReadToEnd();
                    var model = JsonConvert.DeserializeObject<MerchantBillingStatementContract>(content);
                    if (model != null && !string.IsNullOrWhiteSpace(model.BusinessName))
                    {
                        ViewData.Model = model;
                        using (var sw = new StringWriter())
                        {
                            var viewResult = ViewEngines.Engines.FindView(ControllerContext, "MerchantBillingStatement", null);
                            var viewContext = new ViewContext(ControllerContext, viewResult.View, ViewData, TempData, sw);
                            viewResult.View.Render(viewContext, sw);
                            using (var input = new MemoryStream(Encoding.UTF8.GetBytes(sw.GetStringBuilder().ToString())))
                            {
                                var output = new MemoryStream();
                                var document = new iTextSharp.text.Document(iTextSharp.text.PageSize.LETTER, 50, 50, 50, 50);
                                var writer = PdfWriter.GetInstance(document, output);
                                writer.CloseStream = false;
                                document.Open();

                                var xmlWorker = XMLWorkerHelper.GetInstance();
                                xmlWorker.ParseXHtml(writer, document, input, Encoding.UTF8);
                                document.Close();
                                output.Position = 0;

                                return new FileStreamResult(output, "application/pdf");
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest, ex.Message);
                }
            }

            return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Content is missing");
        }
    }
}