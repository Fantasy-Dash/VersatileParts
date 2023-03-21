using System.Security.Cryptography;
using System.Text;

namespace VP.Crypto.Extensions
{
    /// <summary>
    /// 字节数组扩展扩展
    /// </summary>
    public static class ByteArrayExtensions
    {
        /// <summary>
        /// 根据 <paramref name="key"/> 将 <paramref name="value"/> 编码成 <see cref="SHA256"/> 格式
        /// </summary>
        /// <param name="value">要编码的<see cref="byte"/>数组</param>
        /// <param name="key">HMAC密钥</param>
        /// <returns>使用HMAC方式编码的16进制SHA256字符串</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="key" /> 或 <paramref name="value" /> 是 <see langword="null" />
        /// </exception>
        /// <inheritdoc cref="HMACSHA256.HashData(byte[], byte[])"/>
        public static string ToHMACSHA256(this byte[] value, byte[] key)
        {
            var hashBytes = HMACSHA256.HashData(key, value);
            var sb = new StringBuilder();
            foreach (var hashByte in hashBytes)
                sb.Append(hashByte.ToString("X2"));
            return sb.ToString();
        }
    }
}
