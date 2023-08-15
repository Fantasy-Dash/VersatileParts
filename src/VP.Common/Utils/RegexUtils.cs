using System.Text.RegularExpressions;

namespace VP.Common.Utils
{//todo 注释
    public static partial class RegexUtils
    {

        [GeneratedRegex("[0-9]", RegexOptions.Compiled)]
        public static partial Regex GetNumbersRegex();

        [GeneratedRegex("[a-z]", RegexOptions.Compiled)]
        public static partial Regex GetEnglishLettersRegex();

        [GeneratedRegex(@"(-?\d+)(\.\d+)",RegexOptions.Compiled)]
        public static partial Regex GetFirstDoubleRegex();

        [GeneratedRegex("(?:^|,)(\"(?:[^\"]+|\"\")*\"|[^,\"]*)", RegexOptions.Compiled)]
        public static partial Regex CsvSpliterRegex();
    }
}
