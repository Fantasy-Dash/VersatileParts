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
            value = GetValue<T>(path, key);
            if (value is string)
                return !string.IsNullOrWhiteSpace(value as string);
            return value != null;
        }

        /// <summary>
        /// 获取指定Json值
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static T? GetValue<T>(string path, string key)
        {
            var jsonNode = FileHelper.ReadToType<JsonNode>(path);
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
        public static bool TrySetValue<T>(string path, string key, T value)
        {
            try
            {
                JsonNode? jsonNode = FileHelper.ReadToType<JsonNode>(path);
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
        public static void SetValue<T>(string path, string key, T value)
        {
            if (!TrySetValue(path, key, value))
            {
                throw new Exception("指定配置文件修改异常");
            }
        }

        /// <summary>
        /// 合并json文件
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static bool TryMergeFile(string filePath, string mergeFilePath)
        {
            try
            {
                JsonNode? jsonNode = FileHelper.ReadToType<JsonNode>(filePath);
                JsonNode? mergeJsonNode = FileHelper.ReadToType<JsonNode>(mergeFilePath);

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
            catch (Exception)
            {
                return false;
            }
        }

    }
}
