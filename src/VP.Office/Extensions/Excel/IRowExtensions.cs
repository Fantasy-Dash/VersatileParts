using NPOI.SS.UserModel;

namespace VP.Office.Extensions.Excel
{
    public static class IRowExtensions
    {
        public static ICell GetOrCreateCell(this IRow row, int colIndex)
        {
            return row.GetCell(colIndex) ?? row.CreateCell(colIndex);
        }
    }
}
