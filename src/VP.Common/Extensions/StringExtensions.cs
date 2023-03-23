using System.Text;

namespace VP.Common.Extensions
{
    /// <summary>
    /// 字符串扩展
    /// </summary>
    public static class StringExtensions
    {
        /// <param name="value">要编码的 <see cref="string"/></param>
        /// <inheritdoc cref="ByteArrayExtensions.ToSHA256(byte[])"/>
        public static string ToSHA256(this string? value)
        {
            if (string.IsNullOrEmpty(value)) return string.Empty;
            return Encoding.UTF8.GetBytes(value).ToSHA256();
        }

        /// <inheritdoc cref="Encoding.GetBytes(string)"/>
        public static byte[] GetBytes(this string? value)
        {
            if (string.IsNullOrEmpty(value)) return Array.Empty<byte>();
            return Encoding.UTF8.GetBytes(value);
        }
    }
}
