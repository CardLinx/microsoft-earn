//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace OffersEmail.FrontEnd.Controllers
{
    using System;
    using System.Drawing;
    using System.Drawing.Drawing2D;
    using System.Drawing.Imaging;
    using System.IO;
    using System.Net;
    using System.Net.Http;
    using System.Net.Mime;
    using System.Web.Mvc;

    /// <summary>
    /// Image controller to resize images
    /// </summary>
    public class ImageController : Controller
    {
        /// <summary>
        /// Gets the resized image
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="size">The size.</param>
        /// <returns>
        /// The image
        /// </returns>
        public ActionResult Get(string url, string size)
        {
            if (string.IsNullOrWhiteSpace(url))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "image url is missing");
            }

            int height, width;
            if (string.IsNullOrWhiteSpace(size))
            {
                width = 560;
                height = 450;
            }
            else
            {
                var dimension = size.Split(new[] { ',' }, 2, StringSplitOptions.RemoveEmptyEntries);

                if (dimension.Length == 2 && int.TryParse(dimension[0], out width) &&
                    int.TryParse(dimension[1], out height))
                {
                    if (width < 0 || height < 0)
                    {
                        return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Size should be positive integer.");
                    }
                }
                else
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Unknown size format.");
                }
            }

            return CreateFileResult(url, width, height);
        }

        /// <summary>
        /// Resizes the image.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <returns>The resized image</returns>
        private static Image ResizeImage(Image source, int width, int height)
        {
            int sourceWidth = source.Width;
            int sourceHeight = source.Height;

            float scaleWidth = width / (float)sourceWidth;
            float scaleHeigth = height / (float)sourceHeight;

            int destWidth;
            int destHeight;

            if (scaleWidth > scaleHeigth)
            {
                // scale width and leave gaps top/bottom
                destWidth = width;
                destHeight = Convert.ToInt32(sourceHeight * scaleWidth);
            }
            else
            {
                // scale height and leave gaps left/right
                destHeight = height;
                destWidth = Convert.ToInt32(sourceWidth * scaleHeigth);
            }

            // Create a new bitmap object.
            var bitmap = new Bitmap(destWidth, destHeight, PixelFormat.Format24bppRgb);

            // set resolution of bitmap.
            bitmap.SetResolution(source.HorizontalResolution, source.VerticalResolution);

            // create a graphics object and set quality of graphics.
            using (var graphics = Graphics.FromImage(bitmap))
            {
                graphics.InterpolationMode = InterpolationMode.Default;
                graphics.SmoothingMode = SmoothingMode.Default;
                graphics.PixelOffsetMode = PixelOffsetMode.Default;
                graphics.CompositingQuality = CompositingQuality.Default;
                graphics.DrawImage(source, 0, 0, destWidth, destHeight);

                int x = (bitmap.Width - width) / 2;
                int y = (bitmap.Height - height) / 2;
                var cropArea = new Rectangle(x, y, width, height);
                return bitmap.Clone(cropArea, bitmap.PixelFormat);
            }
        }

        /// <summary>
        /// Creates the file result.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <returns>
        /// The action result for cdn controller
        /// </returns>
        private static ActionResult CreateFileResult(string url, int width, int height)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    var response = client.GetStreamAsync(url);

                    if (response.Result != null)
                    {
                        var img = Image.FromStream(response.Result);

                        if (width > 0 && height > 0)
                        {
                            img = ResizeImage(img, width, height);
                        }

                        // copy the object to a new instance of bitmap is a hack for tackling error while working with resized image and saving it to stream.
                        using (var tempImage = new Bitmap(img))
                        {
                            img.Dispose();
                            var outputStream = new MemoryStream();
                            tempImage.Save(outputStream, ImageFormat.Jpeg);
                            outputStream.Position = 0;
                            return new FileStreamResult(outputStream, MediaTypeNames.Image.Jpeg);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, ex.Message);
            }

            return new HttpStatusCodeResult(HttpStatusCode.NotFound);
        }
    }
}