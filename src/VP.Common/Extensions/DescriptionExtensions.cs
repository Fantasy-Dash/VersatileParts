using System.ComponentModel;
using System.Reflection;

namespace VP.Common.Extensions
{
    /// <summary>
    /// 描述特性的读取扩展类
    /// </summary>
    public static class DescriptionExtensions
    {
        /// <summary>
        /// 获得枚举的 <see cref="DescriptionAttribute"/> 属性值
        /// </summary>
        /// <param name="value">枚举值</param>
        /// <param name="nameInstead">当枚举值没有定义<see cref="DescriptionAttribute"/>，是否使用枚举名代替，默认是使用</param>
        /// <returns>枚举的Description属性值</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="value" /> 是 <see langword="null" />
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="value" /> 的基础类型不是 <see cref="Enum" />
        /// </exception>
        /// <inheritdoc cref="Attribute.GetCustomAttribute(Assembly, Type)"/>
        /// <inheritdoc cref="Type.GetField(string)"/>
        public static string? GetDescription(this Enum value, bool nameInstead = true)
        {
            Type type = value.GetType();
            string? name = Enum.GetName(type, value);
            if (name == null) return null;
            FieldInfo? field = type.GetField(name);
            if (field == null) return null;
            DescriptionAttribute? attribute = Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) as DescriptionAttribute;
            if (attribute == null && nameInstead) return name;
            return attribute?.Description;
        }

        /// <param name="type">类型</param>
        /// <param name="propertyName">属性名</param>
        /// <param name="nameInstead">当类型没有定义<see cref="DescriptionAttribute"/>，是否使用属性名代替，默认是使用</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="type" /> 是 <see langword="null" />
        /// </exception>
        /// <exception cref="AmbiguousMatchException">无法匹配到属性</exception>
        /// <inheritdoc cref="GetDescription(MemberInfo, string, bool)"/>
        /// <inheritdoc cref="Type.GetProperty(string)"/>
        public static string? GetDescription(this Type type, string propertyName, bool nameInstead = true)
        {
            var pro = type.GetProperty(propertyName);
            return pro != null ? pro.GetDescription(propertyName, nameInstead) : propertyName;
        }



        /// <summary>
        /// 获得类型的 <see cref="DescriptionAttribute"/> 属性值
        /// </summary>
        /// <param name="info">类型</param>
        /// <param name="name">属性名</param>
        /// <param name="nameInstead">当类型没有定义<see cref="DescriptionAttribute"/>，是否使用属性名代替，默认是使用</param>
        /// <returns>类型的 <see cref="DescriptionAttribute"/> 属性值</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="info" /> 是 <see langword="null" />
        /// </exception>
        /// <exception cref="AmbiguousMatchException">无法匹配到属性</exception>
        /// <inheritdoc cref="Type.GetField(string)"/>
        /// <inheritdoc cref="Attribute.GetCustomAttribute(Assembly, Type)"/>
        public static string? GetDescription(this MemberInfo info, string name, bool nameInstead = true)
        {
            Type type = info.GetType();
            if (name == null) return null;
            var field = type.GetField(name);
            if (field is null) return null;
            var attribute = Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) as DescriptionAttribute;
            if (attribute is null && nameInstead)
                return name;
            return attribute?.Description;
        }
    }
}
