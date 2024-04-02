using System.IO;
using System.Runtime.InteropServices;
using System.Text.Json.Nodes;
using VP.Common.Extensions;

namespace VP.Common.Helpers
{
    /// <summary>
    /// 文件相关帮助类
    /// </summary>
    public static class FileHelper
    {

        /// <summary>
        /// Read <paramref name="filePath"/> File,Read to String
        /// </summary>
        /// <param name="filePath">FilePath</param>
        /// <returns>FileStream reads String</returns>
        /// <inheritdoc cref="FileStream(string,FileMode)"/>
        /// <inheritdoc cref="StreamReader(Stream)"/>
        /// <inheritdoc cref="StreamReader.ReadToEndAsync()"/>
        public static async Task<string?> ReadToStringAsync(string filePath, CancellationToken cancellationToken)
        {
            //todo test
            using var streamReader = new StreamReader(new FileStream(filePath, FileMode.Open));
            return await streamReader.ReadToEndAsync(cancellationToken);
        }

        public static async Task<string?> ReadToStringAsync(string filePath)
        {
            return await ReadToStringAsync(filePath, CancellationToken.None);
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
            catch
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
        public static async Task WaitForFileReleaseAsync(string filePath, CancellationToken cancellationToken, int checkMillisecondsInterval = 1000, bool ignoreFileNotFound = true)
        {
            if (!File.Exists(filePath))
            {
                if (!ignoreFileNotFound)
                    throw new FileNotFoundException("File Not Found", filePath);
                else return;
            }

            while (true)
            {
                try
                {
                    using var s = new FileStream(filePath, FileMode.Open, FileAccess.ReadWrite, FileShare.None);
                    break;
                }
                catch (IOException)
                {
                    await Task.Delay(checkMillisecondsInterval, cancellationToken);
                }
            }
        }

        /// <summary>
        /// 获取文件夹中所有文件
        /// </summary>
        /// <param name="path">目标路径</param>
        /// <param name="basePath">源目录 用于相对路径拼接</param>
        /// <param name="ignorePathList">要忽略的目录</param>
        /// <param name="ignoreFileList">要忽略的文件</param>
        /// <returns>文件夹中的所有文件 包含子文件夹中的文件</returns>
        /// <inheritdoc cref="Directory.GetFiles(string, string, EnumerationOptions)"/>
        /// <inheritdoc cref="Path.GetFullPath(string, string)"/>
        public static IEnumerable<string> GetDirectoryFile(string path, string? basePath = null, ICollection<string>? ignorePathList = null, ICollection<string>? ignoreFileList = null)
        {
            IEnumerable<string> files = Directory.GetFiles(path, "*.*", SearchOption.AllDirectories);

            if (basePath!=null)
            {
                ignorePathList=ignorePathList?.Select(r => Path.GetFullPath(r, basePath)).ToList();
                ignoreFileList=ignoreFileList?.Select(r => Path.GetFullPath(r, basePath)).ToList();
            }
            else
            {
                ignorePathList=ignorePathList?.Select(Path.GetFullPath).ToList();
                ignoreFileList=ignoreFileList?.Select(Path.GetFullPath).ToList();
            }

            files=files.IfWhere(ignorePathList!=null&&ignorePathList.Count>0, r => !ignorePathList!.Any(p => r.StartsWith(p, StringComparison.OrdinalIgnoreCase)));
            files=files.IfWhere(ignoreFileList!=null&&ignoreFileList.Count>0, r => !ignoreFileList!.Any(p => r.Contains(p, StringComparison.OrdinalIgnoreCase)));

            return files;
        }

        /// <summary>
        /// 复制目录文件
        /// </summary>
        /// <param name="sourcePath">源目录</param>
        /// <param name="targetPath">目标目录</param>
        /// <param name="basePath">源目录 用于相对路径拼接</param>
        /// <param name="ignorePathList">忽略的子目录</param>
        /// <param name="ignoreFile">忽略的文件</param>
        /// <inheritdoc cref="Directory.CreateDirectory(string)" path="/*[not(name()='returns')]" />
        /// <inheritdoc cref="Path.GetRelativePath(string, string)" path="/*[not(name()='returns')]" />
        /// <inheritdoc cref="File.Copy(string, string)" path="/*[not(name()='returns')]" />
        public static Task CopyDirectoryFileAsync(string sourcePath, string targetPath, string? basePath = null, ICollection<string>? ignorePathList = null, ICollection<string>? ignoreFileList = null, bool isOverwrite = false)
        {
            Directory.CreateDirectory(sourcePath);
            Directory.CreateDirectory(targetPath);
            List<string> files = GetDirectoryFile(sourcePath, basePath, ignorePathList, ignoreFileList).ToList();
            for (int i = 0; i < files.Count; i++)
            {
                var targetFilePath = Path.Combine(targetPath, Path.GetRelativePath(sourcePath, files[i]));
                Directory.CreateDirectory(targetFilePath);
                File.Copy(files[i], targetFilePath, isOverwrite);
            }
            return Task.CompletedTask;
        }

        /// <summary>
        /// 剪切目录文件
        /// </summary>
        /// <param name="sourcePath">源目录</param>
        /// <param name="targetPath">目标目录</param>
        /// <param name="basePath">源目录 用于相对路径拼接</param>
        /// <param name="ignorePathList">忽略的子目录</param>
        /// <param name="ignoreFileList">忽略的文件</param>
        /// <inheritdoc cref="Directory.CreateDirectory(string)" path="/*[not(name()='returns')]" />
        /// <inheritdoc cref="Path.GetRelativePath(string, string)" path="/*[not(name()='returns')]" />
        /// <inheritdoc cref="WaitForFileReleaseAsync" path="/*[not(name()='returns')]" />
        public static async Task CutDirectoryFileAsync(string sourcePath, string targetPath, string basePath, ICollection<string>? ignorePathList = null, ICollection<string>? ignoreFileList = null, bool isOverwrite = false)
        {
            Directory.CreateDirectory(sourcePath);
            Directory.CreateDirectory(targetPath);
            List<string> files = GetDirectoryFile(sourcePath, basePath, ignorePathList, ignoreFileList).ToList();
            for (int i = 0; i < files.Count; i++)
            {
                var targetFilePath = Path.Combine(targetPath, Path.GetRelativePath(sourcePath, files[i]));
                Directory.CreateDirectory(targetFilePath);
                await WaitForFileReleaseAsync(Path.Combine(targetPath, Path.GetFileName(files[i])), CancellationToken.None).ConfigureAwait(false);
                File.Move(files[i], targetFilePath, isOverwrite);
            }
        }

        /// <summary>
        /// 根据给定的命名规则获取一个新文件流
        /// </summary>
        /// <param name="fileName">给定文件路径</param>
        /// <param name="existsFunc">查找文件是否存在的方法</param>
        /// <param name="template">重命名模板</param>
        /// <param name="addition">重命名开始数字</param>
        /// <param name="padLeftWidth">数字补全位数</param>
        /// <returns>新文件流</returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static FileStream GetUniqueFileNameStream(string fileName, Func<string, bool>? existsFunc = null, string template = "({0})", int addition = 2, int padLeftWidth = 0)
        {
            existsFunc ??= File.Exists;
            var dir = Path.GetDirectoryName(fileName) ?? string.Empty;
            if (!OperatingSystem.IsWindows())
                dir=dir.Replace('\\', '/');
            var targetFileName = Path.GetFileNameWithoutExtension(fileName);
            var targetExtension = Path.GetExtension(fileName);
            var additionStr = string.Empty;
            if (padLeftWidth!=0)
            {
                additionStr= string.Format(template, addition.ToString().PadLeft(padLeftWidth, '0'));
                addition++;
            }
            while (existsFunc(Path.Combine(dir, $"{targetFileName}{additionStr}{targetExtension}")))
            {
                if (Math.Log10(addition)+1>padLeftWidth)
                    throw new ArgumentOutOfRangeException(nameof(addition), "超出padLeft位数");
                if (padLeftWidth!=0)
                    additionStr= string.Format(template, addition.ToString().PadLeft(padLeftWidth, '0'));
                else
                    additionStr= string.Format(template, addition);
                addition++;
            }
            ;
            if (OperatingSystem.IsWindows())
                return new FileStream(Path.Combine(dir, $"{targetFileName}{additionStr}{targetExtension}"), FileMode.Create, FileAccess.ReadWrite, FileShare.None);
            return new FileStream(Path.Combine(dir, $"{targetFileName}{additionStr}{targetExtension}").Replace('\\', '/'), FileMode.Create, FileAccess.ReadWrite, FileShare.None);
        }
    }
}
