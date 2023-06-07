using NPOI.SS.UserModel;

namespace VP.Office.Extensions.Excel
{
    public static class IWorkbookExtensions
    {
        public static ISheet GetOrCreateSheet(this IWorkbook workbook, string sheetName)
        {
            return workbook.GetSheet(sheetName) ?? workbook.CreateSheet(sheetName);
        }

        public static IFont FindFont(this IWorkbook workbook,
                                     short baseFontIndex,
                                     bool isbold,
                                     short colorIndex,
                                     short fontHeight,
                                     string fontName,
                                     bool isItalic,
                                     bool isStrikeout,
                                     FontSuperScript typeOffset,
                                     FontUnderlineType underline,
                                     string defaultFontName = "")
        {
            var existsFont = workbook.FindFont(isbold, colorIndex, fontHeight, fontName, isItalic, isStrikeout, typeOffset, underline);
            if (existsFont!=null) return existsFont;
            var newFont = workbook.CreateFont();
            newFont.IsBold=isbold;
            newFont.IsItalic=isItalic;
            newFont.Color=colorIndex;
            newFont.FontName=fontName;
            newFont.IsStrikeout=isStrikeout;
            newFont.TypeOffset=typeOffset;
            newFont.Underline=underline;
            newFont.FontHeight=fontHeight;
            if (baseFontIndex==0)
                newFont.FontName= string.IsNullOrWhiteSpace(defaultFontName)
                    ? workbook.GetFontAt(0).FontName
                    : defaultFontName;
            return newFont;
        }
    }
}
