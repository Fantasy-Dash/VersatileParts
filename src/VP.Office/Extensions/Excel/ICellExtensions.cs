using NPOI.SS.UserModel;

namespace VP.Office.Extensions.Excel
{
    //todo 注释
    public static class ExcelICellExtensions
    {
        public static string? GetStringValue(this ICell? cell)
        {
            if (cell is null) return null;
            if (cell.CellType.Equals(CellType.Formula))
            {
                string? ret = null;
                try
                {
                    ret=cell.StringCellValue;
                    return ret;
                }
                catch { }
                try
                {
                    if (cell.DateCellValue.TimeOfDay==TimeSpan.Zero)
                        ret= cell.DateCellValue.ToShortDateString();
                    else if (cell.DateCellValue.Date.Equals(DateTime.MinValue))
                        ret= cell.DateCellValue.ToShortTimeString();
                    else
                        ret=cell.DateCellValue.ToString();
                    return ret;
                }
                catch { }
                try
                {
                    ret= cell.CachedFormulaResultType switch
                    {
                        CellType.Unknown => cell.ToString(),
                        CellType.Numeric => cell.NumericCellValue.ToString(),
                        CellType.String => cell.StringCellValue,
                        CellType.Blank => string.Empty,
                        CellType.Boolean => cell.BooleanCellValue.ToString(),
                        CellType.Error => throw new Exception(cell.ErrorCellValue.ToString()),
                        _ => null,
                    };
                    return ret;
                }
                catch { }
            }
            return cell.CellType switch
            {
                CellType.Unknown => cell.ToString(),
                CellType.Numeric => cell.NumericCellValue.ToString(),
                CellType.String => cell.StringCellValue,
                CellType.Blank => string.Empty,
                CellType.Boolean => cell.BooleanCellValue.ToString(),
                CellType.Error => throw new Exception(cell.ErrorCellValue.ToString()),
                _ => null,
            };
        }
    }
}
