using PDFiumCore;
using System.Text;

namespace VP.PDF
{
    public static class PDFHelper
    {
        public static string GetAllText(string path, string password)
        {
            var sb = new StringBuilder();
            fpdfview.FPDF_InitLibrary();
            try
            {
                var doc = fpdfview.FPDF_LoadDocument(path, password);
                try
                {
                    var page = fpdfview.FPDF_LoadPage(doc, fpdfview.FPDF_GetPageCount(doc)-1);
                    try
                    {
                        var textPkg = fpdf_text.FPDFTextLoadPage(page);
                        try
                        {
                            for (int i = 0; i < fpdf_text.FPDFTextCountChars(textPkg); i++)
                                sb.Append(Encoding.Unicode.GetString(BitConverter.GetBytes(fpdf_text.FPDFTextGetUnicode(textPkg, i))));
                        }
                        finally
                        {
                            fpdf_text.FPDFTextClosePage(textPkg);
                        }
                    }
                    finally
                    {
                        fpdfview.FPDF_ClosePage(page);
                    }
                }
                finally
                {
                    fpdfview.FPDF_CloseDocument(doc);
                }
            }
            finally
            {
                fpdfview.FPDF_DestroyLibrary();
            }
            return sb.Replace("\0", string.Empty).ToString().Trim();
        }

        public static string GetText(string path, string password, int pageIndex)
        {
            var sb = new StringBuilder();
            fpdfview.FPDF_InitLibrary();
            try
            {
                var doc = fpdfview.FPDF_LoadDocument(path, password);
                try
                {
                    var page = fpdfview.FPDF_LoadPage(doc, pageIndex);
                    try
                    {
                        var textPkg = fpdf_text.FPDFTextLoadPage(page);
                        try
                        {
                            for (int i = 0; i < fpdf_text.FPDFTextCountChars(textPkg); i++)
                                sb.Append(Encoding.Unicode.GetString(BitConverter.GetBytes(fpdf_text.FPDFTextGetUnicode(textPkg, i))));
                        }
                        finally
                        {
                            fpdf_text.FPDFTextClosePage(textPkg);
                        }
                    }
                    finally
                    {
                        fpdfview.FPDF_ClosePage(page);
                    }
                }
                finally
                {
                    fpdfview.FPDF_CloseDocument(doc);
                }
            }
            finally
            {
                fpdfview.FPDF_DestroyLibrary();
            }
            return sb.Replace("\0", string.Empty).ToString().Trim();
        }

        public static int GetPageCount(string path, string password)
        {
            fpdfview.FPDF_InitLibrary();
            try
            {
                var doc = fpdfview.FPDF_LoadDocument(path, password);
                try
                {
                    return fpdfview.FPDF_GetPageCount(doc);
                }
                finally
                {
                    fpdfview.FPDF_CloseDocument(doc);
                }
            }
            finally
            {
                fpdfview.FPDF_DestroyLibrary();
            }
        }
    }
}
