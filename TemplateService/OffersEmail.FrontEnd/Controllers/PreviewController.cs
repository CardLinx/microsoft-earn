//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace OffersEmail.FrontEnd.Controllers
{
    using System;
    using System.Configuration;
    using System.Diagnostics;
    using System.Drawing;
    using System.IO;
    using System.Net;
    using System.Net.Mime;
    using System.Web.Mvc;
    using Lomo.Logging;
    using Microsoft.WindowsAzure.ServiceRuntime;
    using Microsoft.WindowsAzure.Storage;
    using Microsoft.WindowsAzure.Storage.Blob;
    using Models;
    using Newtonsoft.Json;

    /// <summary>
    /// The Hero Tile preview controller
    /// </summary>
    public class PreviewController : Controller
    {
        #region Field members

        /// <summary>
        /// The sync root
        /// </summary>
        private static readonly object SyncRoot = new object();

        #endregion

        #region Public members

        /// <summary>
        /// Heroes the tile HTML.
        /// </summary>
        /// <returns>
        /// The View
        /// </returns>
        [HttpPost]
        public ActionResult HeroTileHtml()
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
                    var model = JsonConvert.DeserializeObject<DealPreviewModel>(content);
                    if (model != null)
                    {
                        return View(model);
                    }
                }
                catch (Exception ex)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest, ex.Message);
                }
            }

            return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Preview deal model is missing");
        }

        /// <summary>
        /// Hero tile image.
        /// </summary>
        /// <returns>
        /// The hero tile image
        /// </returns>
        [HttpPost]
        public ActionResult HeroTileImage()
        {
            return ProcessImage(false);
        }

        /// <summary>
        /// Uploads the hero tile image.
        /// </summary>
        /// <returns>Uploads the image file to azure</returns>
        [HttpPost]
        public ActionResult UploadHeroTileImage()
        {
            return ProcessImage(true);
        }

        #endregion

        #region Private members

        /// <summary>
        /// Processes the image.
        /// </summary>
        /// <param name="upload">if set to <c>true</c> [upload].</param>
        /// <returns>The Image processing result</returns>
        private ActionResult ProcessImage(bool upload)
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
                    var model = JsonConvert.DeserializeObject<DealPreviewModel>(content);
                    if (model != null)
                    {
                        int imageHeight;
                        if (!int.TryParse(Request.QueryString["h"], out imageHeight))
                        {
                            imageHeight = 470;
                        }

                        int imageWidth;
                        if (!int.TryParse(Request.QueryString["w"], out imageWidth))
                        {
                            imageWidth = 950;
                        }

                        string localResourcePath = RoleEnvironment.IsAvailable ? RoleEnvironment.GetLocalResource("Screenshot").RootPath : Server.MapPath("\\Screenshot");
                        var fileName = model.ProviderDealId ?? Guid.NewGuid().ToString();

                        using (TextWriter writer = System.IO.File.CreateText(Path.Combine(localResourcePath, string.Concat(fileName, ".html"))))
                        {
                            ViewData.Model = model;
                            using (var sw = new StringWriter())
                            {
                                var viewResult = ViewEngines.Engines.FindView(ControllerContext, "HeroTileHtml", null);
                                var viewContext = new ViewContext(ControllerContext, viewResult.View, ViewData, TempData, sw);
                                viewResult.View.Render(viewContext, sw);
                                writer.Write(sw.GetStringBuilder().ToString());
                                writer.Close();
                            }
                        }

                        if (CreateImage(localResourcePath, fileName))
                        {
                            var imageName = string.Concat(fileName, ".jpg");
                            using (var fileStream = new FileStream(Path.Combine(localResourcePath, imageName), FileMode.Open, FileAccess.Read))
                            {
                                var newBitmap = new Bitmap(Image.FromStream(fileStream), new Size(imageWidth, imageHeight));
                                var memStream = new MemoryStream();
                                newBitmap.Save(memStream, System.Drawing.Imaging.ImageFormat.Jpeg);
                                memStream.Position = 0;
                                fileStream.Close();
                                System.IO.File.Delete(Path.Combine(localResourcePath, imageName));
                                if (upload)
                                {
                                    var url = UploadToBlob(memStream, imageName);
                                    return Json(new { url });
                                }

                                var file = File(memStream, MediaTypeNames.Image.Jpeg);
                                return file;
                            }
                        }

                        return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, string.Concat("Image creation failed for id: ", fileName));
                    }
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "PreviewController: could not de-serialize DealPreviewModel");
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest, ex.Message);
                }
            }

            return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Preview deal model is missing");
        }

        /// <summary>
        /// Uploads to BLOB.
        /// </summary>
        /// <param name="memStream">The memory stream.</param>
        /// <param name="imageName">Name of the image.</param>
        /// <returns>
        /// The image uri
        /// </returns>
        private string UploadToBlob(Stream memStream, string imageName)
        {
            var storageAccount = CloudStorageAccount.Parse(ConfigurationManager.AppSettings.Get("HeroDealImageStorageConnectionString"));
            var client = storageAccount.CreateCloudBlobClient();
            var container = client.GetContainerReference("images");
            var blob = container.GetBlockBlobReference(imageName);
            container.CreateIfNotExists();
            container.SetPermissions(new BlobContainerPermissions
            {
                PublicAccess = BlobContainerPublicAccessType.Blob
            });

            blob.UploadFromStream(memStream);
            return blob.Uri.OriginalString;
        }

        /// <summary>
        /// Creates the image.
        /// </summary>
        /// <param name="localResourcePath">The local resource path.</param>
        /// <param name="fileName">Name of the file.</param>
        /// <returns>
        /// The operation status message
        /// </returns>
        private bool CreateImage(string localResourcePath, string fileName)
        {
            try
            {
                var phantomjsPath = Path.Combine(localResourcePath, "phantomjs.exe");
                var scriptPath = Path.Combine(localResourcePath, "screenshot.js");

                if (Request.Url != null)
                {
                    lock (SyncRoot)
                    {
                        var processStartInfo = new ProcessStartInfo(phantomjsPath, string.Format(" --web-security=false {0} {1} {2}", scriptPath, Path.Combine(localResourcePath, string.Concat(fileName, ".html")), string.Concat(fileName, ".jpg")))
                        {
                            WorkingDirectory = localResourcePath,
                            UseShellExecute = false
                        };

                        var process = Process.Start(processStartInfo);
                        if (process != null)
                        {
                            process.WaitForExit();
                        }
                    }

                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "could not start phantomjs.exe");
                return false;
            }
            finally
            {
                System.IO.File.Delete(Path.Combine(localResourcePath, string.Concat(fileName, ".html")));
            }
        }

        #endregion
    }
}