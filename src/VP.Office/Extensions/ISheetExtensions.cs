using NPOI.SS.Formula.Functions;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using System.Drawing;
using System.Text.RegularExpressions;
using VP.Common.Utils;
using VP.Office.Helpers;

namespace VP.Office.Extensions
{//todo 注释
    public static class ISheetExtensions
    {
        public static ICell? GetCellByPoint(this ISheet sheet, int x,int y) => sheet.GetRow(y)?.GetCell(x);
        public static ICell? GetCellByPoint(this ISheet sheet, Point point) => sheet.GetRow(point.Y)?.GetCell(point.X);

        public static ICell? GetCellByExcelCellString(this ISheet sheet, string excleCellString)
        {
            int num = Convert.ToInt32(RegexUtils.GetEnglishLettersRegex().Replace(excleCellString, ""));
            string str = RegexUtils.GetNumbersRegex().Replace(excleCellString, "");
            return sheet.GetRow(num-1)?.GetCell(ExcelHelper.GetColumnIndexFromColumnLetters(str));
        }

        public static IEnumerable<ICell?> GetColumnCells(this ISheet sheet, int columnIndex, int startIndex = 0)
        {
            var ret = new List<ICell?>();
            for (int i = startIndex; i <= sheet.LastRowNum; i++)
                ret.Add(sheet.GetRow(i)?.GetCell(columnIndex));
            return ret;
        }

        public static IEnumerable<ICell?> GetRowCells(this ISheet sheet, int rowIndex, int startIndex = 0)
        {
            var ret = new List<ICell?>();
            var row = sheet.GetRow(rowIndex);
            for (int i = startIndex; i <= sheet.LastRowNum; i++)
                ret.Add(row?.GetCell(i));
            return ret;
        }
    }
}
