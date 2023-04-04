using NPOI.HSSF.UserModel;
using NPOI.POIFS.FileSystem;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

namespace VP.Office.Helpers
{//todo 注释
    public static class ExcelHelper
    {
        public static IEnumerable<ISheet> ReadFileToSheetList(string filePath)
        {
            var ret = new List<ISheet>();
            try
            {
                using var fs = new FileStream(filePath, FileMode.Open,FileAccess.Read,FileShare.Read);
                var xssWorkbook = new HSSFWorkbook(fs);
                for (int i = 0; i < xssWorkbook.NumberOfSheets; i++)
                    ret.Add(xssWorkbook.GetSheetAt(i));
            }
            catch (OfficeXmlFileException)
            {
                var file = new FileInfo(filePath);
                var xssWorkbook = new XSSFWorkbook(file);
                for (int i = 0; i < xssWorkbook.NumberOfSheets; i++)
                    ret.Add(xssWorkbook.GetSheetAt(i));
            }
            return ret;
        }

        /// <summary>
        /// 将字母串转换为数字
        /// </summary>
        /// <param name="letters">要转换的字母串</param>
        /// <returns>转换后的数字</returns>
        public static int GetColumnIndexFromLetters(string? letters)
        {
            if (string.IsNullOrWhiteSpace(letters))
                throw new ArgumentException(null, nameof(letters));
            int result = 0;
            int power = 1;
            for (int i = letters.Length - 1; i >= 0; i--)//倒序遍历
            {
                char letter = letters[i];
                int value = letter - 'A' + 1; // 将字母转换为数字（A对应1，B对应2，以此类推）
                result += value * power;
                power *= 26;
            }
            return result - 1;
        }
    }
}