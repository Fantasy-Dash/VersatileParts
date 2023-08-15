using System.Text.Json.Nodes;

namespace VP.Common.Helpers
{
    /// <summary>
    /// 文件帮助类
    /// </summary>
    public static class FileHelper
    {
        /// <summary>
        /// 读取文件或文件流 返回指定类型数据
        /// </summary>
        /// <typeparam name="T">输出类型</typeparam>
        /// <param name="fileStream">文件流</param>
        /// <returns>指定类型数据</returns>
        /// <inheritdoc cref="StreamReader(Stream)"/>
        /// <inheritdoc cref="StreamReader.ReadToEnd()"/>
        /// <inheritdoc cref="JsonNode.Parse(Stream, JsonNodeOptions?, System.Text.Json.JsonDocumentOptions)"/>
        public static T? ReadToType<T>(FileStream fileStream)
        {
            fileStream.Position=0;
            using var streamReader = new StreamReader(fileStream);
            var a = typeof(T);
            var b = typeof(string);
            if (typeof(T) == typeof(string))
                return (T?)(object?)streamReader.ReadToEnd();
            if (typeof(T) == typeof(JsonNode))
                return (T?)(object?)JsonNode.Parse(fileStream);
            object data = streamReader.ReadToEnd();
            return (T)data;
        }

        /// <param name="path">文件路径</param>
        /// <inheritdoc cref="ReadToType{T}(FileStream)"/>
        /// <inheritdoc cref="FileStream(string,FileMode)"/>
        public static T? ReadToType<T>(string path)
        {
            if (File.Exists(path))
                return ReadToType<T>(new FileStream(path, FileMode.OpenOrCreate));
            return default;
        }


        /// <summary>
        /// 文件是否被占用
        /// </summary>
        /// <param name="filePath">文件路径</param>
        /// <returns>文件被占用则返回<c>True</c></returns>
        /// <inheritdoc cref="FileStream(string,FileMode,FileAccess,FileShare)"/>
        public static bool IsFileUsing(string filePath)
        {
            if (!File.Exists(filePath))
                return false;

            try
            {
                using var s = new FileStream(filePath, FileMode.Open, FileAccess.ReadWrite, FileShare.None);
            }
            catch (IOException)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 等待文件释放
        /// </summary>
        /// <param name="filePath">文件路径</param>
        /// <param name="ignoreFileNotFound">跳过找不到的文件</param>
        /// <returns>可等待任务</returns>
        /// <exception cref="FileNotFoundException">文件不存在: <paramref name="filePath"/> </exception>
        /// <inheritdoc cref="FileStream(string,FileMode,FileAccess,FileShare)"/>
        public static async Task WaitForFileReleaseAsync(string filePath, bool ignoreFileNotFound = true)
        {
            // 确保文件存在

            if (!File.Exists(filePath))
            {
                if (!ignoreFileNotFound)
                    throw new FileNotFoundException("文件不存在", filePath);
                else return;
            }

            // 等待文件解除占用
            while (true)
            {
                try
                {
                    using var s = new FileStream(filePath, FileMode.Open, FileAccess.ReadWrite, FileShare.None);
                    break;
                }
                catch (IOException)
                {
                    // 文件仍然被占用，等待一段时间
                    await Task.Delay(200).ConfigureAwait(false);
                }
            }
        }

        /// <summary>
        /// 获取文件夹中所有文件
        /// </summary>
        /// <param name="path">目标路径</param>
        /// <param name="ignorePath">要忽略的目录</param>
        /// <param name="ignoreFile">要忽略的文件</param>
        /// <returns>文件夹中的所有文件 包含子文件夹中的文件</returns>
        /// <inheritdoc cref="Directory.GetFiles(string, string, EnumerationOptions)"/>
        /// <inheritdoc cref="Path.GetRelativePath(string, string)"/>
        /// <inheritdoc cref="ParallelEnumerable.ForAll{TSource}(ParallelQuery{TSource}, Action{TSource})"/>
        public static IEnumerable<string> GetDirectoryFile(string path, IEnumerable<string>? ignorePath = null, IEnumerable<string>? ignoreFile = null)
        {
            var ret = new List<string>();
            var files = Directory.GetFiles(path, "*", SearchOption.AllDirectories);
            files.AsParallel().WithDegreeOfParallelism(20).ForAll(file =>
            {
                var relativePath = Path.GetRelativePath(path, file);
                var isSkip = false;
                if (relativePath.IndexOf(@"\") > -1 && ignorePath != null)
                    foreach (var item in ignorePath)
                        if (relativePath.StartsWith(item))
                            isSkip = true;
                if (!isSkip && ignoreFile != null)
                    foreach (var item in ignoreFile)
                        if (relativePath.EndsWith(item))
                            isSkip = true;
                if (!isSkip)
                    ret.Add(file);
            });
            return ret;
        }

        /// <summary>
        /// 复制目录文件
        /// </summary>
        /// <param name="sourcePath">源目录</param>
        /// <param name="targetPath">目标目录</param>
        /// <param name="ignorePath">忽略的子目录</param>
        /// <param name="ignoreFile">忽略的文件</param>
        /// <inheritdoc cref="Directory.CreateDirectory(string)" path="/*[not(name()='returns')]" />
        /// <inheritdoc cref="Path.GetRelativePath(string, string)" path="/*[not(name()='returns')]" />
        /// <inheritdoc cref="ParallelEnumerable.ForAll{TSource}(ParallelQuery{TSource}, Action{TSource})" path="/*[not(name()='returns')]" />
        public static void CopyDirectoryFile(string sourcePath, string targetPath, IEnumerable<string>? ignorePath, IEnumerable<string>? ignoreFile)
        {
            Directory.CreateDirectory(sourcePath);
            Directory.CreateDirectory(targetPath);
            var files = Directory.GetFiles(sourcePath, "*", SearchOption.AllDirectories);
            files.AsParallel().WithDegreeOfParallelism(20).ForAll(file =>
            {
                var relativePath = Path.GetRelativePath(sourcePath, file);
                var isSkip = false;
                if (relativePath.IndexOf(@"\") > -1 && ignorePath != null)
                    foreach (var item in ignorePath)
                        if (relativePath.StartsWith(item))
                            isSkip = true;
                if (!isSkip && ignoreFile != null)
                    foreach (var item in ignoreFile)
                        if (relativePath.EndsWith(item))
                            isSkip = true;
                if (!isSkip)
                {
                    if (relativePath.IndexOf("\\")>-1)
                        Directory.CreateDirectory(new FileInfo(Path.Combine(targetPath, relativePath)).DirectoryName!);
                    using var sourceStream = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                    using var destStream = new FileStream(Path.Combine(targetPath, relativePath), FileMode.Create, FileAccess.Write, FileShare.ReadWrite);
                    sourceStream.CopyTo(destStream);
                }
            });
        }

        /// <summary>
        /// 替换目录文件
        /// </summary>
        /// <param name="sourcePath">源目录</param>
        /// <param name="targetPath">目标目录</param>
        /// <param name="ignorePath">忽略的子目录</param>
        /// <param name="ignoreFile">忽略的文件</param>
        /// <inheritdoc cref="Directory.CreateDirectory(string)" path="/*[not(name()='returns')]" />
        /// <inheritdoc cref="Path.GetRelativePath(string, string)" path="/*[not(name()='returns')]" />
        /// <inheritdoc cref="ParallelEnumerable.ForAll{TSource}(ParallelQuery{TSource}, Action{TSource})" path="/*[not(name()='returns')]" />
        public static void ReplaceDirectoryFile(string sourcePath, string targetPath, IEnumerable<string>? ignorePath, IEnumerable<string>? ignoreFile)
        {
            Directory.CreateDirectory(sourcePath);
            Directory.CreateDirectory(targetPath);
            var files = Directory.GetFiles(sourcePath, "*", SearchOption.AllDirectories);
            files.AsParallel().WithDegreeOfParallelism(20).ForAll(file =>
            {
                var relativePath = Path.GetRelativePath(sourcePath, file);
                var isSkip = false;
                if (relativePath.IndexOf(@"\") > -1 && ignorePath != null)
                    foreach (var item in ignorePath)
                        if (relativePath.StartsWith(item))
                            isSkip = true;
                if (!isSkip && ignoreFile != null)
                    foreach (var item in ignoreFile)
                        if (relativePath.EndsWith(item))
                            isSkip = true;
                if (!isSkip)
                {
                    WaitForFileReleaseAsync(Path.Combine(targetPath, Path.GetFileName(file))).ConfigureAwait(false).GetAwaiter().GetResult();
                    File.Copy(file, Path.Combine(targetPath, Path.GetFileName(file)), true);
                    File.Delete(file);
                }
            });
        }

        /// <summary>
        /// 删除目录中指定的文件
        /// </summary>
        /// <param name="path">源目录</param>
        /// <param name="needDeleteFilePath">需要删除的文件路径</param>
        /// <param name="ignorePath">忽略的子目录</param>
        /// <param name="ignoreFile">忽略的文件</param>
        /// <inheritdoc cref="Directory.CreateDirectory(string)" path="/*[not(name()='returns')]" />
        /// <inheritdoc cref="Path.GetFullPath(string)" path="/*[not(name()='returns')]" />
        /// <inheritdoc cref="ParallelEnumerable.ForAll{TSource}(ParallelQuery{TSource}, Action{TSource})" path="/*[not(name()='returns')]" />
        public static void DeletedirectoryFile(string path, IEnumerable<string> needDeleteFilePath, IEnumerable<string>? ignorePath, IEnumerable<string>? ignoreFile)
        {
            Directory.CreateDirectory(path);
            needDeleteFilePath.AsParallel().WithDegreeOfParallelism(20).ForAll(deleteFilePath =>
            {
                var filePath = Path.GetFullPath(Path.Combine(path, deleteFilePath));
                var relativePath = Path.GetRelativePath(path, filePath);
                var isSkip = false;
                if (relativePath.IndexOf(@"\") > -1 && ignorePath != null)
                    foreach (var item in ignorePath)
                        if (relativePath.StartsWith(item))
                            isSkip = true;
                if (!isSkip && ignoreFile != null)
                    foreach (var item in ignoreFile)
                        if (relativePath.EndsWith(item))
                            isSkip = true;
                if (!isSkip)
                {
                    WaitForFileReleaseAsync(Path.Combine(path, Path.GetFileName(filePath)))
                    .ConfigureAwait(false).GetAwaiter().GetResult();
                    File.Delete(filePath);
                }
            });
        }
    }
}
