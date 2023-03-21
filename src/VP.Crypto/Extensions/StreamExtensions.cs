using VP.Common.Extensions;

namespace VP.Crypto.Extensions
{
    /// <summary>
    /// 流扩展
    /// </summary>
    public static class StreamExtensions
    {
        /// <param name="key">编码使用的密钥</param>
        /// <param name="value">要编码的 <see cref="Stream"/></param>
        /// <inheritdoc cref="ByteArrayExtensions.ToHMACSHA256(byte[], byte[])"/>
        public static string ToHMACSHA256(this Stream value, byte[] key)
        {
            if (value.Length == 0) return string.Empty;
            return value.GetAllBytes().ToHMACSHA256(key);
        }

    }
}
