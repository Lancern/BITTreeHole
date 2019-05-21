using System;
using BITTreeHole.Utilities;
using Xunit;

namespace BITTreeHole.Test.UnitTest
{
    public class ImageMaskExtractionTests
    {
        [Fact]
        public void NullMask()
        {
            Assert.Throws<ArgumentNullException>(() => ImageMaskUtil.ExtractImageIdFromMask(null));
        }

        [Fact]
        public void InvalidMask()
        {
            Assert.Throws<InvalidImageMaskException>(() => ImageMaskUtil.ExtractImageIdFromMask("0123abc"));
            Assert.Throws<InvalidImageMaskException>(() => ImageMaskUtil.ExtractImageIdFromMask("01239"));
            Assert.Throws<InvalidImageMaskException>(() => ImageMaskUtil.ExtractImageIdFromMask("01421"));
        }

        [Fact]
        public void EmptyMask()
        {
            Assert.Empty(ImageMaskUtil.ExtractImageIdFromMask(string.Empty));
        }

        [Fact]
        public void ValidMask()
        {
            Assert.Equal(new [] { 0, 8, 1, 5 }, ImageMaskUtil.ExtractImageIdFromMask("0815"));
        }
    }
}
