using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using BITTreeHole.Utilities;
using Microsoft.AspNetCore.Http;
using Xunit;

namespace BITTreeHole.Test.UnitTest
{
    public class ImageMaskUtilTests
    {
        private sealed class MockFormFile : IFormFile
        {
            public Stream OpenReadStream()
            {
                throw new NotImplementedException();
            }

            public void CopyTo(Stream target)
            {
                throw new NotImplementedException();
            }

            public Task CopyToAsync(Stream target, CancellationToken cancellationToken = new CancellationToken())
            {
                throw new NotImplementedException();
            }

            public string ContentType { get; }
            public string ContentDisposition { get; }
            public IHeaderDictionary Headers { get; }
            public long Length { get; }
            public string Name { get; }
            public string FileName { get; }
        }
        
        [Fact]
        public void NullMaskOrFiles()
        {
            Assert.Throws<ArgumentNullException>(() => ImageMaskUtil.ZipImageIdMask(null, null));
            Assert.Throws<ArgumentNullException>(() => ImageMaskUtil.ZipImageIdMask(null, new List<IFormFile>()));
            Assert.Throws<ArgumentNullException>(() => ImageMaskUtil.ZipImageIdMask(string.Empty, null));
        }
        
        [Fact]
        public void InvalidMask()
        {
            Assert.Throws<InvalidImageMaskException>(
                () => ImageMaskUtil.ZipImageIdMask("0123abc", new List<IFormFile>()));
            Assert.Throws<InvalidImageMaskException>(
                () => ImageMaskUtil.ZipImageIdMask("0129", new List<IFormFile>()));
        }

        [Fact]
        public void InconsistentNumberOfImages()
        {
            Assert.Throws<InvalidImageMaskException>(
                () => ImageMaskUtil.ZipImageIdMask("01", new IFormFile[1]));
        }

        [Fact]
        public void EmptyMask()
        {
            Assert.Empty(ImageMaskUtil.ZipImageIdMask(string.Empty, new List<IFormFile>()));
        }

        [Fact]
        public void ValidMask()
        {
            var formFiles = new IFormFile[]
            {
                new MockFormFile(), new MockFormFile(), new MockFormFile()
            };
            var result = ImageMaskUtil.ZipImageIdMask("351", formFiles);
            
            Assert.Equal(3, result.Count);
            Assert.Same(result[1], formFiles[2]);
            Assert.Same(result[3], formFiles[0]);
            Assert.Same(result[5], formFiles[1]);
        }
    }
}
