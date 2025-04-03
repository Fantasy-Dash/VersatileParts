using NPOI.SS.UserModel;

namespace VP.Office.Extensions.Excel
{
    //todo 注释
    public static class ExcelICellExtensions
    {
        public static string GetStringValue(this ICell? cell)
        {
            if (cell is null) return string.Empty;
            if (cell.CellType.Equals(CellType.Formula))
            {
                string ret;
                try
                {
                    ret=cell.StringCellValue;
                    return ret;
                }
                catch { }
                try
                {
                    if (cell.DateCellValue.HasValue)
                    {
                        if (cell.DateCellValue.Value.TimeOfDay==TimeSpan.Zero)
                            ret= cell.DateCellValue.Value.ToShortDateString();
                        else if (cell.DateCellValue.Value.Equals(DateTime.MinValue))
                            ret= cell.DateCellValue.Value.ToShortTimeString();
                        else
                            ret=cell.DateCellValue.Value.ToString();
                    }
                    else
                        ret=string.Empty;
                    return ret;
                }
                catch { }
                try
                {
                    ret= cell.CachedFormulaResultType switch
                    {
                        CellType.Unknown => cell.ToString()??string.Empty,
                        CellType.Numeric => cell.NumericCellValue.ToString(),
                        CellType.String => cell.StringCellValue,
                        CellType.Blank => string.Empty,
                        CellType.Boolean => cell.BooleanCellValue.ToString(),
                        CellType.Error => throw new Exception(cell.ErrorCellValue.ToString()),
                        _ => string.Empty,
                    };
                    return ret;
                }
                catch { }
            }
            return cell.CellType switch
            {
                CellType.Unknown => cell.ToString()??string.Empty,
                CellType.Numeric => cell.NumericCellValue.ToString(),
                CellType.String => cell.StringCellValue,
                CellType.Blank => string.Empty,
                CellType.Boolean => cell.BooleanCellValue.ToString(),
                CellType.Error => throw new Exception(cell.ErrorCellValue.ToString()),
                _ => string.Empty,
            };
        }
    }
}
