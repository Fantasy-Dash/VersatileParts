using NPOI.SS.UserModel;
using NPOI.SS.Util;
using System.Drawing;
using VP.Common.Utils;

namespace VP.Office.Extensions.Excel
{//todo 注释
    public static class ISheetExtensions
    {
        public static ICell? GetCellByPoint(this ISheet sheet, int x, int y) => sheet.GetOrCreateRow(y)?.GetCell(x);
        public static ICell? GetCellByPoint(this ISheet sheet, Point point) => sheet.GetOrCreateRow(point.Y)?.GetCell(point.X);

        public static ICell? GetCellByExcelCellString(this ISheet sheet, string excleCellString)
        {
            int num = Convert.ToInt32(RegexUtils.GetEnglishLettersRegex().Replace(excleCellString, ""));
            string str = RegexUtils.GetNumbersRegex().Replace(excleCellString, "");
            return sheet.GetOrCreateRow(num - 1)?.GetCell(CellReference.ConvertColStringToIndex(str));
        }

        public static IEnumerable<ICell?> GetColumnCells(this ISheet sheet, int columnIndex, int startIndex = 0)
        {
            var ret = new List<ICell?>();
            for (int i = startIndex; i <= sheet.LastRowNum; i++)
                ret.Add(sheet.GetOrCreateRow(i)?.GetCell(columnIndex));
            return ret;
        }

        public static IEnumerable<ICell?> GetRowCells(this ISheet sheet, int rowIndex, int startIndex = 0)
        {
            var ret = new List<ICell?>();
            var row = sheet.GetRow(rowIndex);
            if (row != null)
                for (int i = startIndex; i <= row.LastCellNum; i++)
                    ret.Add(row.GetCell(i));
            return ret;
        }

        public static IRow GetOrCreateRow(this ISheet sheet, int rowIndex)
        {
            return sheet.GetRow(rowIndex) ?? sheet.CreateRow(rowIndex);
        }
    }
}
