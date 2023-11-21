using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Unicode;

namespace VP.Common.Extensions
{
    /// <summary>
    /// Json节点扩展
    /// </summary>
    public static class JsonNodeExtensions
    {
        /// <summary>
        /// 尝试获取 <see cref="JsonValue" /> 的值
        /// </summary>
        /// <param name="jsonNode">json节点</param>
        /// <param name="jValue"> <see cref="JsonValue" /> 的值</param>
        /// <returns>获取Json值是否成功</returns>
        public static bool TryGetValue<T>(this JsonNode? jsonNode, out T? jValue)
        {
            jValue = default;
            try
            {
                if (jsonNode is null) return false;
                if (typeof(T).Equals(typeof(JsonArray)))
                    jValue = (T?)(object?)JsonNode.Parse(jsonNode.AsArray().ToJsonString());
                else
                    jValue = jsonNode.GetValue<T>();
                return jValue != null;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// 获取 <see cref="JsonValue" /> 的值
        /// </summary>
        /// <param name="jsonNode">json节点</param>
        /// <param name="key"> <see cref="JsonNode" /> 的key 使用分隔符进行分隔</param>
        /// <param name="isCreateNewNode">当没找到节点时 是否创建新节点</param>
        /// <param name="separator">key使用的分隔符 默认使用":"</param>
        /// <returns>返回找到的子节点</returns>
        public static JsonNode? GetChild(this JsonNode jsonNode, string key, bool isCreateNewNode = true, string separator = ":")
        {
            var keys = key.Split(separator).ToList();
            JsonNode jNode = jsonNode;
            foreach (var k in keys)
            {
                if (jNode[k] is null && isCreateNewNode)
                    jNode[k] = new JsonObject();
                else if (jNode[k] is null)
                    return null;
                jNode = jNode[k] ?? new JsonObject();

            }
            return jNode;
        }

        /// <summary>
        /// 获取 <see cref="JsonNode" /> 的key
        /// </summary>
        /// <param name="jsonNode">json节点</param>
        /// <returns>key的名称</returns>
        /// <inheritdoc cref="Enumerable.Last{TSource}(IEnumerable{TSource})"/>
        public static string GetKey(this JsonNode jsonNode) => jsonNode.GetPath().Split(".").Last();

        /// <summary>
        /// 尝试设置<see cref="JsonNode"/>的值
        /// </summary>
        /// <param name="jsonNode">Json节点</param>
        /// <param name="jsonValue">Json值</param>
        /// <returns>返回是否设置成功的布尔值</returns>
        public static bool TrySetValue<T>(this JsonNode? jsonNode, T? jsonValue)
        {
            return jsonNode.SetValue(jsonValue) != null;
        }

        /// <summary>
        /// 设置Json值
        /// </summary>
        /// <param name="jsonNode">Json节点</param>
        /// <param name="jsonValue">Json值</param>
        /// <returns>返回Json节点</returns>
        public static JsonNode? SetValue<T>(this JsonNode? jsonNode, T? jsonValue)
        {
            if (jsonNode == null || jsonNode.Parent == null || jsonValue == null) return null;
            if (typeof(T).IsSubclassOf(typeof(JsonNode)))
            {
                if (typeof(T).Equals(typeof(JsonArray)))
                    jsonNode.Parent[jsonNode.GetKey()] = (JsonArray)(object)jsonValue;
                if (typeof(T).Equals(typeof(JsonValue)))
                    jsonNode.Parent[jsonNode.GetKey()] = (JsonValue)(object)jsonValue;
                if (typeof(T).Equals(typeof(JsonObject)))
                    jsonNode.Parent[jsonNode.GetKey()] = (JsonObject)(object)jsonValue;
            }
            else
                jsonNode.Parent[jsonNode.GetKey()] = JsonValue.Create(jsonValue);

            return jsonNode;
        }

        /// <summary>
        /// 存储 <paramref name="node"/> 到 <paramref name="filePath"/> 中
        /// </summary>
        /// <param name="node">Json节点</param>
        /// <param name="filePath">文件路径</param>
        /// <inheritdoc cref="FileStream(string,FileMode)"/>
        /// <inheritdoc cref="Utf8JsonWriter(Stream,JsonWriterOptions)"/>
        /// <inheritdoc cref="JsonNode.WriteTo(Utf8JsonWriter, JsonSerializerOptions?)"/>
        public static void WriteTo(this JsonNode? node, string filePath)
        {
            if (node == null) return;
            using var wfs = new FileStream(filePath, FileMode.Create);
            using var jw = new Utf8JsonWriter(wfs, new()
            {
                Indented = true,
                Encoder = JavaScriptEncoder.Create(UnicodeRanges.All)
            });
            node.WriteTo(jw);
        }

        /// <summary>
        /// 合并两个JsonNode
        /// </summary>
        /// <param name="node"><see cref="JsonNode"/></param>
        /// <param name="mergeNode">需要合并的 <see cref="JsonNode"/></param>
        /// <param name="skipMergingField">需要跳过合并的字段</param>
        /// <param name="obsoleteField">需要删除的字段</param>
        /// <returns>返回 <see cref="JsonNode"/> 传播以进行后续操作</returns>
        /// <exception cref="JsonException">
        /// Json不能表示有效的单个 <see cref="JsonValue" />
        /// </exception>
        /// <exception cref="FormatException">
        /// 当前 <see cref="JsonNode"/> 不能表示为 {T}.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// 当前 <see cref="JsonNode"/> 不是一个 <see cref="JsonValue"/> 或无法表示为 {T}.
        /// </exception>
        public static JsonNode? Merge(this JsonNode? node, JsonNode? mergeNode, List<string> skipMergingField, List<string> obsoleteField)
        {
            if (mergeNode == null) return node;
            if (node == null)
            {
                node=mergeNode;
                return node;
            }
            obsoleteField.ForEach(item => node.Remove(item));
            var allKeyList = new List<string>();
            mergeNode.GetAllKey(ref allKeyList);
            skipMergingField.RemoveAll(row => node.GetChild(row, false) is null);
            allKeyList.RemoveAll(skipMergingField.Contains);
            allKeyList.ForEach(row =>
            {
                if (!mergeNode.GetChild(row).TryGetValue<string>(out var jValue))
                {
                    if (mergeNode.GetChild(row).TryGetValue<JsonArray>(out var jArrayValue))
                        node.GetChild(row).SetValue(jArrayValue);
                }
                else
                    node.GetChild(row).SetValue(jValue);
            });
            return node;
        }

        /// <summary>
        /// 获取当前节点下所有Key 使用提供的分隔符做分隔
        /// </summary>
        /// <param name="node"><see cref="JsonNode"/></param>
        /// <param name="separator">多层级分隔符</param>
        /// <returns>返回所有Key的列表</returns>
        public static List<string> GetAllKey(this JsonNode node, string separator = ":")
        {
            var ret = new List<string>();
            //使用一个返回对象做递归 应该能减小一部分开销
            node.GetAllKey(ref ret, separator);
            return ret;
        }
        private static void GetAllKey(this JsonNode node, ref List<string> ret, string separator = ":")
        {
            if (node.GetType().IsSubclassOf(typeof(JsonArray))
                || node.GetType().Equals(typeof(JsonArray)))
            {
                ret.Add(node.GetPath().Replace(".", separator).Remove(0, 2));
                return;
            }
            if (node.GetType().IsSubclassOf(typeof(JsonValue))
                || node.GetType().Equals(typeof(JsonValue)))
            {
                ret.Add(node.GetPath().Replace(".", separator).Remove(0, 2));
                return;
            }
            foreach (var item in node.AsObject())
            {
                if (item.Value is null) continue;
                item.Value.GetAllKey(ref ret, separator);
            }
        }

        /// <summary>
        /// 从提供的 <see cref="JsonNode"/> 中移除指定的Key
        /// </summary>
        /// <param name="node"><see cref="JsonNode"/></param>
        /// <param name="key">JsonKey</param>
        /// <param name="isRemoveParentObj">如果父级为空 是否移除父级</param>
        /// <param name="separator">JsonKey中使用的多层级分隔符</param>
        /// <exception cref="JsonException">操作Json异常</exception>
        public static void Remove(this JsonNode? node, string key, bool isRemoveParentObj = true, string separator = ":")
        {
            if (node is null) return;
            var array = key.Split(separator).ToList();
            var cNode = node;
            foreach (var item in array)
                cNode = cNode?.GetChild(item);
            if (cNode!=null)
                cNode = cNode.Parent;
            for (int i = array.Count - 1; i >= 0; i--)
            {
                if (cNode is null) break;
                var success = cNode.AsObject().Remove(array[i]);
                if (!success) throw new JsonException("target=" + cNode.ToJsonString() + "key=" + array[i]);
                if (cNode.AsObject().Count > 0 || !isRemoveParentObj) break;
                cNode = cNode.Parent;
            }
        }
    }
}
