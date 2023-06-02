using NPOI.SS.UserModel;

namespace VP.Office.Extensions
{
    public static class IWorkbookExtensions
    {
        public static ISheet GetOrCreateSheet(this IWorkbook workbook,string sheetName)
        {
           return workbook.GetSheet(sheetName)??workbook.CreateSheet(sheetName);
        }
    }
}
