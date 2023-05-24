using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace VP.Common.Utils
{//todo 注释
    public static partial class RegexUtils
    {

        [GeneratedRegex("[0-9]", RegexOptions.IgnoreCase)]
        public static partial Regex GetNumbersRegex();

        [GeneratedRegex("[a-z]", RegexOptions.IgnoreCase)]
        public static partial Regex GetEnglishLettersRegex();
    }
}
