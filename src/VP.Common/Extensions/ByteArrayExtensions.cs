using System.Security.Cryptography;
using System.Text;

namespace VP.Common.Extensions
{
    /// <summary>
    /// 字节扩展
    /// </summary>
    public static class ByteArrayExtensions
    {
        /// <summary>
        /// 将 <paramref name="value"/> 编码成 <see cref="SHA256"/> 格式
        /// </summary>
        /// <param name="value">要编码的<see cref="byte"/>数组</param>
        /// <returns>16进制SHA256字符串</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="key" /> 是 <see langword="null" />
        /// </exception>
        /// <inheritdoc cref="SHA256.HashData(byte[])"/>
        public static string ToSHA256(this byte[] value)
        {
            var hashBytes = SHA256.HashData(value);
            var sb = new StringBuilder();
            foreach (var hashByte in hashBytes)
                sb.Append(hashByte.ToString("X2"));
            return sb.ToString();
        }
    }
}
