using System.Text;
using VP.Common.Extensions;

namespace VP.Crypto.Extensions
{
    /// <summary>
    /// 字符串扩展
    /// </summary>
    public static class StringExtensions
    {
        /// <inheritdoc cref="ByteArrayExtensions.ToHMACSHA256(byte[], byte[])"/>>
        /// <param name="value">要编码的 <see cref="string"/></param>
        public static string ToHMACSHA256(this string? value, string key)
        {
            if (string.IsNullOrEmpty(value)) return string.Empty;
            return Encoding.UTF8.GetBytes(value).ToHMACSHA256(key.GetBytes());
        }
    }
}
