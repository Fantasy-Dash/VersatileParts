using NPOI.SS.UserModel;
using System.Drawing;
using VP.Office.Helpers;

namespace VP.Office.Extensions
{//todo 注释
    public static class ISheetExtensions
    {

        public static ICell? GetCellByPoint(this ISheet sheet, Point point) => sheet.GetRow(point.X)?.GetCell(point.Y);

        public static IEnumerable<ICell?> GetColumnCells(this ISheet sheet, int columnIndex, int startIndex = 0)
        {
            var ret = new List<ICell?>();
            for (int i = startIndex; i <= sheet.LastRowNum; i++)
                ret.Add(sheet.GetRow(i)?.GetCell(columnIndex));
            return ret;
        }
    }
}
