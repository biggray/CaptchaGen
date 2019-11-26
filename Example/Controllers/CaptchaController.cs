using CaptchaGen;
using Example.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace Example.Controllers
{

    /// <summary>
    /// Captcha Controller class
    /// </summary>
    public class CaptchaController : ControllerBase
    {
        /// <summary>
        /// Returns a captcha string as json object.
        /// </summary>
        /// <returns>CaptchaCode as json object</returns>
        public CaptchaCode GetCaptchaCode()
        {
            return new CaptchaCode() { sessionString = CaptchaCodeFactory.GenerateCaptchaCode(8) };
        }
    }
}
