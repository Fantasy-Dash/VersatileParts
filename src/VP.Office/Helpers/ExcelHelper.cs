using NPOI.HSSF.UserModel;
using NPOI.POIFS.FileSystem;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System.Text;

namespace VP.Office.Helpers
{//todo 注释
    public static class ExcelHelper
    {
        public static IEnumerable<ISheet> ReadFileToSheetList(string filePath)
        {
            var ret = new List<ISheet>();
            try
            {
                var workbook = new XSSFWorkbook(new FileInfo(filePath));
                for (int i = 0; i < workbook.NumberOfSheets; i++)
                    ret.Add(workbook.GetSheetAt(i));
                workbook.Dispose();
            }
            catch (OfficeXmlFileException)
            {
                var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
                try
                {
                    IWorkbook workbook = new HSSFWorkbook(fs);
                    for (int i = 0; i < workbook.NumberOfSheets; i++)
                        ret.Add(workbook.GetSheetAt(i));
                    workbook.Dispose();
                }
                finally
                {
                    fs.Dispose();
                }
            }
            return ret;
        }

        public static IEnumerable<ISheet> ReadStreamToSheetList(Stream stream)
        {
            var ret = new List<ISheet>();
            try
            {
                var workbook = new XSSFWorkbook(stream);
                for (int i = 0; i < workbook.NumberOfSheets; i++)
                    ret.Add(workbook.GetSheetAt(i));
                workbook.Dispose();
                stream.Dispose();
            }
            catch (OfficeXmlFileException)
            {
                try
                {
                    IWorkbook workbook = new HSSFWorkbook(stream);
                    for (int i = 0; i < workbook.NumberOfSheets; i++)
                        ret.Add(workbook.GetSheetAt(i));
                    workbook.Dispose();
                }
                finally
                {
                    stream.Dispose();
                }
            }
            return ret;
        }

        /// <summary>
        /// 将字母串转换为数字
        /// </summary>
        /// <param name="letters">要转换的字母串</param>
        /// <returns>转换后的数字</returns>
        public static int GetColumnIndexFromColumnLetters(string? letters)
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

        /// <summary>
        /// 将数字转换为字母串
        /// </summary>
        /// <param name="index">要转换的数字</param>
        /// <returns>转换后的字母串</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/>不能为负数</exception>
        public static string GetColumnLettersFromColumnIndex(int index)
        {
            if (index<0) throw new ArgumentOutOfRangeException(nameof(index));
            var result = new StringBuilder();
            while (index/26>0)
            {
                _=result.Append((char)((index/26)+'A'-1));
                index%=26;
            }
            _=result.Append((char)(index+'A'));
            return result.ToString();
        }
    }
}