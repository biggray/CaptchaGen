using CaptchaGen;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace Example.Controllers
{
    /// <summary>
    /// Image controller class
    /// </summary>
    public class ImageController : ControllerBase
    {
        /// <summary>
        /// Returns a response message as an image.
        /// </summary>
        /// <param name="id">Captcha string for which the image is to be generated</param>
        /// <returns>HttpResponseMessage with "image/jpeg" header</returns>
        public IActionResult Get(string id)
        {
            var imageStream = ImageFactory.GenerateImage(id);
            imageStream.Position = 0;
            return File(imageStream, "image/jpeg");

        }
    }
}
