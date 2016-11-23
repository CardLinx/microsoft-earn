//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace OffersEmail.FrontEnd.Controllers
{
    using System.Web.Mvc;

    /// <summary>
    /// HM email templates
    /// </summary>
    public class HmController : Controller
    {
        /// <summary>
        /// The CsvVideoTemplateTextA email template
        /// </summary>
        /// <param name="id">The template version</param>
        /// <returns>The CsvVideoTemplateTextA View</returns>
        public ActionResult CsvVideoTemplateText(string id)
        {
            id = id ?? string.Empty;
            switch (id.ToLowerInvariant())
            {
                case "b":
                    return View("CsvVideoTemplateTextB");
                case "c":
                    return View("CsvVideoTemplateTextC");
                case "d":
                    return View("CsvVideoTemplateTextD");
                default:
                    return View("CsvVideoTemplateTextA");
            }
        }
    }
}