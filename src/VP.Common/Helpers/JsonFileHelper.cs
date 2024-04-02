using System.Text.Json.Nodes;
using VP.Common.Extensions;

namespace VP.Common.Helpers
{
    /// <summary>
    /// Json文件读写Helper
    /// </summary>
    public static class JsonFileHelper
    {

        /// <summary>
        /// 尝试获取指定Json值
        /// 能获取到返回则true
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static bool TryGetValue<T>(string path, string key, out T? value)
        {
            value = GetValueAsync<T>(path, key).Result;
            if (value is string)
                return !string.IsNullOrWhiteSpace(value as string);
            return value != null;
        }

        /// <summary>
        /// 获取指定Json值
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static async Task<T?> GetValueAsync<T>(string path, string key)
        {
            var str = await FileHelper.ReadToStringAsync(path)??throw new ArgumentException("文件读取失败", nameof(path));
            var jsonNode = JsonNode.Parse(str);
            var jsonNodeChild = jsonNode?.GetChild(key);
            return jsonNodeChild is null
                || jsonNodeChild.ToJsonString().Equals("{}")
                ? default
                : jsonNodeChild.GetValue<T>();
        }

        /// <summary>
        /// 设置指定Json值
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static async Task<bool> TrySetValueAsync<T>(string path, string key, T value)
        {
            try
            {
                var json = await FileHelper.ReadToStringAsync(path)??throw new ArgumentNullException(nameof(path));
                JsonNode? jsonNode = JsonNode.Parse(json);
                var ret = jsonNode?.GetChild(key).TrySetValue(value);
                if (ret is true) jsonNode.WriteTo(path);
                return ret is true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// 设置指定Json值
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static async Task SetValueAsync<T>(string path, string key, T value)
        {
            if (!await TrySetValueAsync(path, key, value))
            {
                throw new Exception("指定配置文件修改异常");
            }
        }

        /// <summary>
        /// 合并json文件
        /// </summary>
        /// <param name="filePath">源文件</param>
        /// <param name="mergeFilePath">要进行合并的文件 会将不同合并到源文件中</param>
        /// <returns></returns>
        public static async Task<bool> TryMergeFileAsync(string filePath, string mergeFilePath)
        {
            try
            {
                var json = await FileHelper.ReadToStringAsync(filePath)??throw new ArgumentNullException(nameof(filePath));
                var mergeJson = await FileHelper.ReadToStringAsync(mergeFilePath)??throw new ArgumentNullException(nameof(mergeFilePath));

                JsonNode? jsonNode = JsonNode.Parse(json);
                JsonNode? mergeJsonNode = JsonNode.Parse(mergeJson);

                var skipMergingField = new List<string>();
                var skipMergingFieldArray = mergeJsonNode?.GetChild("KeepMergingField")?.AsArray();
                if (skipMergingFieldArray != null)
                    foreach (var item in skipMergingFieldArray)
                        if (item != null)
                            skipMergingField.Add(item.GetValue<string>());
                var obsoleteField = new List<string>();
                var obsoleteFieldArray = mergeJsonNode?.GetChild("ObsoleteField")?.AsArray();
                if (obsoleteFieldArray != null)
                    foreach (var item in obsoleteFieldArray)
                        if (item != null)
                            obsoleteField.Add(item.GetValue<string>());
                jsonNode.Merge(mergeJsonNode, skipMergingField, obsoleteField)
                    .WriteTo(filePath);

                return true;
            }
            catch
            {
                return false;
            }
        }

    }
}
