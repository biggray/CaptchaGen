using CaptchaGen;
using Shouldly;
using System;
using Xunit;

namespace CaptchaGen.Tests
{
    public class CaptchaCodeFactoryTests
    {
        [Fact]
        public void GenerateCaptchaCodeTest()
        {
            string captcha = CaptchaGen.CaptchaCodeFactory.GenerateCaptchaCode(6);
            captcha.Length.ShouldBe(6);
        }

        [Fact]
        public void GenerateCaptchaCodeTest1()
        {
            Should.Throw<ArgumentException>(
                () =>
                {
                    CaptchaGen.CaptchaCodeFactory.GenerateCaptchaCode(-2);
                }
            ).Message.ShouldBe("Error is not thrown for an illegal size");
        }
    }
}
