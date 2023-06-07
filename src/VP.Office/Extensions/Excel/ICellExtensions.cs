using NPOI.SS.UserModel;

namespace VP.Office.Extensions.Excel
{
    //todo
    public static class ExcelICellExtensions
    {
        public static string? GetStringValue(this ICell? cell)
        {
            if (cell is null) return null;
            return cell.CellType switch
            {
                CellType.Unknown => cell.ToString(),
                CellType.Numeric => cell.NumericCellValue.ToString(),
                CellType.String or CellType.Formula => cell.StringCellValue,
                CellType.Blank => string.Empty,
                CellType.Boolean => cell.BooleanCellValue.ToString(),
                CellType.Error => throw new Exception(cell.ErrorCellValue.ToString()),
                _ => null,
            };
        }
    }
}
