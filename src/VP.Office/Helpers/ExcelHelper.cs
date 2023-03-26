using NPOI.HSSF.UserModel;
using NPOI.POIFS.FileSystem;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

namespace VP.Office.Helpers
{//todo 注释
    public static class ExcelHelper
    {
        public static IEnumerable<ISheet> ReadFileToSheetList(string filePath)
        {
            var ret = new List<ISheet>();
            try
            {
                using var fs = new FileStream(filePath, FileMode.Open);
                var xssWorkbook = new HSSFWorkbook(fs);
                for (int i = 0; i < xssWorkbook.NumberOfSheets; i++)
                    ret.Add(xssWorkbook.GetSheetAt(i));
            }
            catch (OfficeXmlFileException)
            {
                var file = new FileInfo(filePath);
                var xssWorkbook = new XSSFWorkbook(file);
                for (int i = 0; i < xssWorkbook.NumberOfSheets; i++)
                    ret.Add(xssWorkbook.GetSheetAt(i));
            }
            return ret;
        }

        public static IRow ReadHeadRow(ISheet sheet) => sheet.GetRow(0);

        public static IEnumerable<ICell> ReadRowToList(IRow row, int startIndex = 0)
        {
            var ret = new List<ICell>();
            for (int i = startIndex; i <= row.LastCellNum; i++)
                ret.Add(row.GetCell(i));
            return ret;
        }

        public static IEnumerable<ICell> ReadColumnToList(ISheet sheet, int startIndex = 0, int columnIndex = 0)
        {
            var ret = new List<ICell>();
            for (int i = startIndex; i <= sheet.LastRowNum; i++)
                ret.Add(sheet.GetRow(i).GetCell(columnIndex));
            return ret;
        }

    }
}