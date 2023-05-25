using NPOI.SS.UserModel;

namespace VP.Office.Extensions
{
    //todo
    public static class ExcelICellExtensions
    {
        public static string? GetStringValue(this ICell? cell)
        {
            if(cell is null) return null;
            return cell.CellType switch
            {
                CellType.Unknown => cell.ToString(),
                CellType.Numeric => cell.NumericCellValue.ToString(),
                CellType.String or CellType.Formula => cell.StringCellValue,
                CellType.Blank => string.Empty,
                CellType.Boolean => cell.BooleanCellValue.ToString(),
                _ => null,
            };
        }
    }
}
