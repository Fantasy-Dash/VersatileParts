using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System.Text.RegularExpressions;
using UtfUnknown;
using VP.Common.Utils;

namespace VP.Office.Helpers
{//todo 注释
    public static class ExcelHelper
    {
        public static async Task<IWorkbook> ConvertCsvToWorkbook(string csvFilePath)
        {
            var workbook = new XSSFWorkbook();
            var sheet = workbook.CreateSheet();
            var regex = RegexUtils.CsvSpliterRegex();
            using var fs = new FileStream(csvFilePath, FileMode.Open);
            if (fs.Length==0)
                return new XSSFWorkbook();
            var encoding = CharsetDetector.DetectFromStream(fs).Detected.Encoding;
            fs.Position=0;
            using (var sr = new StreamReader(fs, encoding))
            {
                string? rows;
                int rowIndex = 0;
                while ((rows = await sr.ReadLineAsync()) != null)
                {
                    var values = new List<string>();
                    MatchCollection matches = regex.Matches(rows);
                    foreach (Match match in matches.Cast<Match>())
                    {
                        string value = match.Groups[1].Value.Trim('"');
                        values.Add(value);
                    }
                    var iRow = sheet.CreateRow(rowIndex);
                    int colIndex = 0;
                    rowIndex++;
                    foreach (var item in values)
                    {
                        if (!string.IsNullOrWhiteSpace(item))
                        {
                            iRow.CreateCell(colIndex).SetCellValue(item);
                        }
                        colIndex++;
                    }
                }
            }
            return workbook;
        }
    }
}