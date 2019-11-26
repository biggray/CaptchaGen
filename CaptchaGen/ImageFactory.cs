using System;
using System.Drawing;
using System.IO;
using SkiaSharp;

namespace CaptchaGen
{
    /// <summary>
    /// Generates a captcha image based on the captcha code string given.
    /// </summary>
    public static class ImageFactory
    {
        /// <summary>
        /// Amount of distortion required.
        /// Default value = 18
        /// </summary>
        public static int Distortion { get; set; } = 18;
        const int HEIGHT = 96;
        const int WIDTH = 150;
        const string FONTFAMILY = "Arial";
        const int FONTSIZE = 25;

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
                .BuildImage(captchaCode, HEIGHT, WIDTH, FONTSIZE, Distortion)
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
                .BuildImage(captchaCode, imageHeight, imageWidth, fontSize, Distortion)
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
        private static SKImage BuildImage(string captchaCode, int imageHeight, int imageWidth, int fontSize, int distortion)
        {
            var imageInfo = new SKImageInfo(imageWidth, imageHeight, SKImageInfo.PlatformColorType, SKAlphaType.Premul);
            using (var plainSkSurface = SKSurface.Create(imageInfo))
            {
                var plainCanvas = plainSkSurface.Canvas;
                plainCanvas.Clear(BackgroundColor);

                using (var paintInfo = new SKPaint())
                {
                    paintInfo.Typeface = SKTypeface.FromFamilyName(FONTFAMILY, SKFontStyle.Italic);
                    paintInfo.TextSize = fontSize;
                    paintInfo.Color = SKColors.Gray;

                    plainCanvas.DrawText(captchaCode, 8.4F, 20.4F, paintInfo);
                }

                using (var captchaSkSurface = SKSurface.Create(imageInfo))
                {
                    var captchaCanvas = captchaSkSurface.Canvas;

                    // distort the image with a wave function
                    int newX, newY;
                    var plainPixmap = plainSkSurface.PeekPixels();
                    for (int y = 0; y < imageHeight; y++)
                    {
                        for (int x = 0; x < imageWidth; x++)
                        {
                            newX = (int)(x + (distortion * Math.Sin(Math.PI * y / 64.0)));
                            newY = (int)(y + (distortion * Math.Cos(Math.PI * x / 64.0)));
                            if (newX < 0 || newX >= imageWidth) newX = 0;
                            if (newY < 0 || newY >= imageHeight) newY = 0;

                            captchaCanvas.DrawPoint(x, y, plainPixmap.GetPixelColor(newX, newY));
                        }
                    }

                    return captchaSkSurface.Snapshot();
                }
            }
        }
    }
}
