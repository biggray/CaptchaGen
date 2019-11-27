using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using SkiaSharp;

namespace CaptchaGen
{
    /// <summary>
    /// Generates a captcha image based on the captcha code string given.
    /// </summary>
    public class ImageFactory
    {
        protected static Random RandomGen { get; set; } = new Random();

        /// <summary>
        /// Amount of distortion required.
        /// Default value = 18
        /// </summary>
        public static int Distortion { get; set; } = 15;
        const int HEIGHT = 48;
        const int WIDTH = 120;
        const int FONTSIZE = 20;

        /// <summary>
        /// Background color to be used.
        /// Default value = Color.Wheat
        /// </summary>
        public static SKColor BackgroundColor { get; set; } = SKColors.Wheat;


        /// <summary>
        /// Generates the image with default image properties(150px X 96px) and distortion
        /// </summary>
        /// <param name="captchaCode">Captcha code for which the image has to be generated</param>
        /// <param name="imageFormat">Image format to encode to</param>
        /// <param name="imageQuality">Image quality for encoding</param>
        /// <returns>Generated jpeg image as a MemoryStream object</returns>
        public static Stream GenerateImage(string captchaCode, SKEncodedImageFormat imageFormat = SKEncodedImageFormat.Jpeg, int imageQuality = 80)
            => ImageFactory
                .BuildImage(captchaCode, HEIGHT, WIDTH, FONTSIZE)
                .Encode(imageFormat, imageQuality)
                .AsStream();

        /// <summary>
        /// Generates the image with given image properties
        /// </summary>
        /// <param name="captchaCode">Captcha code for which the image has to be generated</param>
        /// <param name="imageHeight">Height of the image to be generated</param>
        /// <param name="imageWidth">Width of the image to be generated</param>
        /// <param name="fontSize">Font size to be used</param>
        /// <param name="distortion">Distortion required</param>
        /// <param name="imageFormat">Image format to encode to</param>
        /// <param name="imageQuality">Image quality for encoding</param>
        /// <returns>Generated jpeg image as a MemoryStream object</returns>
        public static Stream GenerateImage(
            string captchaCode,
            int imageHeight, int imageWidth, int fontSize, int distortion,
            SKEncodedImageFormat imageFormat = SKEncodedImageFormat.Jpeg, int imageQuality = 80
        ) => ImageFactory
                .BuildImage(captchaCode, imageHeight, imageWidth, fontSize, distortion)
                .Encode(imageFormat, imageQuality)
                .AsStream();


        /// <summary>
        /// Generates the image with given image properties
        /// </summary>
        /// <param name="captchaCode">Captcha code for which the image has to be generated</param>
        /// <param name="imageHeight">Height of the image to be generated</param>
        /// <param name="imageWidth">Width of the image to be generated</param>
        /// <param name="fontSize">Font size to be used</param>
        /// <param name="imageFormat">Image format to encode to</param>
        /// <param name="imageQuality">Image quality for encoding</param>
        /// <returns>Generated jpeg image as a MemoryStream object</returns>
        public static Stream GenerateImage(
            string captchaCode,
            int imageHeight, int imageWidth, int fontSize,
            SKEncodedImageFormat imageFormat = SKEncodedImageFormat.Jpeg, int imageQuality = 80
        ) => ImageFactory
                .BuildImage(captchaCode, imageHeight, imageWidth, fontSize)
                .Encode(imageFormat, imageQuality)
                .AsStream();

        /// <summary>
        /// Actual image generator. Internally used.
        /// </summary>
        /// <param name="captchaCode">Captcha code for which the image has to be generated</param>
        /// <param name="imageHeight">Height of the image to be generated</param>
        /// <param name="imageWidth">Width of the image to be generated</param>
        /// <param name="fontSize">Font size to be used</param>
        /// <param name="distortion">Distortion required</param>
        /// <returns>Generated jpeg image as a MemoryStream object</returns>
        private static SKImage BuildImage(string captchaCode, int imageHeight, int imageWidth, int fontSize, int? distortion = null)
        {
            var imageInfo = new SKImageInfo(imageWidth, imageHeight, SKImageInfo.PlatformColorType, SKAlphaType.Premul);
            using (var plainSkSurface = SKSurface.Create(imageInfo))
            {
                var plainCanvas = plainSkSurface.Canvas;
                plainCanvas.Clear(BackgroundColor);

                using (var paintInfo = new SKPaint())
                {
                    paintInfo.Typeface = SKTypeface.FromFamilyName(null, SKFontStyle.Italic);
                    paintInfo.TextSize = fontSize;
                    paintInfo.Color = SKColors.Gray;
                    paintInfo.IsAntialias = true;

                    var xToDraw = (imageWidth - paintInfo.MeasureText(captchaCode)) / 2;
                    var yToDraw = (imageHeight - fontSize) / 2 + fontSize;
                    plainCanvas.DrawText(captchaCode, xToDraw, yToDraw, paintInfo);
                }
                plainCanvas.Flush();

                using (var captchaSkSurface = SKSurface.Create(imageInfo))
                {
                    var captchaCanvas = captchaSkSurface.Canvas;

                    // distort the image with a wave function
                    int newX, newY;
                    double randomDistortion;
                    var distortionThreshold = 5;
                    var maxDistortion = distortion ?? Distortion;
                    while (
                        (randomDistortion = maxDistortion * RandomGen.NextDouble()) < distortionThreshold
                        && maxDistortion > (distortionThreshold * 2)
                    ) ;
                    if (RandomGen.NextDouble() > 0.5) randomDistortion *= -1;
                    var plainPixmap = plainSkSurface.PeekPixels();
                    for (int y = 0; y < imageHeight; y++)
                    {
                        for (int x = 0; x < imageWidth; x++)
                        {
                            newX = (int)(x + (randomDistortion * Math.Sin(Math.PI * y / 64.0)));
                            newY = (int)(y + (randomDistortion * Math.Cos(Math.PI * x / 64.0)));
                            if (newX < 0 || newX >= imageWidth) newX = 0;
                            if (newY < 0 || newY >= imageHeight) newY = 0;

                            captchaCanvas.DrawPoint(x, y, plainPixmap.GetPixelColor(newX, newY));
                        }
                    }

                    var noisePointCount = (int)(imageWidth * imageHeight * 0.05);
                    var enumerateList = Enumerable.Range(0, noisePointCount);
                    var noiseXList = enumerateList.Select(x => RandomGen.Next(imageWidth)).ToArray();
                    var noiseYList = enumerateList.Select(x => RandomGen.Next(imageHeight)).ToArray();
                    for (var i = 0; i < noisePointCount; i++)
                    {
                        captchaCanvas.DrawPoint(noiseXList[i], noiseYList[i], SKColors.LightGray);
                    }

                    captchaCanvas.Flush();

                    return captchaSkSurface.Snapshot();
                }
            }
        }
    }
}
