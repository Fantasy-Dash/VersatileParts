namespace VP.Common.Extensions
{
    /// <summary>
    /// 流扩展
    /// </summary>
    public static class StreamExtensions
    {
        /// <summary>
        /// 将流中的所有数据都读取到 <see cref="byte[]"/> 中
        /// </summary>
        /// <param name="stream">支持Seek的流</param>
        /// <returns><see cref="byte[]"/></returns>
        /// <exception cref="NotSupportedException">流不能设置位置</exception>
        public static byte[] GetAllBytes(this Stream stream)
        {
            using var memoryStream = new MemoryStream();
            if (!stream.CanSeek) throw new NotSupportedException("Stream Not Supported Seeking");
            stream.Position = 0;
            stream.CopyTo(memoryStream);
            return memoryStream.ToArray();
        }

        /// <param name="value">要进行Hash运算的流</param>
        /// <inheritdoc cref="ByteArrayExtensions.ToSHA256(byte[])"/>
        public static string ToSHA256(this Stream value)
        {
            if (value.Length == 0) return string.Empty;
            return value.GetAllBytes().ToSHA256();
        }
    }
}
