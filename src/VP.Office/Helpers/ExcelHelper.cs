using NPOI.HSSF.UserModel;
using NPOI.POIFS.FileSystem;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using NPOI.XSSF.UserModel;
using VP.Common.Helpers;

namespace VP.Office.Helpers
{//todo 注释
    public static class ExcelHelper
    {
        public static IEnumerable<ISheet> ReadFileToSheetList(string filePath)
        {
            if (FileHelper.IsFileUsing(filePath))
                throw new FileLoadException("文件已被占用");
            var ret = new List<ISheet>();
            try
            {
                var workbook = new XSSFWorkbook(new FileInfo(filePath));
                workbook.GetCreationHelper().CreateFormulaEvaluator().EvaluateAll();
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
                    workbook.GetCreationHelper().CreateFormulaEvaluator().EvaluateAll();
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
        public static int GetColumnIndexFromColumnLetters(string? letters) => CellReference.ConvertColStringToIndex(letters);

        /// <summary>
        /// 将数字转换为字母串
        /// </summary>
        /// <param name="index">要转换的数字</param>
        /// <returns>转换后的字母串</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/>不能为负数</exception>
        public static string GetColumnLettersFromColumnIndex(int index) => CellReference.ConvertNumToColString(index);
    }
}