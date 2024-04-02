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
        /// <inheritdoc cref="Encoding.GetBytes(string)"/>
        public static string ToSHA256(this string? value)
        {
            if (string.IsNullOrEmpty(value)) return string.Empty;
            return Encoding.UTF8.GetBytes(value).ToSHA256();
        }

        /// <inheritdoc cref="Encoding.GetBytes(string)"/>
        public static byte[] GetBytes(this string? value)
        {
            return string.IsNullOrEmpty(value) ? [] : Encoding.UTF8.GetBytes(value);
        }

        /// <summary>
        /// Return String Is null or empty.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>A bool.</returns>
        public static bool IsNullOrEmpty(this string? value)
        {
            return string.IsNullOrEmpty(value);
        }

        /// <summary>
        /// Return String Is null or white space.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>A bool.</returns>
        public static bool IsNullOrWhiteSpace(this string? value)
        {
            return string.IsNullOrWhiteSpace(value);
        }
    }
}
