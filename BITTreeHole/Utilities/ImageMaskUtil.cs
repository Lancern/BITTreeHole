using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Http;

namespace BITTreeHole.Utilities
{
    /// <summary>
    /// 提供图片掩码的支持功能。
    /// </summary>
    public static class ImageMaskUtil
    {
        /// <summary>
        /// 检查给定的图片掩码是否合法。
        /// </summary>
        /// <param name="mask">要检查的图片掩码。</param>
        /// <returns>给定的图片掩码是否合法。</returns>
        private static bool IsValidMask(string mask)
        {
            if (mask.Length > 9)
            {
                return false;
            }

            var seen = new bool[9];
            foreach (var c in mask)
            {
                if (!char.IsDigit(c))
                {
                    return false;
                }

                var d = c - '0';
                if (d == 9)
                {
                    return false;
                }

                if (seen[d])
                {
                    return false;
                }

                seen[d] = true;
            }

            return true;
        }
        
        /// <summary>
        /// 检查给定的图片掩码是否合法，若不合法，抛出 <see cref="InvalidImageMaskException"/> 异常。
        /// </summary>
        /// <param name="mask">要检查的图片掩码。</param>
        /// <exception cref="InvalidImageMaskException">给定的图片掩码不合法。</exception>
        private static void EnsureValidMask(string mask)
        {
            if (!IsValidMask(mask))
                throw new InvalidImageMaskException();
        }
        
        /// <summary>
        /// 将给定的图片掩码与图片文件集合进行绑定。
        /// </summary>
        /// <param name="mask">图片掩码。</param>
        /// <param name="imageFiles">图片文件。</param>
        /// <returns>图片 ID 到图片文件的字典。</returns>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="mask"/>为null
        ///     或
        ///     <paramref name="imageFiles"/>为nul
        /// </exception>
        /// <exception cref="InvalidImageMaskException">图片掩码无效 或 图片掩码长度与图像数量不一致。</exception>
        public static Dictionary<int, IFormFile> ZipImageIdMask(string mask, IReadOnlyCollection<IFormFile> imageFiles)
        {
            if (mask == null)
                throw new ArgumentNullException(nameof(mask));
            if (imageFiles == null)
                throw new ArgumentNullException(nameof(imageFiles));

            EnsureValidMask(mask);
            if (imageFiles.Count != mask.Length)
                throw new InvalidImageMaskException("掩码长度与给定的图像数量不一致。");

            return mask.Select(ch => ch - '0')
                       .Zip(imageFiles, (id, file) => (id, file))
                       .ToDictionary(pair => pair.Item1, pair => pair.Item2);
        }

        /// <summary>
        /// 从给定的图片掩码抽取图片索引。
        /// </summary>
        /// <param name="mask">图片掩码。</param>
        /// <returns>从图片掩码中抽取的图片索引。</returns>
        /// <exception cref="ArgumentNullException"><paramref name="mask"/>为null</exception>
        /// <exception cref="InvalidImageMaskException">图片掩码无效。</exception>
        public static int[] ExtractImageIdFromMask(string mask)
        {
            if (mask == null)
                throw new ArgumentNullException(nameof(mask));
            
            EnsureValidMask(mask);

            return mask.Select(ch => ch - '0')
                       .ToArray();
        }
    }
}
