using CaptchaGen;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using Xunit;

namespace CaptchaGen.Tests
{
    public class ImageFactoryTests
    {
        [Fact]
        public void GenerateImageTest()
        {
            string testString = "fEwS21";
            var stream = ImageFactory.GenerateImage(testString);
            stream.ShouldNotBeNull();
            Bitmap image = Image.FromStream(stream) as Bitmap;
            image.Width.ShouldBe(150);
        }

        [Fact]
        public void GenerateImageWithImageAttributesTest()
        {
            string testString = "fEwS21";
            int height = 135, width = 250;
            var stream = ImageFactory.GenerateImage(testString, height, width, 23);
            stream.ShouldNotBeNull();
            Bitmap image = Image.FromStream(stream) as Bitmap;
            image.Width.ShouldBe(width);
            image.Height.ShouldBe(height);
        }
    }
}
